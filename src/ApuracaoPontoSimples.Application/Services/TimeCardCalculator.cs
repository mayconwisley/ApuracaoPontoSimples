using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Domain.Enums;

namespace ApuracaoPontoSimples.Application.Services;

public sealed class TimeCardCalculator : ITimeCardCalculator
{
    private const double NightFactor = 27.4;

    public TimeCard Calculate(TimeCard timeCard, ScheduleConfig schedule)
    {
        if (timeCard.Days.Count == 0)
        {
            timeCard.Totals = new TimeCardTotals();
            return timeCard;
        }

        var context = CalculationContext.FromSchedule(schedule);
        var state = new CalculationState();
        var totals = new TimeCardTotals();

        foreach (var day in timeCard.Days.OrderBy(d => d.Date))
        {
            var calc = CalculateDay(day, context, state);
            day.Calculation = calc;

            ApplyTotals(totals, calc);
        }

        timeCard.Totals = totals;
        return timeCard;
    }

    private static DayCalculation CalculateDay(DayEntry day, CalculationContext context, CalculationState state)
    {
        var calc = new DayCalculation();
        var entries = ReadEntries(day);
        var apuracao = CalculateApuracao(entries);
        var horaSabado = apuracao - (context.SaturdayHours ?? TimeSpan.Zero);

        ApplyApuracao(calc, apuracao);
        ApplyWeeklyHours(day, calc, context, state);

        var bancoHora = CalculateBancoHora(day, calc.Apuracao ?? TimeSpan.Zero, apuracao, context);
        ApplyBancoHoras(day, calc, bancoHora, horaSabado, context);
        ApplyBancoTolerance(calc, context);

        var horaExtra = CalculateHoraExtra(day, calc.Apuracao ?? TimeSpan.Zero, context);
        ApplyHorasExtras(day, calc, horaExtra, apuracao, context);
        ApplyExtraTolerance(calc, context);

        ApplyDsrFeriadoRules(day, calc, apuracao);
        ApplyNightAdditional(calc, entries, context);
        FinalizeDsrFeriadoConversion(calc);

        return calc;
    }

    private static TimeEntries ReadEntries(DayEntry day)
        => new(
            day.Interval1.Start ?? TimeSpan.Zero,
            day.Interval1.End ?? TimeSpan.Zero,
            day.Interval2.Start ?? TimeSpan.Zero,
            day.Interval2.End ?? TimeSpan.Zero,
            day.Interval3.Start ?? TimeSpan.Zero,
            day.Interval3.End ?? TimeSpan.Zero);

    private static TimeSpan CalculateApuracao(TimeEntries entries)
        => (entries.Saida3 - entries.Entrada3)
         + (entries.Saida2 - entries.Entrada2)
         + (entries.Saida1 - entries.Entrada1);

    private static void ApplyApuracao(DayCalculation calc, TimeSpan apuracao)
    {
        calc.Apuracao = apuracao == TimeSpan.Zero ? null : apuracao;
        calc.ApuracaoConv = ToHours(calc.Apuracao);
        if (calc.ApuracaoConv == 0)
            calc.ApuracaoConv = null;
    }

    private static void ApplyWeeklyHours(DayEntry day, DayCalculation calc, CalculationContext context, CalculationState state)
    {
        if (!day.IsSunday)
        {
            state.TotalHoraSemana += calc.Apuracao ?? TimeSpan.Zero;
            calc.AcumuladoSemana = state.TotalHoraSemana;
        }
        else
        {
            state.TotalHoraSemana = TimeSpan.Zero;
        }

        if (!day.IsSaturday || !context.WeeklyHours.HasValue)
            return;

        var fechamento = (calc.AcumuladoSemana ?? TimeSpan.Zero) - context.WeeklyHours.Value;
        if (fechamento > TimeSpan.Zero)
            calc.FechamentoSemanaPositivo = fechamento;
        else if (fechamento < TimeSpan.Zero)
            calc.FechamentoSemanaNegativo = fechamento;
    }

    private static TimeSpan CalculateBancoHora(
        DayEntry day,
        TimeSpan apuracaoDoDia,
        TimeSpan apuracaoRaw,
        CalculationContext context)
    {
        var bancoHora = day.Code == DayCode.CF
            ? apuracaoDoDia - context.HoraFerias
            : apuracaoDoDia - context.HoraDia;

        if (day.Code == DayCode.CF)
        {
            if (apuracaoRaw > context.HoraFerias)
                bancoHora = context.Y13 - context.HoraDia;
        }
        else
        {
            if (apuracaoRaw > context.Y13)
                bancoHora = context.Y13 - context.HoraDia;
        }

        if (day.Absence is { Type: AbsenceType.BancoDeHoras } && day.Absence.Hours.HasValue)
            bancoHora = day.Absence.Hours.Value;

        return bancoHora;
    }

    private static void ApplyBancoHoras(
        DayEntry day,
        DayCalculation calc,
        TimeSpan bancoHora,
        TimeSpan horaSabado,
        CalculationContext context)
    {
        if (context.SaturdayHours.HasValue)
            horaSabado = (calc.Apuracao ?? TimeSpan.Zero) - context.SaturdayHours.Value;

        calc.BancoHoras = bancoHora == TimeSpan.Zero ? null : bancoHora;
        calc.BancoHorasConv = ToHours(calc.BancoHoras);

        if (bancoHora > TimeSpan.Zero)
        {
            calc.HorasPositivas = bancoHora;
            calc.HorasPositivasConv = ToHours(calc.HorasPositivas);
        }

        if (bancoHora < TimeSpan.Zero)
        {
            calc.HorasNegativas = day.IsSaturday && context.SaturdayCountsAsBank
                ? horaSabado
                : bancoHora;

            if (ShouldConvertNegativeHours(day))
                calc.HorasNegativasConv = ToHours(calc.HorasNegativas);
        }

        if (day.IsSunday)
        {
            calc.BancoHoras = null;
            calc.HorasNegativas = null;
            calc.BancoHorasConv = null;
            calc.HorasNegativasConv = null;
        }

        if (day.IsSaturday)
        {
            calc.BancoHoras = context.SaturdayCountsAsBank ? horaSabado : null;
            calc.BancoHorasConv = ToHours(calc.BancoHoras);
            if (calc.BancoHorasConv == 0)
                calc.BancoHorasConv = null;
        }
    }

    private static void ApplyBancoTolerance(DayCalculation calc, CalculationContext context)
    {
        if (!calc.BancoHoras.HasValue)
            return;

        if (calc.BancoHoras.Value > context.Tolerancia || calc.BancoHoras.Value < context.ToleranciaNegativa)
            return;

        calc.BancoHoras = null;
        calc.BancoHorasConv = null;
        calc.HorasPositivas = null;
        calc.HorasNegativas = null;
        calc.HorasPositivasConv = null;
        calc.HorasNegativasConv = null;
    }

    private static TimeSpan CalculateHoraExtra(DayEntry day, TimeSpan apuracaoDoDia, CalculationContext context)
        => day.Code == DayCode.CF
            ? apuracaoDoDia - context.HoraFerias
            : apuracaoDoDia - context.Y13;

    private static void ApplyHorasExtras(
        DayEntry day,
        DayCalculation calc,
        TimeSpan horaExtra,
        TimeSpan apuracaoRaw,
        CalculationContext context)
    {
        if (horaExtra <= TimeSpan.Zero)
            return;

        calc.HorasExtras = (day.IsSaturday && !context.SaturdayCountsAsBank)
            ? apuracaoRaw
            : horaExtra;
        calc.HorasExtrasConv = ToHours(calc.HorasExtras);
    }

    private static void ApplyExtraTolerance(DayCalculation calc, CalculationContext context)
    {
        if (!calc.HorasExtras.HasValue)
            return;

        if (calc.HorasExtras.Value > context.Tolerancia)
            return;

        calc.HorasExtras = null;
        calc.HorasExtrasConv = null;
    }

    private static void ApplyDsrFeriadoRules(DayEntry day, DayCalculation calc, TimeSpan apuracaoRaw)
    {
        if (day.IsSunday && calc.Apuracao.HasValue)
        {
            calc.HorasDsrFeriado = apuracaoRaw;
            calc.HorasExtras = null;
            calc.HorasExtrasConv = null;
        }

        if (day.HolidayType == HolidayType.Feriado)
        {
            calc.BancoHoras = null;
            calc.HorasDsrFeriado = apuracaoRaw;
            calc.HorasExtras = null;
            calc.HorasNegativas = null;
            calc.HorasPositivas = null;
            calc.BancoHorasConv = null;
            calc.HorasPositivasConv = null;
            calc.HorasNegativasConv = null;
            calc.HorasExtrasConv = null;
            return;
        }

        if (day.HolidayType != HolidayType.Dsr)
            return;

        calc.BancoHoras = null;
        calc.HorasExtras = null;
        calc.HorasNegativas = null;
        calc.HorasPositivas = null;
        calc.BancoHorasConv = null;
        calc.HorasPositivasConv = null;
        calc.HorasNegativasConv = null;
        calc.HorasExtrasConv = null;
    }

    private static void ApplyNightAdditional(DayCalculation calc, TimeEntries entries, CalculationContext context)
    {
        var notEnt1 = CalculateNightEntry(entries.Entrada1, context.NightEnd);
        var notSai1 = CalculateNightExit(entries.Saida1, context.NightStart);
        var notEnt2 = CalculateNightEntry(entries.Entrada2, context.NightEnd);
        var notSai2 = CalculateNightExit(entries.Saida2, context.NightStart);
        var notEnt3 = CalculateNightEntry(entries.Entrada3, context.NightEnd);
        var notSai3 = CalculateNightExit(entries.Saida3, context.NightStart);

        var horaNotTotal = notEnt1 + notSai1 + notEnt2 + notSai2 + notEnt3 + notSai3;
        if (horaNotTotal <= TimeSpan.Zero)
            return;

        calc.HorasNoturnas = horaNotTotal;
        calc.HorasNoturnasConv = NightConvert(horaNotTotal);
    }

    private static void FinalizeDsrFeriadoConversion(DayCalculation calc)
    {
        calc.HorasDsrFeriadoConv = ToHours(calc.HorasDsrFeriado);
        if (calc.HorasDsrFeriadoConv == 0)
        {
            calc.HorasDsrFeriado = null;
            calc.HorasDsrFeriadoConv = null;
        }
    }

    private static void ApplyTotals(TimeCardTotals totals, DayCalculation calc)
    {
        totals.BancoHorasConvTotal += calc.BancoHorasConv ?? 0;
        totals.BancoHorasTotal += calc.BancoHoras ?? TimeSpan.Zero;

        totals.ApuracaoConvTotal += calc.ApuracaoConv ?? 0;
        totals.ApuracaoTotal += calc.Apuracao ?? TimeSpan.Zero;

        totals.HorasPositivasTotal += calc.HorasPositivas ?? TimeSpan.Zero;
        totals.HorasPositivasConvTotal += calc.HorasPositivasConv ?? 0;

        totals.HorasNegativasTotal += calc.HorasNegativas ?? TimeSpan.Zero;
        totals.HorasNegativasConvTotal += calc.HorasNegativasConv ?? 0;

        totals.HorasNoturnasTotal += calc.HorasNoturnas ?? TimeSpan.Zero;
        totals.HorasNoturnasConvTotal += calc.HorasNoturnasConv ?? 0;

        totals.HorasExtrasTotal += calc.HorasExtras ?? TimeSpan.Zero;
        totals.HorasExtrasConvTotal += calc.HorasExtrasConv ?? 0;

        totals.HorasDsrFeriadoTotal += calc.HorasDsrFeriado ?? TimeSpan.Zero;
        totals.HorasDsrFeriadoConvTotal += calc.HorasDsrFeriadoConv ?? 0;

        if (calc.FechamentoSemanaPositivo.HasValue)
            totals.FechamentoSemanaPositivoTotal += calc.FechamentoSemanaPositivo.Value;
        if (calc.FechamentoSemanaNegativo.HasValue)
            totals.FechamentoSemanaNegativoTotal += calc.FechamentoSemanaNegativo.Value;
    }

    private static double? ToHours(TimeSpan? value)
        => value.HasValue ? value.Value.TotalHours : null;

    private static double? NightConvert(TimeSpan value)
        => value.TotalHours * (NightFactor / 24.0);

    private static TimeSpan CalculateNightEntry(TimeSpan entry, TimeSpan nightEnd)
        => entry < nightEnd && entry != TimeSpan.Zero
            ? nightEnd - entry
            : TimeSpan.Zero;

    private static TimeSpan CalculateNightExit(TimeSpan exit, TimeSpan nightStart)
        => exit > nightStart
            ? exit - nightStart
            : TimeSpan.Zero;

    private static bool ShouldConvertNegativeHours(DayEntry day)
        => !day.IsSunday ^ (day.HolidayType == HolidayType.Dsr);

    private sealed class CalculationState
    {
        public TimeSpan TotalHoraSemana { get; set; }
    }

    private readonly record struct TimeEntries(
        TimeSpan Entrada1,
        TimeSpan Saida1,
        TimeSpan Entrada2,
        TimeSpan Saida2,
        TimeSpan Entrada3,
        TimeSpan Saida3);

    private readonly record struct CalculationContext(
        TimeSpan HoraDia,
        TimeSpan HoraFerias,
        TimeSpan Y13,
        TimeSpan Tolerancia,
        TimeSpan ToleranciaNegativa,
        TimeSpan NightStart,
        TimeSpan NightEnd,
        TimeSpan? WeeklyHours,
        TimeSpan? SaturdayHours,
        bool SaturdayCountsAsBank)
    {
        public static CalculationContext FromSchedule(ScheduleConfig schedule)
        {
            var tolerancia = (schedule.ToleranceEntry ?? TimeSpan.Zero) + (schedule.ToleranceExit ?? TimeSpan.Zero);
            var y13 = schedule.UseDailyHoursAsY13 || schedule.DailyLimit == null
                ? schedule.DailyHours
                : schedule.DailyLimit.Value;

            return new CalculationContext(
                schedule.DailyHours,
                TimeSpan.FromHours(8),
                y13,
                tolerancia,
                -tolerancia,
                schedule.NightStart ?? TimeSpan.FromHours(22),
                schedule.NightEnd ?? TimeSpan.FromHours(5),
                schedule.WeeklyHours,
                schedule.SaturdayHours,
                schedule.SaturdayCountsAsBank);
        }
    }
}

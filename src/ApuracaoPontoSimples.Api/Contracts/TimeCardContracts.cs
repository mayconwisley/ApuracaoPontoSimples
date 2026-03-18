using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Domain.Enums;
using ApuracaoPontoSimples.Domain.ValueObjects;

namespace ApuracaoPontoSimples.Api.Contracts;

public sealed record DayEntryRequest(
    DateOnly Date,
    DayCode Code,
    TimeSpan? Entrada1,
    TimeSpan? Saida1,
    TimeSpan? Entrada2,
    TimeSpan? Saida2,
    TimeSpan? Entrada3,
    TimeSpan? Saida3,
    HolidayType HolidayType,
    bool IsSunday,
    bool IsSaturday,
    TimeSpan? AbsenceHours,
    AbsenceType AbsenceType);

public sealed record CreateTimeCardRequest(Guid EmployeeId, DateOnly StartDate, DateOnly EndDate, List<DayEntryRequest> Days);

public sealed record TimeIntervalDto(TimeSpan? Start, TimeSpan? End);
public sealed record AbsenceDto(AbsenceType Type, TimeSpan? Hours);
public sealed record DayCalculationDto(
    TimeSpan? Apuracao,
    TimeSpan? BancoHoras,
    TimeSpan? HorasPositivas,
    TimeSpan? HorasNegativas,
    TimeSpan? HorasExtras,
    TimeSpan? HorasDsrFeriado,
    TimeSpan? HorasNoturnas,
    double? BancoHorasConv,
    double? ApuracaoConv,
    double? HorasPositivasConv,
    double? HorasNegativasConv,
    double? HorasExtrasConv,
    double? HorasDsrFeriadoConv,
    double? HorasNoturnasConv,
    TimeSpan? AcumuladoSemana,
    TimeSpan? FechamentoSemanaPositivo,
    TimeSpan? FechamentoSemanaNegativo);

public sealed record DayEntryDto(
    DateOnly Date,
    DayCode Code,
    TimeIntervalDto Interval1,
    TimeIntervalDto Interval2,
    TimeIntervalDto Interval3,
    HolidayType HolidayType,
    bool IsSunday,
    bool IsSaturday,
    AbsenceDto? Absence,
    DayCalculationDto? Calculation);

public sealed record TimeCardTotalsDto(
    double BancoHorasConvTotal,
    TimeSpan BancoHorasTotal,
    double ApuracaoConvTotal,
    TimeSpan ApuracaoTotal,
    TimeSpan HorasPositivasTotal,
    double HorasPositivasConvTotal,
    TimeSpan HorasNegativasTotal,
    double HorasNegativasConvTotal,
    TimeSpan HorasNoturnasTotal,
    double HorasNoturnasConvTotal,
    TimeSpan HorasExtrasTotal,
    double HorasExtrasConvTotal,
    TimeSpan HorasDsrFeriadoTotal,
    double HorasDsrFeriadoConvTotal,
    TimeSpan FechamentoSemanaPositivoTotal,
    TimeSpan FechamentoSemanaNegativoTotal);

public sealed record TimeCardDto(
    Guid Id,
    Guid EmployeeId,
    DateOnly? StartDate,
    DateOnly? EndDate,
    List<DayEntryDto> Days,
    TimeCardTotalsDto? Totals);

public static class TimeCardMappings
{
    public static TimeCardDto ToDto(this TimeCard timeCard)
        => new(
            timeCard.Id,
            timeCard.EmployeeId,
            timeCard.StartDate,
            timeCard.EndDate,
            timeCard.Days.Select(ToDto).ToList(),
            timeCard.Totals == null ? null : ToDto(timeCard.Totals));

    public static DayEntryDto ToDto(this DayEntry day)
        => new(
            day.Date,
            day.Code,
            ToDto(day.Interval1),
            ToDto(day.Interval2),
            ToDto(day.Interval3),
            day.HolidayType,
            day.IsSunday,
            day.IsSaturday,
            day.Absence == null ? null : ToDto(day.Absence),
            day.Calculation == null ? null : ToDto(day.Calculation));

    public static TimeIntervalDto ToDto(this TimeInterval interval)
        => new(interval.Start, interval.End);

    public static AbsenceDto ToDto(this Absence absence)
        => new(absence.Type, absence.Hours);

    public static DayCalculationDto ToDto(this DayCalculation calculation)
        => new(
            calculation.Apuracao,
            calculation.BancoHoras,
            calculation.HorasPositivas,
            calculation.HorasNegativas,
            calculation.HorasExtras,
            calculation.HorasDsrFeriado,
            calculation.HorasNoturnas,
            calculation.BancoHorasConv,
            calculation.ApuracaoConv,
            calculation.HorasPositivasConv,
            calculation.HorasNegativasConv,
            calculation.HorasExtrasConv,
            calculation.HorasDsrFeriadoConv,
            calculation.HorasNoturnasConv,
            calculation.AcumuladoSemana,
            calculation.FechamentoSemanaPositivo,
            calculation.FechamentoSemanaNegativo);

    public static TimeCardTotalsDto ToDto(this TimeCardTotals totals)
        => new(
            totals.BancoHorasConvTotal,
            totals.BancoHorasTotal,
            totals.ApuracaoConvTotal,
            totals.ApuracaoTotal,
            totals.HorasPositivasTotal,
            totals.HorasPositivasConvTotal,
            totals.HorasNegativasTotal,
            totals.HorasNegativasConvTotal,
            totals.HorasNoturnasTotal,
            totals.HorasNoturnasConvTotal,
            totals.HorasExtrasTotal,
            totals.HorasExtrasConvTotal,
            totals.HorasDsrFeriadoTotal,
            totals.HorasDsrFeriadoConvTotal,
            totals.FechamentoSemanaPositivoTotal,
            totals.FechamentoSemanaNegativoTotal);
}

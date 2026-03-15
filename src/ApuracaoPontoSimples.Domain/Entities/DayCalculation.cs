namespace ApuracaoPontoSimples.Domain.Entities;

public sealed class DayCalculation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DayEntryId { get; set; }
    public TimeSpan? Apuracao { get; set; }
    public TimeSpan? BancoHoras { get; set; }
    public TimeSpan? HorasPositivas { get; set; }
    public TimeSpan? HorasNegativas { get; set; }
    public TimeSpan? HorasExtras { get; set; }
    public TimeSpan? HorasDsrFeriado { get; set; }
    public TimeSpan? HorasNoturnas { get; set; }

    public double? BancoHorasConv { get; set; }
    public double? ApuracaoConv { get; set; }
    public double? HorasPositivasConv { get; set; }
    public double? HorasNegativasConv { get; set; }
    public double? HorasExtrasConv { get; set; }
    public double? HorasDsrFeriadoConv { get; set; }
    public double? HorasNoturnasConv { get; set; }

    public TimeSpan? AcumuladoSemana { get; set; }
    public TimeSpan? FechamentoSemanaPositivo { get; set; }
    public TimeSpan? FechamentoSemanaNegativo { get; set; }
}

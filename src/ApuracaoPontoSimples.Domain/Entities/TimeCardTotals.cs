namespace ApuracaoPontoSimples.Domain.Entities;

public sealed class TimeCardTotals
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TimeCardId { get; set; }
    public double BancoHorasConvTotal { get; set; }
    public TimeSpan BancoHorasTotal { get; set; }

    public double ApuracaoConvTotal { get; set; }
    public TimeSpan ApuracaoTotal { get; set; }

    public TimeSpan HorasPositivasTotal { get; set; }
    public double HorasPositivasConvTotal { get; set; }

    public TimeSpan HorasNegativasTotal { get; set; }
    public double HorasNegativasConvTotal { get; set; }

    public TimeSpan HorasNoturnasTotal { get; set; }
    public double HorasNoturnasConvTotal { get; set; }

    public TimeSpan HorasExtrasTotal { get; set; }
    public double HorasExtrasConvTotal { get; set; }

    public TimeSpan HorasDsrFeriadoTotal { get; set; }
    public double HorasDsrFeriadoConvTotal { get; set; }

    public TimeSpan FechamentoSemanaPositivoTotal { get; set; }
    public TimeSpan FechamentoSemanaNegativoTotal { get; set; }
}

using ApuracaoPontoSimples.Domain.Enums;

namespace ApuracaoPontoSimples.Domain.Entities;

public sealed class Absence
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DayEntryId { get; set; }
    public AbsenceType Type { get; set; }
    public TimeSpan? Hours { get; set; }
}

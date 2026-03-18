using ApuracaoPontoSimples.Domain.Enums;
using ApuracaoPontoSimples.Domain.ValueObjects;

namespace ApuracaoPontoSimples.Domain.Entities;

public sealed class DayEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TimeCardId { get; set; }
    public DateOnly Date { get; set; }
    public DayCode Code { get; set; } = DayCode.None;

    public TimeInterval Interval1 { get; set; } = new(null, null);
    public TimeInterval Interval2 { get; set; } = new(null, null);
    public TimeInterval Interval3 { get; set; } = new(null, null);

    public HolidayType HolidayType { get; set; } = HolidayType.None;
    public bool IsSunday { get; set; }
    public bool IsSaturday { get; set; }

    public Absence? Absence { get; set; }
    public DayCalculation? Calculation { get; set; }
}

namespace ApuracaoPontoSimples.Domain.Entities;

public sealed class TimeCard
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public List<DayEntry> Days { get; set; } = new();
    public TimeCardTotals? Totals { get; set; }
}

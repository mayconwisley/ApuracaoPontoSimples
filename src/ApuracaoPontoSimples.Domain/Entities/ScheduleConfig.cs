namespace ApuracaoPontoSimples.Domain.Entities;

public sealed class ScheduleConfig
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EmployeeId { get; set; }
    public TimeSpan DailyHours { get; set; }
    public TimeSpan? DailyLimit { get; set; }
    public TimeSpan? SaturdayHours { get; set; }
    public TimeSpan? ToleranceEntry { get; set; }
    public TimeSpan? ToleranceExit { get; set; }
    public TimeSpan? NightStart { get; set; }
    public TimeSpan? NightEnd { get; set; }
    public TimeSpan? WeeklyHours { get; set; }
    public bool SaturdayCountsAsBank { get; set; } = true; // AD1 = 1
    public bool UseDailyHoursAsY13 { get; set; } = true;   // AB1 = Verdadeiro
}

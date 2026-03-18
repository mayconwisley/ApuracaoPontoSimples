namespace ApuracaoPontoSimples.Application.Models;

public sealed record ScheduleConfigInput(
    TimeSpan DailyHours,
    TimeSpan? DailyLimit,
    TimeSpan? SaturdayHours,
    TimeSpan? ToleranceEntry,
    TimeSpan? ToleranceExit,
    TimeSpan? NightStart,
    TimeSpan? NightEnd,
    TimeSpan? WeeklyHours,
    bool SaturdayCountsAsBank,
    bool UseDailyHoursAsY13);

public sealed record CreateEmployeeInput(
    string Name,
    string? Pis,
    DateOnly? AdmissionDate,
    Guid EmployerId,
    ScheduleConfigInput? Schedule);

public sealed record UpdateEmployeeInput(
    string Name,
    string? Pis,
    DateOnly? AdmissionDate,
    Guid EmployerId,
    ScheduleConfigInput? Schedule);

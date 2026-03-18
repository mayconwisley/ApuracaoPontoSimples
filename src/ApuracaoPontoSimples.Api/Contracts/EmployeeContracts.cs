using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Api.Contracts;

public sealed record EmployerDto(Guid Id, string Name, string? Cnpj, string? Address);
public sealed record CreateEmployerRequest(string Name, string? Cnpj, string? Address);
public sealed record EmployeeDto(Guid Id, string Name, string? Pis, DateOnly? AdmissionDate, Guid EmployerId, EmployerDto? Employer);
public sealed record EmployeeDetailsDto(Guid Id, string Name, string? Pis, DateOnly? AdmissionDate, Guid EmployerId, ScheduleConfigDto? Schedule);
public sealed record ScheduleConfigDto(TimeSpan DailyHours, TimeSpan? DailyLimit, TimeSpan? SaturdayHours, TimeSpan? ToleranceEntry, TimeSpan? ToleranceExit, TimeSpan? NightStart, TimeSpan? NightEnd, TimeSpan? WeeklyHours, bool SaturdayCountsAsBank, bool UseDailyHoursAsY13);
public sealed record CreateEmployeeRequest(string Name, string? Pis, DateOnly? AdmissionDate, Guid EmployerId, ScheduleConfigDto Schedule);
public sealed record UpdateEmployeeRequest(string Name, string? Pis, DateOnly? AdmissionDate, Guid EmployerId, ScheduleConfigDto Schedule);

public static class EmployeeMappings
{
    public static EmployerDto ToDto(this Employer employer)
        => new(employer.Id, employer.Name, employer.Cnpj, employer.Address);

    public static EmployeeDto ToDto(this Employee employee)
        => new(employee.Id, employee.Name, employee.Pis, employee.AdmissionDate, employee.EmployerId, employee.Employer?.ToDto());

    public static EmployeeDetailsDto ToDetailsDto(this Employee employee)
        => new(
            employee.Id,
            employee.Name,
            employee.Pis,
            employee.AdmissionDate,
            employee.EmployerId,
            employee.Schedule == null ? null : employee.Schedule.ToDto());

    public static ScheduleConfig ToEntity(this ScheduleConfigDto dto)
        => new()
        {
            DailyHours = dto.DailyHours,
            DailyLimit = dto.DailyLimit,
            SaturdayHours = dto.SaturdayHours,
            ToleranceEntry = dto.ToleranceEntry,
            ToleranceExit = dto.ToleranceExit,
            NightStart = dto.NightStart,
            NightEnd = dto.NightEnd,
            WeeklyHours = dto.WeeklyHours,
            SaturdayCountsAsBank = dto.SaturdayCountsAsBank,
            UseDailyHoursAsY13 = dto.UseDailyHoursAsY13
        };

    public static ScheduleConfigDto ToDto(this ScheduleConfig entity)
        => new(
            entity.DailyHours,
            entity.DailyLimit,
            entity.SaturdayHours,
            entity.ToleranceEntry,
            entity.ToleranceExit,
            entity.NightStart,
            entity.NightEnd,
            entity.WeeklyHours,
            entity.SaturdayCountsAsBank,
            entity.UseDailyHoursAsY13);

    public static ScheduleConfigInput ToInput(this ScheduleConfigDto dto)
        => new(
            dto.DailyHours,
            dto.DailyLimit,
            dto.SaturdayHours,
            dto.ToleranceEntry,
            dto.ToleranceExit,
            dto.NightStart,
            dto.NightEnd,
            dto.WeeklyHours,
            dto.SaturdayCountsAsBank,
            dto.UseDailyHoursAsY13);

    public static void UpdateFrom(this ScheduleConfig entity, ScheduleConfigDto dto)
    {
        entity.DailyHours = dto.DailyHours;
        entity.DailyLimit = dto.DailyLimit;
        entity.SaturdayHours = dto.SaturdayHours;
        entity.ToleranceEntry = dto.ToleranceEntry;
        entity.ToleranceExit = dto.ToleranceExit;
        entity.NightStart = dto.NightStart;
        entity.NightEnd = dto.NightEnd;
        entity.WeeklyHours = dto.WeeklyHours;
        entity.SaturdayCountsAsBank = dto.SaturdayCountsAsBank;
        entity.UseDailyHoursAsY13 = dto.UseDailyHoursAsY13;
    }
}

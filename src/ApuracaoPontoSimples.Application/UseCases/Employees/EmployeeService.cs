using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Domain.Entities;

namespace ApuracaoPontoSimples.Application.UseCases.Employees;

public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employees;
    private readonly IEmployerRepository _employers;
    private readonly IUnitOfWork _unitOfWork;

    public EmployeeService(
        IEmployeeRepository employees,
        IEmployerRepository employers,
        IUnitOfWork unitOfWork)
    {
        _employees = employees;
        _employers = employers;
        _unitOfWork = unitOfWork;
    }

    public Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken)
        => _employees.GetAllWithEmployerAsync(cancellationToken);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        => _employees.GetByIdWithScheduleReadOnlyAsync(id, cancellationToken);

    public async Task<ServiceResult<Employee>> CreateAsync(CreateEmployeeInput input, CancellationToken cancellationToken)
    {
        var employerExists = await _employers.ExistsAsync(input.EmployerId, cancellationToken);
        if (!employerExists)
            return ServiceResult<Employee>.Fail(ServiceErrorType.Validation, "Employer not found. Create an employer first.");

        var employee = new Employee
        {
            Name = input.Name,
            Pis = input.Pis,
            AdmissionDate = input.AdmissionDate,
            EmployerId = input.EmployerId,
            Schedule = input.Schedule == null ? null : ToEntity(input.Schedule)
        };

        if (employee.Schedule != null)
            employee.Schedule.EmployeeId = employee.Id;

        _employees.Add(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ServiceResult<Employee>.Ok(employee);
    }

    public async Task<ServiceResult<Employee>> UpdateAsync(Guid id, UpdateEmployeeInput input, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdWithScheduleAsync(id, cancellationToken);
        if (employee == null)
            return ServiceResult<Employee>.Fail(ServiceErrorType.NotFound, "Employee not found.");

        var employerExists = await _employers.ExistsAsync(input.EmployerId, cancellationToken);
        if (!employerExists)
            return ServiceResult<Employee>.Fail(ServiceErrorType.Validation, "Employer not found. Create an employer first.");

        employee.Name = input.Name;
        employee.Pis = input.Pis;
        employee.AdmissionDate = input.AdmissionDate;
        employee.EmployerId = input.EmployerId;

        if (input.Schedule == null)
        {
            employee.Schedule = null;
        }
        else if (employee.Schedule == null)
        {
            employee.Schedule = ToEntity(input.Schedule);
            employee.Schedule.EmployeeId = employee.Id;
        }
        else
        {
            UpdateSchedule(employee.Schedule, input.Schedule);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult<Employee>.Ok(employee);
    }

    public async Task<ServiceResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee == null)
            return ServiceResult.Fail(ServiceErrorType.NotFound, "Employee not found.");

        _employees.Remove(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ServiceResult.Ok();
    }

    private static ScheduleConfig ToEntity(ScheduleConfigInput input)
        => new()
        {
            DailyHours = input.DailyHours,
            DailyLimit = input.DailyLimit,
            SaturdayHours = input.SaturdayHours,
            ToleranceEntry = input.ToleranceEntry,
            ToleranceExit = input.ToleranceExit,
            NightStart = input.NightStart,
            NightEnd = input.NightEnd,
            WeeklyHours = input.WeeklyHours,
            SaturdayCountsAsBank = input.SaturdayCountsAsBank,
            UseDailyHoursAsY13 = input.UseDailyHoursAsY13
        };

    private static void UpdateSchedule(ScheduleConfig entity, ScheduleConfigInput input)
    {
        entity.DailyHours = input.DailyHours;
        entity.DailyLimit = input.DailyLimit;
        entity.SaturdayHours = input.SaturdayHours;
        entity.ToleranceEntry = input.ToleranceEntry;
        entity.ToleranceExit = input.ToleranceExit;
        entity.NightStart = input.NightStart;
        entity.NightEnd = input.NightEnd;
        entity.WeeklyHours = input.WeeklyHours;
        entity.SaturdayCountsAsBank = input.SaturdayCountsAsBank;
        entity.UseDailyHoursAsY13 = input.UseDailyHoursAsY13;
    }
}

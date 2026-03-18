using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Domain.Enums;
using ApuracaoPontoSimples.Domain.ValueObjects;

namespace ApuracaoPontoSimples.Application.UseCases.TimeCards;

public sealed class TimeCardService : ITimeCardService
{
    private readonly IEmployeeRepository _employees;
    private readonly ITimeCardRepository _timeCards;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITimeCardCalculator _calculator;

    public TimeCardService(
        IEmployeeRepository employees,
        ITimeCardRepository timeCards,
        IUnitOfWork unitOfWork,
        ITimeCardCalculator calculator)
    {
        _employees = employees;
        _timeCards = timeCards;
        _unitOfWork = unitOfWork;
        _calculator = calculator;
    }

    public async Task<ServiceResult<TimeCard>> CreateAsync(CreateTimeCardInput input, CancellationToken cancellationToken)
    {
        if (input.StartDate > input.EndDate)
            return ServiceResult<TimeCard>.Fail(ServiceErrorType.Validation, "StartDate must be less than or equal to EndDate.");

        if (input.Days.Any(d => d.Date < input.StartDate || d.Date > input.EndDate))
            return ServiceResult<TimeCard>.Fail(ServiceErrorType.Validation, "All day entries must be within StartDate and EndDate.");

        var employee = await _employees.GetByIdWithScheduleAsync(input.EmployeeId, cancellationToken);
        if (employee == null)
            return ServiceResult<TimeCard>.Fail(ServiceErrorType.Validation, "Employee not found.");
        if (employee.Schedule == null)
            return ServiceResult<TimeCard>.Fail(ServiceErrorType.Validation, "Employee schedule not found.");

        await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var overlappingTimeCards = await _timeCards.GetOverlappingAsync(
                input.EmployeeId,
                input.StartDate,
                input.EndDate,
                cancellationToken);

            var mergedStartDate = input.StartDate;
            var mergedEndDate = input.EndDate;
            var dayByDate = new Dictionary<DateOnly, DayEntry>();

            foreach (var card in overlappingTimeCards)
            {
                if (card.StartDate.HasValue && card.StartDate.Value < mergedStartDate)
                    mergedStartDate = card.StartDate.Value;
                if (card.EndDate.HasValue && card.EndDate.Value > mergedEndDate)
                    mergedEndDate = card.EndDate.Value;

                foreach (var day in card.Days)
                {
                    dayByDate[day.Date] = CloneDay(day);
                }
            }

            foreach (var day in input.Days)
            {
                dayByDate[day.Date] = new DayEntry
                {
                    Date = day.Date,
                    Code = day.Code,
                    Interval1 = new TimeInterval(day.Entrada1, day.Saida1),
                    Interval2 = new TimeInterval(day.Entrada2, day.Saida2),
                    Interval3 = new TimeInterval(day.Entrada3, day.Saida3),
                    HolidayType = day.HolidayType,
                    IsSunday = day.IsSunday,
                    IsSaturday = day.IsSaturday,
                    Absence = day.AbsenceType != AbsenceType.None
                        ? new Absence { Type = day.AbsenceType, Hours = day.AbsenceHours }
                        : null
                };
            }

            if (overlappingTimeCards.Count > 0)
            {
                _timeCards.RemoveRange(overlappingTimeCards);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var timeCard = new TimeCard
            {
                EmployeeId = input.EmployeeId,
                StartDate = mergedStartDate,
                EndDate = mergedEndDate,
                Days = dayByDate
                    .Where(item => item.Key >= mergedStartDate && item.Key <= mergedEndDate)
                    .OrderBy(item => item.Key)
                    .Select(item => item.Value)
                    .ToList()
            };

            _calculator.Calculate(timeCard, employee.Schedule);

            await _timeCards.AddAsync(timeCard, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return ServiceResult<TimeCard>.Ok(timeCard);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<ServiceResult<TimeCard>> GetAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        if (startDate > endDate)
            return ServiceResult<TimeCard>.Fail(ServiceErrorType.Validation, "startDate must be less than or equal to endDate.");

        var employee = await _employees.GetByIdWithScheduleReadOnlyAsync(employeeId, cancellationToken);
        if (employee == null)
            return ServiceResult<TimeCard>.Fail(ServiceErrorType.NotFound, "Employee not found.");
        if (employee.Schedule == null)
            return ServiceResult<TimeCard>.Fail(ServiceErrorType.Validation, "Employee schedule not found.");

        var overlappingTimeCards = await _timeCards.GetOverlappingAsync(employeeId, startDate, endDate, cancellationToken);
        if (overlappingTimeCards.Count == 0)
            return ServiceResult<TimeCard>.Fail(ServiceErrorType.NotFound, "Time card not found.");

        var dayByDate = new Dictionary<DateOnly, DayEntry>();
        foreach (var card in overlappingTimeCards)
        {
            foreach (var day in card.Days.Where(d => d.Date >= startDate && d.Date <= endDate))
            {
                dayByDate[day.Date] = CloneDay(day);
            }
        }

        var timeCard = new TimeCard
        {
            EmployeeId = employeeId,
            StartDate = startDate,
            EndDate = endDate,
            Days = dayByDate
                .OrderBy(item => item.Key)
                .Select(item => item.Value)
                .ToList()
        };

        _calculator.Calculate(timeCard, employee.Schedule);
        return ServiceResult<TimeCard>.Ok(timeCard);
    }

    private static DayEntry CloneDay(DayEntry source)
        => new()
        {
            Date = source.Date,
            Code = source.Code,
            Interval1 = new TimeInterval(source.Interval1.Start, source.Interval1.End),
            Interval2 = new TimeInterval(source.Interval2.Start, source.Interval2.End),
            Interval3 = new TimeInterval(source.Interval3.Start, source.Interval3.End),
            HolidayType = source.HolidayType,
            IsSunday = source.IsSunday,
            IsSaturday = source.IsSaturday,
            Absence = source.Absence == null
                ? null
                : new Absence
                {
                    Type = source.Absence.Type,
                    Hours = source.Absence.Hours
                }
        };
}

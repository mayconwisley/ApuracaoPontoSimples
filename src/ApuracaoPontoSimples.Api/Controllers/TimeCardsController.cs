using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Domain.Enums;
using ApuracaoPontoSimples.Domain.ValueObjects;
using ApuracaoPontoSimples.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/timecards")]
[Authorize]
public sealed class TimeCardsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITimeCardCalculator _calculator;

    public TimeCardsController(AppDbContext db, ITimeCardCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    [HttpPost]
    public async Task<ActionResult<TimeCard>> Create(CreateTimeCardRequest request)
    {
        var employee = await _db.Employees.Include(e => e.Schedule).FirstOrDefaultAsync(e => e.Id == request.EmployeeId);
        if (employee == null) return BadRequest("Employee not found.");
        if (employee.Schedule == null) return BadRequest("Employee schedule not found.");
        if (request.StartDate > request.EndDate) return BadRequest("StartDate must be less than or equal to EndDate.");
        if (request.Days.Any(d => d.Date < request.StartDate || d.Date > request.EndDate))
            return BadRequest("All day entries must be within StartDate and EndDate.");

        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var overlappingTimeCards = await _db.TimeCards
                .Include(t => t.Days)
                .ThenInclude(d => d.Absence)
                .Where(t =>
                    t.EmployeeId == request.EmployeeId &&
                    t.StartDate <= request.EndDate &&
                    t.EndDate >= request.StartDate)
                .ToListAsync();

            var mergedStartDate = request.StartDate;
            var mergedEndDate = request.EndDate;
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

            foreach (var day in request.Days)
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
                _db.TimeCards.RemoveRange(overlappingTimeCards);
                await _db.SaveChangesAsync();
            }

            var timeCard = new TimeCard
            {
                EmployeeId = request.EmployeeId,
                StartDate = mergedStartDate,
                EndDate = mergedEndDate,
                Days = dayByDate
                    .Where(item => item.Key >= mergedStartDate && item.Key <= mergedEndDate)
                    .OrderBy(item => item.Key)
                    .Select(item => item.Value)
                    .ToList()
            };

            _calculator.Calculate(timeCard, employee.Schedule);

            _db.TimeCards.Add(timeCard);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(timeCard);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    [HttpGet("{employeeId:guid}/{startDate}/{endDate}")]
    public async Task<ActionResult<TimeCard>> Get(Guid employeeId, DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate) return BadRequest("startDate must be less than or equal to endDate.");

        var employee = await _db.Employees
            .Include(e => e.Schedule)
            .FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null) return NotFound();
        if (employee.Schedule == null) return BadRequest("Employee schedule not found.");

        var overlappingTimeCards = await _db.TimeCards
            .Include(t => t.Days)
            .ThenInclude(d => d.Absence)
            .Where(t =>
                t.EmployeeId == employeeId &&
                t.StartDate <= endDate &&
                t.EndDate >= startDate)
            .ToListAsync();

        if (overlappingTimeCards.Count == 0) return NotFound();

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
        return Ok(timeCard);
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

using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/timecards")]
[Authorize]
public sealed class TimeCardsController : ControllerBase
{
    private readonly ITimeCardService _timeCards;

    public TimeCardsController(ITimeCardService timeCards)
    {
        _timeCards = timeCards;
    }

    [HttpPost]
    public async Task<ActionResult<TimeCardDto>> Create(CreateTimeCardRequest request, CancellationToken cancellationToken)
    {
        var input = new CreateTimeCardInput(
            request.EmployeeId,
            request.StartDate,
            request.EndDate,
            request.Days.Select(d => new DayEntryInput(
                d.Date,
                d.Code,
                d.Entrada1,
                d.Saida1,
                d.Entrada2,
                d.Saida2,
                d.Entrada3,
                d.Saida3,
                d.HolidayType,
                d.IsSunday,
                d.IsSaturday,
                d.AbsenceHours,
                d.AbsenceType)).ToList());

        var result = await _timeCards.CreateAsync(input, cancellationToken);
        if (!result.Success)
            return result.ErrorType == ServiceErrorType.NotFound ? NotFound(result.ErrorMessage) : BadRequest(result.ErrorMessage);

        return Ok(result.Value!.ToDto());
    }

    [HttpGet("{employeeId:guid}/{startDate}/{endDate}")]
    public async Task<ActionResult<TimeCardDto>> Get(Guid employeeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var result = await _timeCards.GetAsync(employeeId, startDate, endDate, cancellationToken);
        if (!result.Success)
            return result.ErrorType == ServiceErrorType.NotFound ? NotFound(result.ErrorMessage) : BadRequest(result.ErrorMessage);

        return Ok(result.Value!.ToDto());
    }
}

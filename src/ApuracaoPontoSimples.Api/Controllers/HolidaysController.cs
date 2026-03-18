using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/holidays")]
[Authorize]
public sealed class HolidaysController : ControllerBase
{
    private readonly IHolidayService _holidays;

    public HolidaysController(IHolidayService holidays)
    {
        _holidays = holidays;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<HolidayDto>>> GetAll(CancellationToken cancellationToken)
    {
        var holidays = await _holidays.GetAllAsync(cancellationToken);
        return Ok(holidays.Select(h => h.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<HolidayDto>> Create(HolidayRequest request, CancellationToken cancellationToken)
    {
        var input = new HolidayInput(request.Date, request.Description);
        var result = await _holidays.CreateAsync(input, cancellationToken);
        return Ok(result.Value!.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<HolidayDto>> Update(Guid id, HolidayRequest request, CancellationToken cancellationToken)
    {
        var input = new HolidayInput(request.Date, request.Description);
        var result = await _holidays.UpdateAsync(id, input, cancellationToken);
        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Value!.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _holidays.DeleteAsync(id, cancellationToken);
        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return NoContent();
    }
}

using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/holidays")]
[Authorize]
public sealed class HolidaysController : ControllerBase
{
    private readonly AppDbContext _db;

    public HolidaysController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Holiday>>> GetAll()
    {
        var holidays = await _db.Holidays.AsNoTracking().ToListAsync();
        return Ok(holidays);
    }

    [HttpPost]
    public async Task<ActionResult<Holiday>> Create(HolidayRequest request)
    {
        var holiday = new Holiday { Date = request.Date, Description = request.Description };
        _db.Holidays.Add(holiday);
        await _db.SaveChangesAsync();
        return Ok(holiday);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Holiday>> Update(Guid id, HolidayRequest request)
    {
        var holiday = await _db.Holidays.FirstOrDefaultAsync(h => h.Id == id);
        if (holiday == null)
            return NotFound();

        holiday.Date = request.Date;
        holiday.Description = request.Description;

        await _db.SaveChangesAsync();
        return Ok(holiday);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var holiday = await _db.Holidays.FirstOrDefaultAsync(h => h.Id == id);
        if (holiday == null)
            return NotFound();

        _db.Holidays.Remove(holiday);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

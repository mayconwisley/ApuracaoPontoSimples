using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/employers")]
[Authorize]
public sealed class EmployersController : ControllerBase
{
    private readonly AppDbContext _db;

    public EmployersController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployerDto>>> GetAll()
    {
        var employers = await _db.Employers.AsNoTracking().ToListAsync();
        return Ok(employers.Select(e => e.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<EmployerDto>> Create(CreateEmployerRequest request)
    {
        var employer = new Employer
        {
            Name = request.Name,
            Cnpj = request.Cnpj,
            Address = request.Address
        };

        _db.Employers.Add(employer);
        await _db.SaveChangesAsync();

        return Ok(employer.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployerDto>> Update(Guid id, CreateEmployerRequest request)
    {
        var employer = await _db.Employers.FirstOrDefaultAsync(e => e.Id == id);
        if (employer == null)
            return NotFound();

        employer.Name = request.Name;
        employer.Cnpj = request.Cnpj;
        employer.Address = request.Address;

        await _db.SaveChangesAsync();
        return Ok(employer.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var employer = await _db.Employers.FirstOrDefaultAsync(e => e.Id == id);
        if (employer == null)
            return NotFound();

        _db.Employers.Remove(employer);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Domain.Entities;
using ApuracaoPontoSimples.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize]
public sealed class EmployeesController : ControllerBase
{
    private readonly AppDbContext _db;

    public EmployeesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        var employees = await _db.Employees
        .Include(i => i.Employer)
        .AsNoTracking()
        .ToListAsync();
        return Ok(employees.Select(e => e.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeRequest request)
    {
        var employerExists = await _db.Employers.AnyAsync(e => e.Id == request.EmployerId);
        if (!employerExists)
            return BadRequest("Employer not found. Create an employer first.");

        var employee = new Employee
        {
            Name = request.Name,
            Pis = request.Pis,
            AdmissionDate = request.AdmissionDate,
            EmployerId = request.EmployerId,
            Schedule = request.Schedule.ToEntity()
        };
        if (employee.Schedule != null)
            employee.Schedule.EmployeeId = employee.Id;

        _db.Employees.Add(employee);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee.ToDto());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDetailsDto>> GetById(Guid id)
    {
        var employee = await _db.Employees
            .AsNoTracking()
            .Include(e => e.Schedule)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return NotFound();
        return Ok(employee.ToDetailsDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> Update(Guid id, UpdateEmployeeRequest request)
    {
        var employee = await _db.Employees.Include(e => e.Schedule).FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null)
            return NotFound();

        var employerExists = await _db.Employers.AnyAsync(e => e.Id == request.EmployerId);
        if (!employerExists)
            return BadRequest("Employer not found. Create an employer first.");

        employee.Name = request.Name;
        employee.Pis = request.Pis;
        employee.AdmissionDate = request.AdmissionDate;
        employee.EmployerId = request.EmployerId;

        if (employee.Schedule == null)
        {
            employee.Schedule = request.Schedule.ToEntity();
            employee.Schedule.EmployeeId = employee.Id;
        }
        else
        {
            employee.Schedule.UpdateFrom(request.Schedule);
        }

        await _db.SaveChangesAsync();
        return Ok(employee.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null)
            return NotFound();

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

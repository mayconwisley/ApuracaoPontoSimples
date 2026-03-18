using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/employees")]
[Authorize]
public sealed class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employees;

    public EmployeesController(IEmployeeService employees)
    {
        _employees = employees;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var employees = await _employees.GetAllAsync(cancellationToken);
        return Ok(employees.Select(e => e.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create(CreateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var input = new CreateEmployeeInput(
            request.Name,
            request.Pis,
            request.AdmissionDate,
            request.EmployerId,
            request.Schedule == null ? null : request.Schedule.ToInput());

        var result = await _employees.CreateAsync(input, cancellationToken);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value!.ToDto());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var employee = await _employees.GetByIdAsync(id, cancellationToken);
        if (employee == null) return NotFound();
        return Ok(employee.ToDetailsDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployeeDto>> Update(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var input = new UpdateEmployeeInput(
            request.Name,
            request.Pis,
            request.AdmissionDate,
            request.EmployerId,
            request.Schedule == null ? null : request.Schedule.ToInput());

        var result = await _employees.UpdateAsync(id, input, cancellationToken);
        if (!result.Success)
            return result.ErrorType == ServiceErrorType.NotFound
                ? NotFound(result.ErrorMessage)
                : BadRequest(result.ErrorMessage);

        return Ok(result.Value!.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _employees.DeleteAsync(id, cancellationToken);
        if (!result.Success && result.ErrorType == ServiceErrorType.NotFound)
            return NotFound(result.ErrorMessage);

        return NoContent();
    }
}

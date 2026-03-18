using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/employers")]
[Authorize]
public sealed class EmployersController : ControllerBase
{
    private readonly IEmployerService _employers;

    public EmployersController(IEmployerService employers)
    {
        _employers = employers;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployerDto>>> GetAll(CancellationToken cancellationToken)
    {
        var employers = await _employers.GetAllAsync(cancellationToken);
        return Ok(employers.Select(e => e.ToDto()));
    }

    [HttpPost]
    public async Task<ActionResult<EmployerDto>> Create(CreateEmployerRequest request, CancellationToken cancellationToken)
    {
        var input = new CreateEmployerInput(request.Name, request.Cnpj, request.Address);
        var result = await _employers.CreateAsync(input, cancellationToken);
        return Ok(result.Value!.ToDto());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployerDto>> Update(Guid id, CreateEmployerRequest request, CancellationToken cancellationToken)
    {
        var input = new UpdateEmployerInput(request.Name, request.Cnpj, request.Address);
        var result = await _employers.UpdateAsync(id, input, cancellationToken);
        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Value!.ToDto());
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _employers.DeleteAsync(id, cancellationToken);
        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return NoContent();
    }
}

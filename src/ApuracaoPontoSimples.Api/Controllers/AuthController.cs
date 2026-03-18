using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var input = new RegisterInput(request.Email, request.Password, request.FullName, request.Role);
        var result = await _authService.RegisterAsync(input, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var input = new LoginInput(request.Email, request.Password);
        var result = await _authService.LoginAsync(input, cancellationToken);
        return result.Success ? Ok(new AuthResponse(result.Value!)) : Unauthorized();
    }

    [HttpPost("bootstrap")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Bootstrap(BootstrapRequest request, CancellationToken cancellationToken)
    {
        var input = new BootstrapInput(request.Email, request.Password, request.FullName);
        var result = await _authService.BootstrapAsync(input, cancellationToken);
        return ToActionResult(result);
    }

    private ActionResult<AuthResponse> ToActionResult(ServiceResult<string> result)
    {
        if (result.Success)
            return Ok(new AuthResponse(result.Value!));

        return result.ErrorType switch
        {
            ServiceErrorType.Conflict => Conflict(result.ErrorMessage),
            ServiceErrorType.Validation => BadRequest(result.ErrorMessage),
            ServiceErrorType.NotFound => NotFound(result.ErrorMessage),
            _ => BadRequest(result.ErrorMessage)
        };
    }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApuracaoPontoSimples.Api.Contracts;
using ApuracaoPontoSimples.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ApuracaoPontoSimples.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (!await _roleManager.RoleExistsAsync(request.Role))
        {
            await _roleManager.CreateAsync(new ApplicationRole { Name = request.Role });
        }

        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, request.Role);

        return Ok(new AuthResponse(GenerateToken(user, new[] { request.Role })));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized();

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Unauthorized();

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(new AuthResponse(GenerateToken(user, roles)));
    }

    [HttpPost("bootstrap")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Bootstrap(BootstrapRequest request)
    {
        if (_userManager.Users.Any())
            return BadRequest("Bootstrap already executed.");

        const string adminRole = "Admin";
        if (!await _roleManager.RoleExistsAsync(adminRole))
        {
            await _roleManager.CreateAsync(new ApplicationRole { Name = adminRole });
        }

        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        await _userManager.AddToRoleAsync(user, adminRole);
        return Ok(new AuthResponse(GenerateToken(user, new[] { adminRole })));
    }

    private string GenerateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"] ?? ""));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(ClaimTypes.Name, user.UserName ?? string.Empty)
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

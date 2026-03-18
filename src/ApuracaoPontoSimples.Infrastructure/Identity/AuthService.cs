using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApuracaoPontoSimples.Infrastructure.Identity;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    public async Task<ServiceResult<string>> RegisterAsync(RegisterInput input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!await _roleManager.RoleExistsAsync(input.Role))
        {
            var roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = input.Role });
            if (!roleResult.Succeeded)
            {
                var error = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                return ServiceResult<string>.Fail(ServiceErrorType.Validation, error);
            }
        }

        var user = new ApplicationUser { UserName = input.Email, Email = input.Email, FullName = input.FullName };
        var result = await _userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
        {
            var error = string.Join("; ", result.Errors.Select(e => e.Description));
            return ServiceResult<string>.Fail(ServiceErrorType.Validation, error);
        }

        await _userManager.AddToRoleAsync(user, input.Role);
        var token = GenerateToken(user, new[] { input.Role });
        return ServiceResult<string>.Ok(token);
    }

    public async Task<ServiceResult<string>> LoginAsync(LoginInput input, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByEmailAsync(input.Email);
        if (user == null)
            return ServiceResult<string>.Fail(ServiceErrorType.Validation, "Invalid credentials.");

        if (!await _userManager.CheckPasswordAsync(user, input.Password))
            return ServiceResult<string>.Fail(ServiceErrorType.Validation, "Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var token = GenerateToken(user, roles);
        return ServiceResult<string>.Ok(token);
    }

    public async Task<ServiceResult<string>> BootstrapAsync(BootstrapInput input, CancellationToken cancellationToken)
    {
        var hasUsers = await _userManager.Users.AnyAsync(cancellationToken);
        if (hasUsers)
            return ServiceResult<string>.Fail(ServiceErrorType.Conflict, "Bootstrap already executed.");

        const string adminRole = "Admin";
        if (!await _roleManager.RoleExistsAsync(adminRole))
        {
            var roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = adminRole });
            if (!roleResult.Succeeded)
            {
                var error = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                return ServiceResult<string>.Fail(ServiceErrorType.Validation, error);
            }
        }

        var user = new ApplicationUser { UserName = input.Email, Email = input.Email, FullName = input.FullName };
        var result = await _userManager.CreateAsync(user, input.Password);
        if (!result.Succeeded)
        {
            var error = string.Join("; ", result.Errors.Select(e => e.Description));
            return ServiceResult<string>.Fail(ServiceErrorType.Validation, error);
        }

        await _userManager.AddToRoleAsync(user, adminRole);
        var token = GenerateToken(user, new[] { adminRole });
        return ServiceResult<string>.Ok(token);
    }

    private string GenerateToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var keyValue = jwtSection["Key"];
        if (string.IsNullOrWhiteSpace(keyValue))
            throw new InvalidOperationException("Jwt:Key is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyValue));
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

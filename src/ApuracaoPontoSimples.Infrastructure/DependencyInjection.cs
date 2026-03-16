using ApuracaoPontoSimples.Infrastructure.Identity;
using ApuracaoPontoSimples.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApuracaoPontoSimples.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var conn = configuration.GetConnectionString("DefaultConnection");
        var sqlPassword = Environment.GetEnvironmentVariable("SQLPassword", EnvironmentVariableTarget.Machine);
        conn = conn.Replace("{{SQLPassword}}", sqlPassword);

        services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(conn));

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<AppDbContext>();

        return services;
    }
}

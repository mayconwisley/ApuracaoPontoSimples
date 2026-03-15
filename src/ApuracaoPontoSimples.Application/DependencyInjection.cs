using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApuracaoPontoSimples.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITimeCardCalculator, TimeCardCalculator>();
        return services;
    }
}

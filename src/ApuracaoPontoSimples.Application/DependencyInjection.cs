using ApuracaoPontoSimples.Application.Interfaces;
using ApuracaoPontoSimples.Application.Services;
using ApuracaoPontoSimples.Application.UseCases.Employees;
using ApuracaoPontoSimples.Application.UseCases.Employers;
using ApuracaoPontoSimples.Application.UseCases.Holidays;
using ApuracaoPontoSimples.Application.UseCases.TimeCards;
using Microsoft.Extensions.DependencyInjection;

namespace ApuracaoPontoSimples.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITimeCardCalculator, TimeCardCalculator>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IEmployerService, EmployerService>();
        services.AddScoped<IHolidayService, HolidayService>();
        services.AddScoped<ITimeCardService, TimeCardService>();
        return services;
    }
}

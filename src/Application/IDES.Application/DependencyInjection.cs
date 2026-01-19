using Microsoft.Extensions.DependencyInjection;
using IDES.Application.Weather.Queries;

namespace IDES.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<GetWeatherForecastQueryHandler>();
        // Ajoutez ici d'autres services de la couche Application (ex: MediatR, AutoMapper)
        return services;
    }
}

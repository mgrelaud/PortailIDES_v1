using Microsoft.Extensions.DependencyInjection;

namespace IDES.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Ajoutez ici d'autres services de la couche Application (ex: MediatR, AutoMapper)
        return services;
    }
}

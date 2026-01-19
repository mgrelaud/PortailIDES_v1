using Microsoft.Extensions.DependencyInjection;
using IDES.Application.Interfaces;
using IDES.Infrastructure.Services;
using IDES.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IDES.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        services.AddSingleton<IGedService, GedService>();
        services.AddSingleton<IRecentFoldersService, RecentFoldersService>();
        services.AddSingleton<IFolderIndexService, FolderIndexService>();

        services.AddSingleton<IElementFactory, ElementFactory>();
        services.AddSingleton<IMoteurCalculService, MoteurCalculService>();
        services.AddScoped<ICatalogueService, CatalogueService>();

        services.AddSingleton<IConfigService, ConfigService>();
        services.AddSingleton<INumeroGeneratorService, NumeroGeneratorService>();
        services.AddSingleton<IMetreService, MetreService>();

        // Configuration de la base de données
        var dbPath = "catalogue.db"; // Simplifié pour l'instant
        services.AddDbContextFactory<CatalogueDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        return services;
    }
}

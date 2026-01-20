using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using MudBlazor.Services;
using MudBlazor;
using IDES.Application;
using IDES.Infrastructure;
using IDES.Application.Interfaces;
using IDES.Portail.MAUI.Services;

namespace IDES.Portail.MAUI;

public class MsalAccessTokenProvider : IAccessTokenProvider
{
    private readonly IPublicClientApplication _msalApp;
    private readonly string[] _scopes;

    public MsalAccessTokenProvider(IPublicClientApplication msalApp, string[] scopes)
    {
        _msalApp = msalApp;
        _scopes = scopes;
    }

    public async Task<string> GetAuthorizationTokenAsync(Uri uri, Dictionary<string, object>? additionalAuthenticationContext = default, CancellationToken cancellationToken = default)
    {
        var accounts = await _msalApp.GetAccountsAsync();
        try
        {
            var result = await _msalApp.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                .ExecuteAsync(cancellationToken);
            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            return string.Empty;
        }
    }

    public AllowedHostsValidator AllowedHostsValidator => new AllowedHostsValidator();
}

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        // --- ÉTAPE 1 : CHARGER LA CONFIGURATION ---
        var assembly = Assembly.GetExecutingAssembly();
        var configFileName = "IDES.Portail.MAUI.appsettings.json";
        using var stream = assembly.GetManifestResourceStream(configFileName);

        if (stream != null)
        {
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();
            builder.Configuration.AddConfiguration(config);
        }

        // --- ÉTAPE 2 : CONFIGURER LES SERVICES DE BASE DE MAUI ---
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // --- ÉTAPE 3 : CONFIGURER LES SERVICES DE L'APPLICATION (CLEAN ARCHITECTURE) ---
        builder.Services.AddApplicationServices();
        builder.Services.AddInfrastructureServices(builder.Configuration);

        // Ajout des services MudBlazor
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 3000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
        
        // Services UI
        builder.Services.AddSingleton<IFileOperationsService, FileOperationsService>();
        builder.Services.AddSingleton<GraphService>();

        // --- AUTHENTIFICATION MICROSOFT GRAPH (MSAL) ---
        const string ClientId = "a7fd8f97-d1cc-4630-8ba9-85881ca26e5e";
        const string TenantId = "02beb24d-d827-4a33-a448-5bf46de440c6";
        
#if WINDOWS
        const string RedirectUri = "http://localhost";
#else
        const string RedirectUri = $"msal{ClientId}://auth";
#endif

        var publicClientAppBuilder = PublicClientApplicationBuilder.Create(ClientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
            .WithRedirectUri(RedirectUri);

#if WINDOWS
        publicClientAppBuilder.WithDefaultRedirectUri();
#endif

        var publicClientApp = publicClientAppBuilder.Build();
        builder.Services.AddSingleton<IPublicClientApplication>(publicClientApp);

        builder.Services.AddSingleton<GraphServiceClient>(sp =>
        {
            var msalApp = sp.GetRequiredService<IPublicClientApplication>();
            var scopes = new[] { "User.Read", "Calendars.ReadWrite" };
            var tokenProvider = new MsalAccessTokenProvider(msalApp, scopes);
            var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);
            return new GraphServiceClient(authProvider);
        });

        builder.Services.AddTransient<MainPage>();

        var app = builder.Build();
        
        // Initialisation de la base de données
        app.Services.InitializeDatabase();

        return app;
    }
}

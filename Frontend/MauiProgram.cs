using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using MudBlazor.Services;
using MudBlazor;
using PortailMetier.Domain.Interfaces;
using PortailMetier.Infrastructure.Services;
using PortailMetier.Frontend.Services;

namespace PortailMetier.Frontend;

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
            // Note: En MAUI Hybrid, l'acquisition interactive doit idéalement être déclenchée 
            // par une action utilisateur dans le composant/service avant l'appel Graph.
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

        // --- ÉTAPE 1 : CHARGER LA CONFIGURATION (DOIT ÊTRE FAIT EN PREMIER) ---
        var assembly = Assembly.GetExecutingAssembly();
        var allResourceNames = assembly.GetManifestResourceNames();
        System.Diagnostics.Debug.WriteLine("--- RESSOURCES EMBARQUÉES DÉTECTÉES ---");
        foreach (var name in allResourceNames)
        {
            System.Diagnostics.Debug.WriteLine($"  -> {name}");
        }
        System.Diagnostics.Debug.WriteLine("------------------------------------");
        
        // Le nom de la ressource est "Namespace.NomDuFichier"
        var configFileName = "PortailMetier.Frontend.appsettings.json";
        using var stream = assembly.GetManifestResourceStream(configFileName);

        if (stream != null)
        {
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            // On ajoute la configuration lue au builder de l'application
            builder.Configuration.AddConfiguration(config);
        }
        else
        {
            // Erreur critique si le fichier n'est pas trouvé. Indispensable pour le débogage.
            System.Diagnostics.Debug.WriteLine($"FATAL ERROR: Embedded resource '{configFileName}' not found.");
            // On pourrait même lancer une exception ici pour arrêter le démarrage
            // throw new FileNotFoundException($"Embedded resource '{configFileName}' not found.");
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

        // --- ÉTAPE 3 : CONFIGURER LES SERVICES DE L'APPLICATION (MUD, VOS SERVICES...) ---

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

        // Ajout de vos services métier (GedService)
        // Il sera maintenant créé avec la bonne configuration !
        builder.Services.AddSingleton<IGedService, GedService>();
        builder.Services.AddSingleton<IRecentFoldersService, RecentFoldersService>();
        builder.Services.AddSingleton<IFolderIndexService, FolderIndexService>();
        builder.Services.AddSingleton<IFileOperationsService, FileOperationsService>();

        // --- AUTHENTIFICATION MICROSOFT GRAPH (MSAL) ---
        const string ClientId = "a7fd8f97-d1cc-4630-8ba9-85881ca26e5e";
        const string TenantId = "02beb24d-d827-4a33-a448-5bf46de440c6"; // Ou "common"
        
        // Sur Windows, MSAL exige souvent un loopback URI (http://localhost) 
        // alors que sur Mobile (Android/iOS) on utilise msal{clientid}://auth
#if WINDOWS
        const string RedirectUri = "http://localhost";
#else
        const string RedirectUri = $"msal{ClientId}://auth";
#endif

        var publicClientAppBuilder = PublicClientApplicationBuilder.Create(ClientId)
            .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
            .WithRedirectUri(RedirectUri);

        // Configuration spécifique pour Windows pour éviter l'erreur de loopback
#if WINDOWS
        publicClientAppBuilder.WithDefaultRedirectUri(); // Utilise http://localhost par défaut pour MSAL Desktop
#endif

        var publicClientApp = publicClientAppBuilder.Build();

        builder.Services.AddSingleton<IPublicClientApplication>(publicClientApp);

        // Enregistrement de GraphServiceClient utilisant MSAL
        builder.Services.AddSingleton<GraphServiceClient>(sp =>
        {
            var msalApp = sp.GetRequiredService<IPublicClientApplication>();
            var scopes = new[] { "User.Read", "Calendars.ReadWrite" };

            var tokenProvider = new MsalAccessTokenProvider(msalApp, scopes);
            var authProvider = new BaseBearerTokenAuthenticationProvider(tokenProvider);
            
            return new GraphServiceClient(authProvider);
        });

        builder.Services.AddSingleton<GraphService>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddSingleton<App>();

        // --- ÉTAPE 4 : CONSTRUIRE L'APPLICATION ---
        return builder.Build();
    }
}

using Microsoft.Identity.Client;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Azure.Identity;
using System.Net.Http.Headers;

namespace PortailMetier.Frontend.Services;

public class GraphService
{
    private readonly IPublicClientApplication _publicClientApp;
    private readonly GraphServiceClient _graphClient;
    private readonly string[] _scopes = { "User.Read", "Calendars.ReadWrite" };
    private AuthenticationResult? _authResult;

    public GraphService(IPublicClientApplication publicClientApp, GraphServiceClient graphClient)
    {
        _publicClientApp = publicClientApp;
        _graphClient = graphClient;
    }

    public bool IsAuthenticated => _authResult != null;
    public string? UserName => _authResult?.Account.Username;

    /// <summary>
    /// Authentifie l'utilisateur via MSAL (silencieusement puis interactivement)
    /// </summary>
    public async Task<AuthenticationResult> LoginAsync()
    {
        var accounts = await _publicClientApp.GetAccountsAsync();

        try
        {
            // Tentative d'acquisition de jeton silencieuse
            _authResult = await _publicClientApp.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
        }
        catch (MsalUiRequiredException)
        {
            // Si l'acquisition silencieuse échoue, on lance le flux interactif
            _authResult = await _publicClientApp.AcquireTokenInteractive(_scopes)
                .WithParentActivityOrWindow(GetParentWindow()) // Nécessaire pour Windows/Android
                .ExecuteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GraphService] Login error: {ex.Message}");
            throw;
        }

        return _authResult;
    }

    /// <summary>
    /// Déconnexion de l'utilisateur
    /// </summary>
    public async Task LogoutAsync()
    {
        var accounts = await _publicClientApp.GetAccountsAsync();
        foreach (var account in accounts)
        {
            await _publicClientApp.RemoveAsync(account);
        }
        _authResult = null;
    }

    /// <summary>
    /// Récupère les événements du calendrier pour la semaine à venir
    /// </summary>
    public async Task<IEnumerable<Event>> GetMyEventsForWeekAsync(DateTime startDate)
    {
        try
        {
            // S'assurer qu'on est connecté
            if (_authResult == null)
            {
                await LoginAsync();
            }

            // Définir la plage de recherche
            string start = startDate.ToUniversalTime().ToString("o");
            string end = startDate.AddDays(7).ToUniversalTime().ToString("o");

            // Appel à l'API Graph Me.CalendarView
            var request = await _graphClient.Me.CalendarView
                .GetAsync(config =>
                {
                    config.QueryParameters.StartDateTime = start;
                    config.QueryParameters.EndDateTime = end;
                    config.QueryParameters.Top = 50;
                    config.QueryParameters.Select = new[] { "subject", "start", "end", "location" };
                    config.QueryParameters.Orderby = new[] { "start/dateTime" };
                });

            return request?.Value ?? new List<Event>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GraphService] GetEvents error: {ex.Message}");
            return new List<Event>();
        }
    }

    /// <summary>
    /// Récupère l'objet Window parent pour l'authentification interactive
    /// </summary>
    private object? GetParentWindow()
    {
#if WINDOWS
        return ((MauiWinUIWindow)App.Current!.Windows[0].Handler.PlatformView).WindowHandle;
#else
        return null;
#endif
    }
}

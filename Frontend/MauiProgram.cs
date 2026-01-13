using Microsoft.Extensions.Logging;
using MudBlazor.Services;
using MudBlazor; // On peut même enlever ce using s'il n'est plus utilisé ici

namespace PortailMetier.Frontend;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        // On garde uniquement l'ajout des services MudBlazor, sans aucune configuration.
        builder.Services.AddMudServices();

        builder.Services.AddSingleton(new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#0063cc",
                Secondary = "#6e6e6e",
                Background = "#f5f5f5",
                Surface = "#ffffff",
                AppbarBackground = "#0063cc"
            },
            PaletteDark = new PaletteDark()
            {
                Primary = "#0063cc",
                Secondary = "#6e6e6e",
                Background = "#1c1c19",
                Surface = "#272720",
                AppbarBackground = "#1c1c19"
            }
        });

        return builder.Build();
    }
}

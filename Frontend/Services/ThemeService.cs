namespace PortailMetier.Frontend.Services;

/// <summary>
/// Service de gestion du thème de l'application (clair/sombre).
/// Utilise un pattern événementiel pour notifier les composants des changements de thème.
/// </summary>
public class ThemeService
{
    /// <summary>
    /// Indique si le mode sombre est activé.
    /// </summary>
    public bool IsDarkMode { get; private set; } = false;

    /// <summary>
    /// Événement déclenché lorsque le thème change.
    /// Les composants peuvent s'abonner à cet événement pour se rafraîchir.
    /// </summary>
    public event Action? OnThemeChanged;

    /// <summary>
    /// Bascule entre le mode clair et le mode sombre.
    /// </summary>
    public void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        OnThemeChanged?.Invoke();
    }

    /// <summary>
    /// Active le mode clair.
    /// </summary>
    public void SetLightMode()
    {
        if (IsDarkMode)
        {
            IsDarkMode = false;
            OnThemeChanged?.Invoke();
        }
    }

    /// <summary>
    /// Active le mode sombre.
    /// </summary>
    public void SetDarkMode()
    {
        if (!IsDarkMode)
        {
            IsDarkMode = true;
            OnThemeChanged?.Invoke();
        }
    }
}

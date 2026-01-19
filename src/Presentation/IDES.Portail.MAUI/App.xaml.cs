namespace IDES.Portail.MAUI;

public partial class App : Microsoft.Maui.Controls.Application
{
	private readonly MainPage _mainPage;

	public App(MainPage mainPage)
	{
		InitializeComponent();
		_mainPage = mainPage;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		var window = new Window(_mainPage) 
		{ 
			Title = "Portail Métier IDES"
		};

		// Configuration de la taille et position de la fenêtre
		window.Created += (s, e) =>
		{
			// Obtenir la taille de l'écran
			var displayInfo = DeviceDisplay.MainDisplayInfo;
			
			// Calculer 90% de la taille de l'écran pour laisser de l'espace
			var width = displayInfo.Width / displayInfo.Density * 0.9;
			var height = displayInfo.Height / displayInfo.Density * 0.9;
			
			// Centrer la fenêtre
			var x = (displayInfo.Width / displayInfo.Density - width) / 2;
			var y = (displayInfo.Height / displayInfo.Density - height) / 2;
			
			window.Width = width;
			window.Height = height;
			window.X = x;
			window.Y = y;
			
			// Taille minimale
			window.MinimumWidth = 1024;
			window.MinimumHeight = 768;
		};

		return window;
	}
}

using PortailMetier.Domain.Interfaces;

namespace PortailMetier.Frontend;

public partial class MainPage : ContentPage
{
	private readonly IFolderIndexService _indexService;

	public MainPage(IFolderIndexService indexService)
	{
		_indexService = indexService;
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
		try
		{
			await _indexService.InitializeAsync();
			
			// Si l'index est vide ou trop vieux, on lance une réindexation en arrière-plan
			if (_indexService.NeedsReindex())
			{
				_ = Task.Run(async () => {
					try {
						await _indexService.ReindexRootFoldersAsync();
					} catch (Exception ex) {
						Console.WriteLine($"[MainPage] Background reindex failed: {ex.Message}");
					}
				});
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"[MainPage] Index initialization failed: {ex.Message}");
		}
	}
}

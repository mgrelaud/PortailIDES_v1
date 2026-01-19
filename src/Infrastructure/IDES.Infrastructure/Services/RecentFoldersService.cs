using IDES.Application.Dtos;
using IDES.Application.Interfaces;
using System.Text.Json;

namespace IDES.Infrastructure.Services;

public class RecentFoldersService : IRecentFoldersService
{
    private const int MaxRecentFolders = 20;
    private const string RecentFoldersKey = "RecentFolders";
    private const string ExpandedStateKey = "ExpandedState";
    private const string LastFolderKey = "LastOpenedFolder";
    
    private readonly string _storagePath;
    private List<DossierDto> _recentFolders = new();
    private HashSet<string> _expandedState = new();
    private DossierDto? _lastOpenedFolder;

    public RecentFoldersService()
    {
        // Stockage dans le dossier AppData local
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _storagePath = Path.Combine(appDataPath, "PortailMetier");
        
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
        
        LoadState();
    }

    public List<DossierDto> GetRecentFolders()
    {
        return _recentFolders.ToList();
    }

    public void AddRecentFolder(DossierDto folder)
    {
        // Supprimer si déjà présent (pour le remettre en haut)
        _recentFolders.RemoveAll(f => f.CheminCompletUNC == folder.CheminCompletUNC);
        
        // Ajouter en premier
        _recentFolders.Insert(0, folder);
        
        // Limiter à 20
        if (_recentFolders.Count > MaxRecentFolders)
        {
            _recentFolders = _recentFolders.Take(MaxRecentFolders).ToList();
        }
        
        // Sauvegarder le dernier dossier
        _lastOpenedFolder = folder;
        
        SaveState();
    }

    public DossierDto? GetLastOpenedFolder()
    {
        return _lastOpenedFolder;
    }

    public void SaveExpandedState(HashSet<string> expandedPaths)
    {
        _expandedState = expandedPaths;
        SaveState();
    }

    public HashSet<string> GetExpandedState()
    {
        return _expandedState;
    }

    private void LoadState()
    {
        try
        {
            var recentFile = Path.Combine(_storagePath, "recent_folders.json");
            if (File.Exists(recentFile))
            {
                var json = File.ReadAllText(recentFile);
                var data = JsonSerializer.Deserialize<StorageData>(json);
                if (data != null)
                {
                    _recentFolders = data.RecentFolders ?? new List<DossierDto>();
                    _expandedState = data.ExpandedPaths?.ToHashSet() ?? new HashSet<string>();
                    _lastOpenedFolder = data.LastOpenedFolder;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RecentFoldersService] Error loading state: {ex.Message}");
        }
    }

    private void SaveState()
    {
        try
        {
            var recentFile = Path.Combine(_storagePath, "recent_folders.json");
            var data = new StorageData
            {
                RecentFolders = _recentFolders,
                ExpandedPaths = _expandedState.ToList(),
                LastOpenedFolder = _lastOpenedFolder
            };
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(recentFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RecentFoldersService] Error saving state: {ex.Message}");
        }
    }

    private class StorageData
    {
        public List<DossierDto>? RecentFolders { get; set; }
        public List<string>? ExpandedPaths { get; set; }
        public DossierDto? LastOpenedFolder { get; set; }
    }
}

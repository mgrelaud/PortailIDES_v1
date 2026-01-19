using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using IDES.Application.Dtos;
using IDES.Application.Interfaces;
using System.IO;

namespace IDES.Infrastructure.Services;

/// <summary>
/// Service d'indexation des dossiers utilisant SQLite
/// </summary>
public class FolderIndexService : IFolderIndexService
{
    private readonly string _basePath;
    private readonly string _dbPath;
    private readonly string _connectionString;
    private DateTime? _lastIndexDate;

    public FolderIndexService(IConfiguration configuration)
    {
        _basePath = configuration.GetValue<string>("GedSettings:BaseUncPath") 
                    ?? throw new ArgumentNullException("GedSettings:BaseUncPath");
        
        // Base de données dans AppData local
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var storagePath = Path.Combine(appDataPath, "PortailMetier");
        
        if (!Directory.Exists(storagePath))
        {
            Directory.CreateDirectory(storagePath);
        }
        
        _dbPath = Path.Combine(storagePath, "folder_index.db");
        _connectionString = $"Data Source={_dbPath}";
    }

    public async Task InitializeAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        // Créer les tables si elles n'existent pas
        var createTableCmd = connection.CreateCommand();
        createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Folders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Nom TEXT NOT NULL,
                CheminCompletUNC TEXT NOT NULL UNIQUE,
                ParentPath TEXT,
                HasContent INTEGER DEFAULT 0,
                DateIndexation TEXT NOT NULL,
                DateModification TEXT
            );
            
            CREATE INDEX IF NOT EXISTS idx_folders_nom ON Folders(Nom);
            CREATE INDEX IF NOT EXISTS idx_folders_parent ON Folders(ParentPath);
            
            CREATE TABLE IF NOT EXISTS IndexMetadata (
                Key TEXT PRIMARY KEY,
                Value TEXT
            );
        ";
        await createTableCmd.ExecuteNonQueryAsync();

        // Charger la date de dernière indexation
        var getDateCmd = connection.CreateCommand();
        getDateCmd.CommandText = "SELECT Value FROM IndexMetadata WHERE Key = 'LastIndexDate'";
        var result = await getDateCmd.ExecuteScalarAsync();
        if (result != null && DateTime.TryParse(result.ToString(), out var date))
        {
            _lastIndexDate = date;
        }

        Console.WriteLine($"[FolderIndexService] Initialized. DB: {_dbPath}");
        Console.WriteLine($"[FolderIndexService] Last index date: {_lastIndexDate?.ToString() ?? "Never"}");
    }

    public async Task ReindexRootFoldersAsync()
    {
        Console.WriteLine("[FolderIndexService] Starting reindexation...");
        
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        // Utiliser une transaction pour de meilleures performances
        using var transaction = connection.BeginTransaction();

        try
        {
            // Supprimer les anciens dossiers racine
            var deleteCmd = connection.CreateCommand();
            deleteCmd.CommandText = "DELETE FROM Folders WHERE ParentPath IS NULL";
            await deleteCmd.ExecuteNonQueryAsync();

            // Scanner les dossiers racine
            var rootDir = new DirectoryInfo(_basePath);
            if (!rootDir.Exists)
            {
                Console.WriteLine($"[FolderIndexService] Base path does not exist: {_basePath}");
                return;
            }

            var insertCmd = connection.CreateCommand();
            insertCmd.CommandText = @"
                INSERT INTO Folders (Nom, CheminCompletUNC, ParentPath, HasContent, DateIndexation)
                VALUES (@nom, @chemin, NULL, @hasContent, @dateIndex)
            ";

            var nomParam = insertCmd.CreateParameter();
            nomParam.ParameterName = "@nom";
            insertCmd.Parameters.Add(nomParam);

            var cheminParam = insertCmd.CreateParameter();
            cheminParam.ParameterName = "@chemin";
            insertCmd.Parameters.Add(cheminParam);

            var hasContentParam = insertCmd.CreateParameter();
            hasContentParam.ParameterName = "@hasContent";
            insertCmd.Parameters.Add(hasContentParam);

            var dateParam = insertCmd.CreateParameter();
            dateParam.ParameterName = "@dateIndex";
            dateParam.Value = DateTime.Now.ToString("o");
            insertCmd.Parameters.Add(dateParam);

            int count = 0;
            foreach (var dir in rootDir.GetDirectories())
            {
                try
                {
                    nomParam.Value = dir.Name;
                    cheminParam.Value = dir.FullName;
                    hasContentParam.Value = HasContent(dir) ? 1 : 0;
                    await insertCmd.ExecuteNonQueryAsync();
                    count++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[FolderIndexService] Error indexing {dir.Name}: {ex.Message}");
                }
            }

            // Mettre à jour la date de dernière indexation
            var updateMetaCmd = connection.CreateCommand();
            updateMetaCmd.CommandText = @"
                INSERT OR REPLACE INTO IndexMetadata (Key, Value) 
                VALUES ('LastIndexDate', @date)
            ";
            updateMetaCmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("o"));
            await updateMetaCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
            _lastIndexDate = DateTime.Now;

            Console.WriteLine($"[FolderIndexService] Indexed {count} folders.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"[FolderIndexService] Reindexation failed: {ex.Message}");
            throw;
        }
    }

    public async Task<List<DossierDto>> SearchAsync(string term, int maxResults = 20)
    {
        var results = new List<DossierDto>();

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT Nom, CheminCompletUNC, HasContent 
            FROM Folders 
            WHERE Nom LIKE @term 
            ORDER BY Nom 
            LIMIT @limit
        ";
        cmd.Parameters.AddWithValue("@term", $"%{term}%");
        cmd.Parameters.AddWithValue("@limit", maxResults);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new DossierDto
            {
                Id = reader.GetString(1),
                Nom = reader.GetString(0),
                CheminCompletUNC = reader.GetString(1),
                HasContent = reader.GetInt32(2) == 1
            });
        }

        Console.WriteLine($"[FolderIndexService] Search '{term}' found {results.Count} results");
        return results;
    }

    public async Task<List<DossierDto>> GetRootFoldersAsync()
    {
        var results = new List<DossierDto>();

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT Nom, CheminCompletUNC, HasContent 
            FROM Folders 
            WHERE ParentPath IS NULL
            ORDER BY Nom
        ";

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new DossierDto
            {
                Id = reader.GetString(1),
                Nom = reader.GetString(0),
                CheminCompletUNC = reader.GetString(1),
                HasContent = reader.GetInt32(2) == 1
            });
        }

        Console.WriteLine($"[FolderIndexService] GetRootFolders returned {results.Count} folders");
        return results;
    }

    public DateTime? GetLastIndexDate() => _lastIndexDate;

    public bool NeedsReindex(int hoursThreshold = 24)
    {
        if (_lastIndexDate == null) return true;
        return (DateTime.Now - _lastIndexDate.Value).TotalHours > hoursThreshold;
    }

    private static bool HasContent(DirectoryInfo d)
    {
        try
        {
            return d.EnumerateFileSystemInfos().Any();
        }
        catch
        {
            return false;
        }
    }
}

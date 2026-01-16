using Microsoft.Extensions.Configuration;
using PortailMetier.Domain.Dtos;
using PortailMetier.Domain.Interfaces;
using System.IO;

namespace PortailMetier.Infrastructure.Services;

public class GedService : IGedService
{
    private readonly string _basePath;
    private readonly IFolderIndexService _indexService;
    private List<DossierDto>? _cacheArborescence;
    private static readonly object _cacheLock = new();
    
    public string BasePath => _basePath;

    public GedService(IConfiguration configuration, IFolderIndexService indexService)
    {
        _basePath = configuration.GetValue<string>("GedSettings:BaseUncPath")
                    ?? throw new ArgumentNullException("Le chemin 'GedSettings:BaseUncPath' n'est pas configuré.");
        _indexService = indexService;
    }
    
    // Version rapide de HasContent - vérifie juste s'il y a des sous-dossiers ou fichiers
    public static bool HasContentFast(DirectoryInfo d)
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

    public Task<List<FichierDto>> GetFichiersAsync(string cheminDossierUnc)
    {
        Console.WriteLine($"[GedService] GetFichiersAsync: '{cheminDossierUnc}'");
        var fichiersDto = new List<FichierDto>();
        try
        {
            var di = new DirectoryInfo(cheminDossierUnc);
            if (!di.Exists)
            {
                return Task.FromResult(fichiersDto); // Retourne une liste vide si le dossier n'existe pas
            }

            var fichiers = di.GetFiles("*", SearchOption.TopDirectoryOnly);

            foreach (var fichier in fichiers)
            {
                fichiersDto.Add(new FichierDto
                {
                    Nom = fichier.Name,
                    TailleHumaine = ToTailleHumaine(fichier.Length),
                    Type = GetTypeFichier(fichier.Extension),
                    DateModifUtc = fichier.LastWriteTimeUtc,
                    CheminCompletUNC = fichier.FullName
                });
            }
        }
        catch (Exception ex)
        {
            // Gérer les erreurs d'accès (droits, etc.)
            Console.WriteLine($"Erreur accès fichiers pour {cheminDossierUnc}: {ex.Message}");
        }

        return Task.FromResult(fichiersDto.OrderBy(f => f.Nom).ToList());
    }

    public Task<List<DossierDto>> GetArborescenceDossiersAsync(bool recursive = false)
    {
        // Le système de cache est bon, mais pour la recherche on doit pouvoir forcer le rechargement récursif.
        // Pour l'instant, on simplifie : si on demande en récursif, on ne prend pas le cache.
        if (recursive)
        {
            var root = new DirectoryInfo(_basePath);
            var allFolders = root.GetDirectories()
                                 .Select(dir => MapToDto(dir, true)) // On passe le flag récursif
                                 .OrderBy(d => d.Nom)
                                 .ToList();
            return Task.FromResult(allFolders);
        }

        // Logique de cache existante pour le TreeView (non-récursif)
        lock (_cacheLock)
        {
            if (_cacheArborescence != null)
            {
                return Task.FromResult(_cacheArborescence);
            }

            var root = new DirectoryInfo(_basePath);
            _cacheArborescence = root.GetDirectories()
                                     .Select(dir => MapToDto(dir, false)) // On passe le flag non-récursif
                                     .OrderBy(d => d.Nom)
                                     .ToList();

            return Task.FromResult(_cacheArborescence);
        }
    }


    private DossierDto MapToDto(DirectoryInfo dir, bool recursive)
    {
        var dto = new DossierDto
        {
            Id = dir.FullName, // Utilisons le chemin complet comme ID unique
            Nom = dir.Name,
            CheminCompletUNC = dir.FullName
        };

        // Si on est en mode récursif, on continue de scanner les sous-dossiers
        if (recursive)
        {
            try
            {
                dto.SousDossiers = dir.GetDirectories()
                                      .Select(subDir => MapToDto(subDir, true)) // On continue la récursion
                                      .OrderBy(d => d.Nom)
                                      .ToList();
            }
            catch (UnauthorizedAccessException)
            {
                // On ignore les dossiers inaccessibles
            }
        }
        // Si on n'est PAS en mode récursif, on ne charge que le premier niveau pour le TreeView
        // (Votre code original ne le faisait pas, mais c'est mieux pour la performance du TreeView)
        else
        {
            try
            {
                dto.SousDossiers = dir.GetDirectories()
                                      .Select(subDir => new DossierDto
                                      {
                                          Id = subDir.FullName,
                                          Nom = subDir.Name,
                                          CheminCompletUNC = subDir.FullName
                                      })
                                      .OrderBy(d => d.Nom)
                                      .ToList();
            }
            catch (UnauthorizedAccessException) { }
        }

        return dto;
    }


    public async Task<List<DossierDto>> GetSousDossiersAsync(string? cheminDossierUnc = null)
    {
        // Si on demande la racine, utiliser l'index SQLite
        if (string.IsNullOrWhiteSpace(cheminDossierUnc))
        {
            Console.WriteLine("[GedService] GetSousDossiersAsync: Returning root folders from SQLite index");
            return await _indexService.GetRootFoldersAsync();
        }

        // Sinon, charger les sous-dossiers du chemin spécifié
        Console.WriteLine($"[GedService] GetSousDossiersAsync: Loading from '{cheminDossierUnc}'");
        var result = new List<DossierDto>();

        try
        {
            var dirInfo = new DirectoryInfo(cheminDossierUnc);
            if (!dirInfo.Exists) 
            {
                 Console.WriteLine($"[GedService] Path does not exist: {cheminDossierUnc}");
                 return result;
            }

            result = dirInfo.GetDirectories()
                            .Where(d => !d.Name.Equals("docupro", StringComparison.OrdinalIgnoreCase) 
                                     && !d.Name.Equals("mels_du_dossier", StringComparison.OrdinalIgnoreCase))
                            .Select(d => new DossierDto
                            {
                                Id = d.FullName,
                                Nom = d.Name,
                                CheminCompletUNC = d.FullName,
                                HasContent = HasContentFast(d)
                            })
                            .OrderBy(d => d.Nom)
                            .ToList();
             Console.WriteLine($"[GedService] Found {result.Count} folders.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[GedService] Error reading folders: {ex.Message}");
        }

        return result;
    }

    public async Task<List<DossierDto>> SearchDossiersAsync(string term)
    {
        Console.WriteLine($"[GedService] SearchDossiersAsync: '{term}' (using SQLite index)");
        
        if (string.IsNullOrWhiteSpace(term)) 
            return new List<DossierDto>();

        return await _indexService.SearchAsync(term);
    }

    // Fonctions utilitaires
    private static bool HasContent(DirectoryInfo d)
    {
         try
         {
             // On regarde s'il y a au moins un fichier ou un dossier
             return d.EnumerateFileSystemInfos().Any();
         }
         catch 
         {
             return false;
         }
    }

    private static string ToTailleHumaine(long length)
    {
        if (length == 0) return "0 B";
        string[] sizes = { "B", "Ko", "Mo", "Go", "To" };
        int order = (int)Math.Log(length, 1024);
        return $"{length / Math.Pow(1024, order):0.##} {sizes[order]}";
    }

    private static string GetTypeFichier(string extension)
    {
        return extension.ToLowerInvariant() switch
        {
            ".pdf" => "Fichier PDF",
            ".rvt" => "Fichier Revit",
            ".docx" or ".doc" => "Document Word",
            ".xlsx" or ".xls" => "Feuille de calcul Excel",
            ".dwg" => "Dessin AutoCAD",
            _ => $"Fichier ({extension})"
        };
    }
}

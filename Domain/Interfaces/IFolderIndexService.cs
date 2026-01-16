using PortailMetier.Domain.Dtos;

namespace PortailMetier.Domain.Interfaces;

/// <summary>
/// Service d'indexation des dossiers pour accélérer les recherches
/// </summary>
public interface IFolderIndexService
{
    /// <summary>
    /// Initialise l'index (crée la base si nécessaire)
    /// </summary>
    Task InitializeAsync();
    
    /// <summary>
    /// Lance une réindexation complète des dossiers racine
    /// </summary>
    Task ReindexRootFoldersAsync();
    
    /// <summary>
    /// Recherche des dossiers par nom dans l'index
    /// </summary>
    Task<List<DossierDto>> SearchAsync(string term, int maxResults = 20);
    
    /// <summary>
    /// Récupère tous les dossiers racine depuis l'index
    /// </summary>
    Task<List<DossierDto>> GetRootFoldersAsync();
    
    /// <summary>
    /// Récupère la date de dernière indexation
    /// </summary>
    DateTime? GetLastIndexDate();
    
    /// <summary>
    /// Vérifie si une réindexation est nécessaire (plus de X heures)
    /// </summary>
    bool NeedsReindex(int hoursThreshold = 24);
}

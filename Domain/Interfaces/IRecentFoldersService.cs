using PortailMetier.Domain.Dtos;

namespace PortailMetier.Domain.Interfaces;

public interface IRecentFoldersService
{
    /// <summary>
    /// Récupère la liste des dossiers récents (max 20)
    /// </summary>
    List<DossierDto> GetRecentFolders();
    
    /// <summary>
    /// Ajoute un dossier à la liste des récents
    /// </summary>
    void AddRecentFolder(DossierDto folder);
    
    /// <summary>
    /// Récupère le dernier dossier ouvert
    /// </summary>
    DossierDto? GetLastOpenedFolder();
    
    /// <summary>
    /// Sauvegarde l'état d'expansion des dossiers
    /// </summary>
    void SaveExpandedState(HashSet<string> expandedPaths);
    
    /// <summary>
    /// Récupère l'état d'expansion sauvegardé
    /// </summary>
    HashSet<string> GetExpandedState();
}

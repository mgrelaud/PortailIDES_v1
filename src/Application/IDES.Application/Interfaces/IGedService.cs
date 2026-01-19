using IDES.Application.Dtos;

namespace IDES.Application.Interfaces;

public interface IGedService
{
    /// <summary>
    /// Récupère la liste complète des dossiers sous forme d'arborescence.
    /// Utilise un cache pour éviter de scanner le disque à chaque fois.
    /// </summary>
    Task<List<DossierDto>> GetArborescenceDossiersAsync(bool recursive = false);

    /// <summary>
    /// Récupère la liste des fichiers pour un chemin de dossier UNC donné.
    /// </summary>
    Task<List<FichierDto>> GetFichiersAsync(string cheminDossierUnc);
    Task<List<DossierDto>> GetSousDossiersAsync(string? cheminDossierUnc = null);
    Task<List<DossierDto>> SearchDossiersAsync(string term);
    string BasePath { get; }
}

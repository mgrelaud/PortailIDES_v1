using IDES.Domain.Catalogue;

namespace IDES.Application.Interfaces;

public interface ICatalogueService
{
    // Éléments
    Task<List<DefinitionElement>> GetCatalogueAsync();
    Task<DefinitionElement?> GetElementByIdAsync(int id);
    Task SaveElementAsync(DefinitionElement element);
    Task DeleteElementAsync(int id);

    // Propriétés
    Task<List<DefinitionPropriete>> GetToutesLesProprietesAsync();
    Task<DefinitionPropriete?> GetProprieteByIdAsync(int id);
    Task SaveProprieteAsync(DefinitionPropriete propriete);
    Task DeleteProprieteAsync(int id);
    Task<bool> EstProprieteUtiliseeAsync(int id);
    Task DeleteElementProprieteAsync(int id);
}

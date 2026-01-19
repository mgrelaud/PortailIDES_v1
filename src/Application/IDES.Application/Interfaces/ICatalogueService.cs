using IDES.Domain.Catalogue;

namespace IDES.Application.Interfaces;

public interface ICatalogueService
{
    Task<List<DefinitionElement>> GetCatalogueAsync();
}

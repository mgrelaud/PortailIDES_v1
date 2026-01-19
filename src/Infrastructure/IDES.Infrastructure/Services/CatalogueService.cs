using IDES.Application.Interfaces;
using IDES.Domain.Catalogue;
using IDES.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IDES.Infrastructure.Services;

public class CatalogueService : ICatalogueService
{
    private readonly IDbContextFactory<CatalogueDbContext> _dbFactory;

    public CatalogueService(IDbContextFactory<CatalogueDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public async Task<List<DefinitionElement>> GetCatalogueAsync()
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.TypesElements
            .Include(e => e.ProprietesAssociees)
                .ThenInclude(pa => pa.DefinitionPropriete)
            .ToListAsync();
    }
}

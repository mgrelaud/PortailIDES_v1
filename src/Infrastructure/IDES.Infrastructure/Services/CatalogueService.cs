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

    public async Task<DefinitionElement?> GetElementByIdAsync(int id)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.TypesElements
            .Include(e => e.ProprietesAssociees)
                .ThenInclude(pa => pa.DefinitionPropriete)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task SaveElementAsync(DefinitionElement element)
    {
        try
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            
            // Pour éviter que EF n'essaie d'insérer des doublons des définitions de propriétés
            var seenProps = new HashSet<int>();
            foreach (var pa in element.ProprietesAssociees)
            {
                if (pa.DefinitionPropriete != null && !seenProps.Contains(pa.DefinitionProprieteId))
                {
                    db.Entry(pa.DefinitionPropriete).State = EntityState.Unchanged;
                    seenProps.Add(pa.DefinitionProprieteId);
                }
            }

            if (element.Id == 0)
            {
                await db.TypesElements.AddAsync(element);
            }
            else
            {
                var existing = await db.TypesElements
                    .Include(e => e.ProprietesAssociees)
                    .FirstOrDefaultAsync(e => e.Id == element.Id);

                if (existing != null)
                {
                    db.Entry(existing).CurrentValues.SetValues(element);

                    // Mise à jour des propriétés associées (sync)
                    foreach (var pa in element.ProprietesAssociees)
                    {
                        var existingPa = existing.ProprietesAssociees
                            .FirstOrDefault(p => p.DefinitionProprieteId == pa.DefinitionProprieteId);

                        if (existingPa != null)
                        {
                            existingPa.Valeur = pa.Valeur;
                        }
                        else
                        {
                            existing.ProprietesAssociees.Add(new ElementPropriete
                            {
                                DefinitionElementId = existing.Id,
                                DefinitionProprieteId = pa.DefinitionProprieteId,
                                Valeur = pa.Valeur
                            });
                        }
                    }

                    // Suppression des propriétés qui ne sont plus dans la liste
                    var toRemove = existing.ProprietesAssociees
                        .Where(epa => !element.ProprietesAssociees.Any(pa => pa.DefinitionProprieteId == epa.DefinitionProprieteId))
                        .ToList();

                    foreach (var paToRemove in toRemove)
                    {
                        existing.ProprietesAssociees.Remove(paToRemove);
                    }
                }
                else
                {
                    db.TypesElements.Update(element);
                }
            }
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CatalogueService] SaveElementAsync error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[CatalogueService] Inner error: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    public async Task DeleteElementAsync(int id)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var element = await db.TypesElements.FindAsync(id);
        if (element != null)
        {
            db.TypesElements.Remove(element);
            await db.SaveChangesAsync();
        }
    }

    public async Task<List<DefinitionPropriete>> GetToutesLesProprietesAsync()
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Proprietes.ToListAsync();
    }

    public async Task<DefinitionPropriete?> GetProprieteByIdAsync(int id)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.Proprietes.FindAsync(id);
    }

    public async Task SaveProprieteAsync(DefinitionPropriete propriete)
    {
        try
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            if (propriete.Id == 0)
            {
                await db.Proprietes.AddAsync(propriete);
            }
            else
            {
                var existing = await db.Proprietes.FindAsync(propriete.Id);
                if (existing != null)
                {
                    db.Entry(existing).CurrentValues.SetValues(propriete);
                }
                else
                {
                    db.Proprietes.Update(propriete);
                }
            }
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CatalogueService] SaveProprieteAsync error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[CatalogueService] Inner error: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    public async Task DeleteProprieteAsync(int id)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var propriete = await db.Proprietes.FindAsync(id);
        if (propriete != null)
        {
            db.Proprietes.Remove(propriete);
            await db.SaveChangesAsync();
        }
    }

    public async Task<bool> EstProprieteUtiliseeAsync(int id)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        return await db.ElementProprietes.AnyAsync(ep => ep.DefinitionProprieteId == id);
    }

    public async Task DeleteElementProprieteAsync(int id)
    {
        using var db = await _dbFactory.CreateDbContextAsync();
        var ep = await db.ElementProprietes.FindAsync(id);
        if (ep != null)
        {
            db.ElementProprietes.Remove(ep);
            await db.SaveChangesAsync();
        }
    }
}

using IDES.Domain.Catalogue;
using Microsoft.EntityFrameworkCore;

namespace IDES.Infrastructure.Persistence;

public class CatalogueDbContext : DbContext
{
    public CatalogueDbContext(DbContextOptions<CatalogueDbContext> options)
        : base(options)
    {
    }

    public DbSet<DefinitionElement> TypesElements { get; set; }
    public DbSet<DefinitionPropriete> Proprietes { get; set; }
    public DbSet<ElementPropriete> ElementProprietes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ElementPropriete>()
            .HasOne(ep => ep.DefinitionElement)
            .WithMany(de => de.ProprietesAssociees)
            .HasForeignKey(ep => ep.DefinitionElementId);

        modelBuilder.Entity<ElementPropriete>()
            .HasOne(ep => ep.DefinitionPropriete)
            .WithMany(dp => dp.ElementsAssocies)
            .HasForeignKey(ep => ep.DefinitionProprieteId);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tenant.Entities
{
    public class InfraEntityConfiguration<TContext> :
        EntityTypeConfigurationSupplier<TContext>,
        IEntityTypeConfiguration<Affiliation>,
        IEntityTypeConfiguration<Category>
        where TContext : DbContext
    {
        public void Configure(EntityTypeBuilder<Category> entity)
        {
            entity.ToTable("TenantCategory");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired();

            entity.Property(e => e.Color)
                .IsRequired();

            entity.HasIndex(e => e.SortOrder);

            entity.HasIndex(e => e.IsPublic);

            entity.HasData(
                new Category { Id = -1, Name = "System", Color = "#ff2bea", IsPublic = false, SortOrder = 9 },
                new Category { Id = -2, Name = "Self-Registered", Color = "#33cc44", IsPublic = true, SortOrder = 8 },
                new Category { Id = -3, Name = "Participants", Color = "#ffffff", IsPublic = true, SortOrder = 0 },
                new Category { Id = -4, Name = "Observers", Color = "#ffcc33", IsPublic = true, SortOrder = 0 },
                new Category { Id = -5, Name = "Organisation", Color = "#ff99cc", IsPublic = true, SortOrder = 1 },
                new Category { Id = -6, Name = "Companies", Color = "#96d5ff", IsPublic = true, SortOrder = 1 });
        }

        public void Configure(EntityTypeBuilder<Affiliation> entity)
        {
            entity.ToTable("TenantAffiliation");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedNever();

            entity.HasIndex(e => e.Abbreviation);

            entity.Property(e => e.CountryCode)
                .IsUnicode(false)
                .HasMaxLength(7);

            entity.HasData(
                new Affiliation { Id = -1, Abbreviation = "null", Name = "(none)" });
        }
    }
}

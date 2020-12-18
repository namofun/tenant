using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelliteSite.NewsModule.Entities;

namespace SatelliteSite.NewsModule
{
    public class NewsEntityConfiguration<TContext> :
        EntityTypeConfigurationSupplier<TContext>,
        IEntityTypeConfiguration<News>
        where TContext : DbContext
    {
        public void Configure(EntityTypeBuilder<News> entity)
        {
            entity.HasKey(n => n.Id);
        }
    }
}

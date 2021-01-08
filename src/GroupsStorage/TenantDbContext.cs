using Microsoft.EntityFrameworkCore;
using Tenant.Entities;

namespace Tenant
{
    public interface ITenantDbContext
    {
        DbSet<Affiliation> Affiliations { get; set; }

        DbSet<Category> Categories { get; set; }
    }
}

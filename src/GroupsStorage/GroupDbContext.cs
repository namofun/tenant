using Microsoft.EntityFrameworkCore;
using Tenant.Entities;

namespace Tenant
{
    public interface IGroupDbContext : ITenantDbContext
    {
        DbSet<GroupTeam> GroupTeams { get; set; }

        DbSet<GroupUser> GroupUsers { get; set; }
    }
}

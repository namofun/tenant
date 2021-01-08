using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SatelliteSite.IdentityModule.Entities;
using SatelliteSite.NewsModule;
using SatelliteSite.NewsModule.Entities;
using SatelliteSite.OjUpdateModule;
using SatelliteSite.OjUpdateModule.Entities;
using Tenant;
using Tenant.Entities;

namespace SatelliteSite
{
    public class DefaultContext :
        IdentityDbContext<AspNetUser, Role, int>,
        ITenantDbContext,
        IGroupDbContext,
        INewsDbContext,
        IStudentDbContext,
        IOJUpdateDbContext
    {
        public DefaultContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<SolveRecord> SolveRecords { get; set; }

        public virtual DbSet<Student> Students { get; set; }

        public virtual DbSet<Class> Classes { get; set; }

        public virtual DbSet<ClassStudent> ClassStudents { get; set; }

        public virtual DbSet<News> News { get; set; }

        public virtual DbSet<Affiliation> Affiliations { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<GroupTeam> GroupTeams { get; set; }

        public virtual DbSet<GroupUser> GroupUsers { get; set; }
    }
}

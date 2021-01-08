using Microsoft.EntityFrameworkCore;
using Tenant.Entities;

namespace Tenant
{
    public interface IStudentDbContext : ITenantDbContext
    {
        DbSet<Student> Students { get; set; }

        DbSet<Class> Classes { get; set; }

        DbSet<ClassStudent> ClassStudents { get; set; }
    }
}

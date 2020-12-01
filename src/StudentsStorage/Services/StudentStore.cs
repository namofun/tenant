using Microsoft.EntityFrameworkCore;
using SatelliteSite.IdentityModule.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Entities;
using Tenant.Models;

namespace Tenant.Services
{
    public class StudentStore<TUser, TContext> : IStudentStore
        where TUser : class, IUser, IUserWithStudent
        where TContext : DbContext
    {
        public TContext Context { get; }
        private DbSet<TUser> Users => Context.Set<TUser>();
        private DbSet<Class> Classes => Context.Set<Class>();
        private DbSet<Student> Students => Context.Set<Student>();
        private DbSet<ClassStudent> ClassStudents => Context.Set<ClassStudent>();
        public StudentStore(TContext context) => Context = context;

        public Task<Class> FindClassAsync(int id)
        {
            return Classes.Where(s => s.Id == id).SingleOrDefaultAsync();
        }

        public Task<Student> FindStudentAsync(int affiliationId, int id)
        {
            return Students.Where(s => s.AffiliationId == affiliationId && s.Id == id).SingleOrDefaultAsync();
        }

        public async Task<IReadOnlyList<IUser>> FindUserByStudentAsync(Student student)
        {
            var query = await Users
                .Where(u => u.AffiliationId == student.AffiliationId && u.StudentId == student.Id)
                .ToListAsync();
            return query;
        }

        public Task<List<OjAccount>> GetRanklistAsync(Affiliation affiliation, RecordType category, int? year)
        {
            return Context.Set<SolveRecord>()
                .Where(s => s.AffiliationId == affiliation.Id)
                .WhereIf(year.HasValue, s => s.Grade == year)
                .Select(p => new OjAccount(p))
                .ToListAsync();
        }

        public Task<List<Class>> ListClassesAsync(Affiliation affiliation)
        {
            return Classes.Where(c => c.AffiliationId == affiliation.Id).ToListAsync();
        }

        public Task<IPagedList<Student>> ListStudentsAsync(Affiliation affiliation, int page, int pageCount)
        {
            var stuQuery =
                from s in Students
                where s.AffiliationId == affiliation.Id
                join u in Users
                    on new { StudentId = (int?)s.Id, AffiliationId = (int?)s.AffiliationId }
                    equals new { u.StudentId, u.AffiliationId }
                into uu from u in uu.DefaultIfEmpty()
                orderby s.Id ascending
                select new Student
                {
                    Id = s.Id,
                    IsVerified = u.StudentVerified,
                    Name = s.Name,
                    Email = u.StudentEmail,
                    UserId = u.Id,
                    UserName = u.UserName,
                    AffiliationId = s.AffiliationId,
                };

            return stuQuery.ToPagedListAsync(page, pageCount);
        }

        private IQueryable<Student> QueryByClass(Class @class) =>
            from cs in ClassStudents
            where cs.ClassId == @class.Id
            join s in Students
                on new { cs.StudentId, @class.AffiliationId }
                equals new { StudentId = s.Id, s.AffiliationId }
            join u in Users
                on new { StudentId = (int?)s.Id, AffiliationId = (int?)s.AffiliationId }
                equals new { u.StudentId, u.AffiliationId }
            into uu from u in uu.DefaultIfEmpty()
            orderby s.Id ascending
            select new Student
            {
                Id = s.Id,
                IsVerified = u.StudentVerified,
                Name = s.Name,
                Email = u.StudentEmail,
                UserId = u.Id,
                UserName = u.UserName,
                AffiliationId = s.AffiliationId,
            };

        public Task<IPagedList<Student>> ListStudentsAsync(Class @class, int page, int pageCount)
        {
            return QueryByClass(@class).ToPagedListAsync(page, pageCount);
        }

        public Task<List<Student>> ListStudentsAsync(Class @class)
        {
            return QueryByClass(@class).ToListAsync();
        }
    }
}

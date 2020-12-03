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

        public async Task<IReadOnlyList<IUser>> FindUserByStudentAsync(Student student)
        {
            return await Users
                .Where(u => u.StudentId == student.Id)
                .ToListAsync();
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
            return Classes
                .Where(c => c.AffiliationId == affiliation.Id)
                .ToListAsync();
        }

        public Task<IPagedList<Student>> ListStudentsAsync(Affiliation affiliation, int page, int pageCount)
        {
            return Students
                .Where(s => s.AffiliationId == affiliation.Id)
                .JoinWithUser(Users)
                .ToPagedListAsync(page, pageCount);
        }

        public Task<IPagedList<Student>> ListStudentsAsync(Class @class, int page, int pageCount)
        {
            return ClassStudents
                .Where(cs => cs.ClassId == @class.Id)
                .Join(Students, cs => cs.StudentId, s => s.Id, (cs, s) => s)
                .JoinWithUser(Users)
                .ToPagedListAsync(page, pageCount);
        }

        public Task<List<Student>> ListStudentsAsync(Class @class)
        {
            return ClassStudents
                .Where(cs => cs.ClassId == @class.Id)
                .Join(Students, cs => cs.StudentId, s => s.Id, (cs, s) => s)
                .JoinWithUser(Users)
                .ToListAsync();
        }

        public Task<Student?> FindStudentAsync(string id)
        {
            return Students
                .Where(s => s.Id == id)
                .SingleOrDefaultAsync()!;
        }

        public Task<Student?> FindStudentAsync(Affiliation affiliation, string rawId)
        {
            return FindStudentAsync($"{affiliation.Id}_{rawId}");
        }

        public Task<int> MergeAsync(Affiliation affiliation, Dictionary<string, string> students)
        {
            return Students.MergeAsync(
                sourceTable: students.Select(s => new { Id = $"{affiliation.Id}_{s.Key.Trim()}", Name = s.Value.Trim(), Aff = affiliation.Id }),
                targetKey: s => s.Id,
                sourceKey: s => s.Id,
                updateExpression: (t, s) => new Student { Name = s.Name },
                insertExpression: s => new Student { Id = s.Id, Name = s.Name, AffiliationId = s.Aff },
                delete: false);
        }
    }
}

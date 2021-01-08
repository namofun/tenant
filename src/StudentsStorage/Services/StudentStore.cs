using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Entities;

namespace Tenant.Services
{
    public class StudentStore<TUser, TContext> : IStudentStore
        where TUser : class, IUserWithStudent
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

        public Task<List<Class>> ListClassesAsync(Affiliation affiliation)
        {
            return Classes
                .Where(c => c.AffiliationId == affiliation.Id)
                .Select(c => new Class { AffiliationId = c.AffiliationId, Count = c.Students.Count, Id = c.Id, Name = c.Name })
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

        public Task<Student?> FindStudentAsync(Affiliation affiliation, string rawId)
        {
            var id = $"{affiliation.Id}_{rawId}";
            return Students.Where(s => s.Id == id).SingleOrDefaultAsync()!;
        }

        public Task<int> MergeAsync(Affiliation affiliation, Dictionary<string, string> students)
        {
            return Students.UpsertAsync(
                sources: students.Select(s => new { Id = $"{affiliation.Id}_{s.Key.Trim()}", Name = s.Value.Trim(), Aff = affiliation.Id }),
                insertExpression: s => new Student { Id = s.Id, Name = s.Name, AffiliationId = s.Aff },
                updateExpression: (t, s) => new Student { Name = s.Name });
        }

        public Task DeleteAsync(Student student)
        {
            Students.Remove(student);
            return Context.SaveChangesAsync();
        }

        public Task DeleteAsync(Class @class)
        {
            Classes.Remove(@class);
            return Context.SaveChangesAsync();
        }

        public Task<Class> FindClassAsync(Affiliation affiliation, int id)
        {
            return Classes
                .Where(s => s.Id == id && s.AffiliationId == affiliation.Id)
                .SingleOrDefaultAsync();
        }

        public async Task<Class> CreateAsync(Affiliation affiliation, string className)
        {
            var e = Classes.Add(new Class
            {
                Name = className,
                AffiliationId = affiliation.Id,
                Affiliation = affiliation,
            });

            await Context.SaveChangesAsync();
            return e.Entity;
        }

        public Task<int> MergeAsync(Class @class, List<string> students)
        {
            return ClassStudents.UpsertAsync(
                sources: students.Select(s => new { ClassId = @class.Id, StudentId = $"{@class.AffiliationId}_{s}" }),
                insertExpression: s => new ClassStudent { StudentId = s.StudentId, ClassId = s.ClassId });
        }

        public async Task<List<string>> CheckExistingStudentsAsync(Affiliation affiliation, List<string> students)
        {
            var prefix = $"{affiliation.Id}_";
            var existing = students.Select(s => $"{affiliation.Id}_{s}").ToList();
            var loaded = await Students.Where(s => existing.Contains(s.Id)).Select(s => s.Id).ToListAsync();
            return loaded.Select(s => s[prefix.Length..]).ToList();
        }

        public async Task<bool> KickAsync(Class @class, Student student)
        {
            var classId = @class.Id;
            var studentId = student.Id;
            int count = await ClassStudents
                .Where(cs => cs.ClassId == classId && cs.StudentId == studentId)
                .BatchDeleteAsync();
            return count > 0;
        }
    }
}

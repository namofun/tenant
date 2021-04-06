using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenant.Entities;

namespace Tenant.Services
{
    public class StudentStore<TUser, TRole, TContext> : IStudentStore, IStudentQueryableStore
        where TUser : class, IUserWithStudent
        where TRole : class, IRole
        where TContext : DbContext
    {
        public TContext Context { get; }
        private DbSet<TUser> Users => Context.Set<TUser>();
        private DbSet<Class> Classes => Context.Set<Class>();
        private DbSet<Student> Students => Context.Set<Student>();
        private DbSet<ClassStudent> ClassStudents => Context.Set<ClassStudent>();
        IQueryable<Student> IStudentQueryableStore.Students => Context.Set<Student>();
        IQueryable<Class> IStudentQueryableStore.Classes => Context.Set<Class>();
        IQueryable<ClassStudent> IStudentQueryableStore.ClassStudents => Context.Set<ClassStudent>();
        IQueryable<IUserWithStudent> IStudentQueryableStore.Users => Context.Set<TUser>();

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

        public Task<List<Class>> ListClassesAsync(Affiliation affiliation, Expression<Func<Class, bool>>? filters = null)
        {
            return Classes
                .Where(c => c.AffiliationId == affiliation.Id)
                .WhereIf(filters != null, filters)
                .Select(c => new Class
                {
                    AffiliationId = c.AffiliationId,
                    Count = c.Students.Count,
                    Id = c.Id,
                    Name = c.Name,
                    CreationTime = c.CreationTime,
                    UserId = c.UserId,
                    UserName = c.UserName,
                })
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

        public async Task<Class> CreateAsync(Affiliation affiliation, string className, int? userId, string? userName)
        {
            var e = Classes.Add(new Class
            {
                Name = className,
                AffiliationId = affiliation.Id,
                CreationTime = DateTimeOffset.Now,
                Affiliation = affiliation,
                UserId = userId,
                UserName = userName,
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

        public async Task<(Affiliation, Student)?> FindStudentAsync(string combinedId)
        {
            var query =
                from s in Students
                where s.Id == combinedId
                join a in Context.Set<Affiliation>() on s.AffiliationId equals a.Id
                select new { s, a };

            var result = await query.SingleOrDefaultAsync();
            if (result == null) return null;
            return (result.a, result.s);
        }

        public Task<List<Class>> ListClassesAsync(IEnumerable<int> affiliationIds, Expression<Func<Class, bool>>? filters = null)
        {
            return Classes
                .WhereIf(affiliationIds != null, c => affiliationIds.Contains(c.AffiliationId))
                .WhereIf(filters != null, filters)
                .Select(c => new Class
                {
                    AffiliationId = c.AffiliationId,
                    Id = c.Id,
                    Name = c.Affiliation.Name + " - " + c.Name,
                    UserId = c.UserId,
                    UserName = c.UserName,
                })
                .ToListAsync();
        }

        public async Task<Class> CloneAsync(Class @class, string className, int? userId, string? userName)
        {
            var e = Classes.Add(new Class
            {
                Name = className,
                AffiliationId = @class.AffiliationId,
                CreationTime = DateTimeOffset.Now,
                UserId = userId,
                UserName = userName,
            });

            await Context.SaveChangesAsync();
            var @new = e.Entity;

            await ClassStudents
                .Where(cs => cs.ClassId == @class.Id)
                .Select(cs => new ClassStudent { ClassId = @new.Id, StudentId = cs.StudentId })
                .BatchInsertIntoAsync(ClassStudents);

            return @new;
        }

        public async Task<IReadOnlyList<IUser>> GetAdministratorsAsync(Affiliation affiliation)
        {
            var claimValue = affiliation.Id.ToString();
            return await Context.Set<IdentityUserClaim<int>>()
                .Where(c => c.ClaimType == "tenant_admin" && c.ClaimValue == claimValue)
                .Join(Users, c => c.UserId, u => u.Id, (c, u) => u)
                .ToListAsync();
        }

        public Task<ILookup<int, string>> GetAdministratorRolesAsync(Affiliation affiliation)
        {
            var claimValue = affiliation.Id.ToString();
            return Context.Set<IdentityUserClaim<int>>()
                .Where(c => c.ClaimType == "tenant_admin" && c.ClaimValue == claimValue)
                .Join(Users, c => c.UserId, u => u.Id, (c, u) => u)
                .Join(Context.Set<IdentityUserRole<int>>(), u => u.Id, ur => ur.UserId, (u, ur) => ur)
                .Join(Context.Set<TRole>(), ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name, r.ShortName })
                .ToLookupAsync(k => k.UserId, v => v.Name);
        }

        public Task<IPagedList<Class>> ListClassesAsync(Affiliation affiliation, int page, int pageCount, Expression<Func<Class, bool>>? filters = null)
        {
            return Classes
                .Where(c => c.AffiliationId == affiliation.Id)
                .WhereIf(filters != null, filters)
                .Select(c => new Class
                {
                    AffiliationId = c.AffiliationId,
                    Count = c.Students.Count,
                    Id = c.Id,
                    Name = c.Name,
                    CreationTime = c.CreationTime,
                    UserId = c.UserId,
                    UserName = c.UserName,
                })
                .ToPagedListAsync(page, pageCount);
        }

        public Task<IPagedList<Class>> ListClassesAsync(IEnumerable<int> affiliationIds, int page, int pageCount, Expression<Func<Class, bool>>? filters = null)
        {
            return Classes
                .WhereIf(affiliationIds != null, c => affiliationIds.Contains(c.AffiliationId))
                .WhereIf(filters != null, filters)
                .Select(c => new Class
                {
                    AffiliationId = c.AffiliationId,
                    Id = c.Id,
                    Name = c.Affiliation.Name + " - " + c.Name,
                    UserId = c.UserId,
                    UserName = c.UserName,
                })
                .ToPagedListAsync(page, pageCount);
        }
    }
}

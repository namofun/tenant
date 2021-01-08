using System.Linq;
using Tenant.Entities;

namespace Tenant.Services
{
    public static class QueryExtensions
    {
        internal static IQueryable<Student> JoinWithUser<TUser>(
            this IQueryable<Student> source,
            IQueryable<TUser> users)
            where TUser : class, IUserWithStudent
        {
            return source
                .OrderBy(s => s.Id)
                .GroupJoin(
                    inner: users,
                    outerKeySelector: s => s.Id,
                    innerKeySelector: u => u.StudentId,
                    resultSelector: (s, uu) => new { s, uu })
                .SelectMany(
                    collectionSelector: a => a.uu.DefaultIfEmpty(),
                    resultSelector: (a, u) => new Student
                    {
                        Id = a.s.Id,
                        IsVerified = u.StudentVerified,
                        Name = a.s.Name,
                        Email = u.StudentEmail,
                        UserId = u.Id,
                        UserName = u.UserName,
                        AffiliationId = a.s.AffiliationId,
                    });
        }
    }
}

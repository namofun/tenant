using System;
using System.Linq;
using Tenant.Entities;

namespace Tenant.Services
{
    /// <summary>
    /// Extensions for tenant tools.
    /// </summary>
    public static class StudentQueryableStoreExtensions
    {
        /// <summary>
        /// Apply the students with user information.
        /// </summary>
        /// <typeparam name="TUser">The type for users with student feature.</typeparam>
        /// <param name="students">The source students queryable.</param>
        /// <param name="users">The user queryable.</param>
        /// <returns>The queryable for <see cref="Student"/>.</returns>
        public static IQueryable<Student> JoinWithUser<TUser>(
            this IQueryable<Student> students,
            IQueryable<TUser> users)
            where TUser : class, IUserWithStudent
            => from s in students
               orderby s.Id ascending
               join u in users on s.Id equals u.StudentId into uu
               from u in uu.DefaultIfEmpty()
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

        /// <summary>
        /// Converts the store as a <see cref="IStudentQueryableStore"/>.
        /// </summary>
        /// <param name="store">The <see cref="IStudentStore"/>.</param>
        /// <returns>The <see cref="IStudentQueryableStore"/>.</returns>
        public static IStudentQueryableStore GetQueryableStore(this IStudentStore store)
            => store as IStudentQueryableStore
                ?? throw new InvalidOperationException("This store is not a IQueryable store.");
    }
}

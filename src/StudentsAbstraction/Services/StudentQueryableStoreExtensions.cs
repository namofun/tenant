﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xylab.Tenant.Entities;

namespace Xylab.Tenant.Services
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

        /// <summary>
        /// List teaching classes via affiliation.
        /// </summary>
        /// <param name="store">The <see cref="IStudentStore"/>.</param>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="filters">The filters on classes.</param>
        /// <returns>The task for fetching teaching classes.</returns>
        public static Task<List<Class>> ListClassesAsync(this IStudentStore store, Affiliation affiliation, int? filters)
            => filters == null
                ? store.ListClassesAsync(affiliation)
                : store.ListClassesAsync(affiliation, c => c.UserId == null || c.UserId == filters);

        /// <summary>
        /// List teaching classes via affiliation.
        /// </summary>
        /// <param name="store">The <see cref="IStudentStore"/>.</param>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="page">The current page.</param>
        /// <param name="pageCount">The count per page.</param>
        /// <param name="filters">The filters on classes.</param>
        /// <returns>The task for fetching teaching classes.</returns>
        public static Task<IPagedList<Class>> ListClassesAsync(this IStudentStore store, Affiliation affiliation, int page, int pageCount, int? filters)
            => filters == null
                ? store.ListClassesAsync(affiliation, page, pageCount)
                : store.ListClassesAsync(affiliation, page, pageCount, c => c.UserId == null || c.UserId == filters);
    }
}

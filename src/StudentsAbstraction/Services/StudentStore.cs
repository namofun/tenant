using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenant.Entities;

namespace Tenant.Services
{
    /// <summary>
    /// The store interface for <see cref="Student"/>.
    /// </summary>
    public interface IStudentStore
    {
        /// <summary>
        /// List students via affiliation.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="page">The current page.</param>
        /// <param name="pageCount">The count per page.</param>
        /// <returns>The task for fetching students.</returns>
        Task<IPagedList<Student>> ListStudentsAsync(Affiliation affiliation, int page, int pageCount);

        /// <summary>
        /// List students via teaching class.
        /// </summary>
        /// <param name="class">The teaching class.</param>
        /// <param name="page">The current page.</param>
        /// <param name="pageCount">The count per page.</param>
        /// <returns>The task for fetching students.</returns>
        Task<IPagedList<Student>> ListStudentsAsync(Class @class, int page, int pageCount);

        /// <summary>
        /// List teaching classes via affiliation.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="filters">The filters on classes.</param>
        /// <returns>The task for fetching teaching classes.</returns>
        Task<List<Class>> ListClassesAsync(Affiliation affiliation, Expression<Func<Class, bool>>? filters = null);

        /// <summary>
        /// List teaching classes via affiliation IDs.
        /// </summary>
        /// <param name="affiliationIds">The affiliation IDs.</param>
        /// <param name="filters">The filters on classes.</param>
        /// <returns>The task for fetching teaching classes.</returns>
        Task<List<Class>> ListClassesAsync(IEnumerable<int> affiliationIds, Expression<Func<Class, bool>>? filters = null);

        /// <summary>
        /// List teaching classes via affiliation.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="page">The current page.</param>
        /// <param name="pageCount">The count per page.</param>
        /// <param name="filters">The filters on classes.</param>
        /// <returns>The task for fetching teaching classes.</returns>
        Task<IPagedList<Class>> ListClassesAsync(Affiliation affiliation, int page, int pageCount, Expression<Func<Class, bool>>? filters = null);

        /// <summary>
        /// List teaching classes via affiliation IDs.
        /// </summary>
        /// <param name="affiliationIds">The affiliation IDs.</param>
        /// <param name="page">The current page.</param>
        /// <param name="pageCount">The count per page.</param>
        /// <param name="filters">The filters on classes.</param>
        /// <returns>The task for fetching teaching classes.</returns>
        Task<IPagedList<Class>> ListClassesAsync(IEnumerable<int> affiliationIds, int page, int pageCount, Expression<Func<Class, bool>>? filters = null);

        /// <summary>
        /// List all students from teaching class.
        /// </summary>
        /// <param name="class">The teaching class.</param>
        /// <returns>The task for fetching students.</returns>
        Task<List<Student>> ListStudentsAsync(Class @class);

        /// <summary>
        /// List all users from student.
        /// </summary>
        /// <param name="student">The student.</param>
        /// <returns>The task for fetching users.</returns>
        Task<IReadOnlyList<IUser>> FindUserByStudentAsync(Student student);

        /// <summary>
        /// Find the teaching class.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="id">The teaching class ID.</param>
        /// <returns>The teaching class.</returns>
        Task<Class> FindClassAsync(Affiliation affiliation, int id);

        /// <summary>
        /// Find the student.
        /// </summary>
        /// <param name="combinedId">The combined student ID.</param>
        /// <returns>The task for finding student.</returns>
        Task<(Affiliation, Student)?> FindStudentAsync(string combinedId);

        /// <summary>
        /// Find the student.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="rawId">The raw student ID.</param>
        /// <returns>The task for finding student.</returns>
        Task<Student?> FindStudentAsync(Affiliation affiliation, string rawId);

        /// <summary>
        /// Batch create or update a student entity.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="students">The dictionary for raw ID to name.</param>
        /// <returns>The task for batch creating student.</returns>
        Task<int> MergeAsync(Affiliation affiliation, Dictionary<string, string> students);

        /// <summary>
        /// Delete one student from store.
        /// </summary>
        /// <param name="student">The student.</param>
        /// <returns>The task for deleting students.</returns>
        Task DeleteAsync(Student student);

        /// <summary>
        /// Delete one class from store.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <returns>The task for deleting classes.</returns>
        Task DeleteAsync(Class @class);

        /// <summary>
        /// Clone one class from store.
        /// </summary>
        /// <param name="class">The source class.</param>
        /// <param name="className">The destination class name.</param>
        /// <param name="userId">The creator user ID.</param>
        /// <param name="userName">The creator user name.</param>
        /// <returns>The task for cloning classes.</returns>
        Task<Class> CloneAsync(Class @class, string className, int? userId, string? userName);

        /// <summary>
        /// Create a teaching class.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="className">The class name.</param>
        /// <param name="userId">The creator user ID.</param>
        /// <param name="userName">The creator user name.</param>
        /// <returns>The task for creating class.</returns>
        Task<Class> CreateAsync(Affiliation affiliation, string className, int? userId, string? userName);

        /// <summary>
        /// Batch add students to the class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="students">The student IDs.</param>
        /// <returns>The task for merging.</returns>
        Task<int> MergeAsync(Class @class, List<string> students);

        /// <summary>
        /// Check those existing students in store.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="students">The student IDs.</param>
        /// <returns>The task for fetching existing ones.</returns>
        Task<List<string>> CheckExistingStudentsAsync(Affiliation affiliation, List<string> students);

        /// <summary>
        /// Remove the certain student from the certain class.
        /// </summary>
        /// <param name="class">The class.</param>
        /// <param name="student">The student.</param>
        /// <returns>Task for checking whether kick succeeded.</returns>
        Task<bool> KickAsync(Class @class, Student student);

        /// <summary>
        /// Lists the administrators by affiliation.
        /// </summary>
        /// <param name="affiliation">The affiliation entity.</param>
        /// <returns>The user-role lookup.</returns>
        Task<IReadOnlyList<IUser>> GetAdministratorsAsync(Affiliation affiliation);

        /// <summary>
        /// Lists the roles of administrators by affiliation.
        /// </summary>
        /// <param name="affiliation">The affiliation entity.</param>
        /// <returns>The role lookup.</returns>
        Task<ILookup<int, string>> GetAdministratorRolesAsync(Affiliation affiliation);

        /// <summary>
        /// Creates a verify code for inviting students.
        /// </summary>
        /// <param name="affiliation">The affiliation entity.</param>
        /// <param name="userId">The user ID of creator.</param>
        /// <returns>The created verify code entity.</returns>
        Task<VerifyCode> CreateVerifyCodeAsync(Affiliation affiliation, int userId);

        /// <summary>
        /// Gets all of the verify codes for this affiliations.
        /// </summary>
        /// <param name="affiliation">The affiliation entity.</param>
        /// <param name="userId">The creator user ID.</param>
        /// <returns>The list of verify codes.</returns>
        Task<IReadOnlyList<VerifyCode>> GetVerifyCodesAsync(Affiliation affiliation, int? userId = null);

        /// <summary>
        /// Redeems the verify code and check whether this is a valid code.
        /// </summary>
        /// <param name="affiliation">The affiliation entity.</param>
        /// <param name="code">The verify code.</param>
        /// <returns>Whether the code exists.</returns>
        Task<bool> RedeemCodeAsync(Affiliation affiliation, string code);

        /// <summary>
        /// Marks the verify code as invalid.
        /// </summary>
        /// <param name="affiliation">The affiliation entity.</param>
        /// <param name="code">The verify code.</param>
        /// <returns>Whether the code exists.</returns>
        Task<bool> InvalidateCodeAsync(Affiliation affiliation, string code);
    }
}

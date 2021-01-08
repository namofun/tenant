using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
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
        /// <returns>The task for fetching teaching classes.</returns>
        Task<List<Class>> ListClassesAsync(Affiliation affiliation);

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
        /// Create a teaching class.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="className">The class name.</param>
        /// <returns>The task for creating class.</returns>
        Task<Class> CreateAsync(Affiliation affiliation, string className);

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
    }
}

using SatelliteSite.IdentityModule.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tenant.Entities;
using Tenant.Models;

namespace Tenant.Services
{
    /// <summary>
    /// The store interface for <see cref="Student"/>.
    /// </summary>
    public interface IStudentStore
    {
        /// <summary>
        /// Get the solving record ranklist.
        /// </summary>
        /// <param name="affiliation">The affiliation.</param>
        /// <param name="category">The record category.</param>
        /// <param name="year">The student grade.</param>
        /// <returns>The task for fetching models.</returns>
        Task<List<OjAccount>> GetRanklistAsync(Affiliation affiliation, RecordType category, int? year = null);

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
        /// Find the student.
        /// </summary>
        /// <param name="affiliationId">The affiliation ID.</param>
        /// <param name="id">The student ID.</param>
        /// <returns>The task for finding student.</returns>
        Task<Student> FindStudentAsync(int affiliationId, int id);

        /// <summary>
        /// Find the teaching class.
        /// </summary>
        /// <param name="id">The teaching class ID.</param>
        /// <returns>The teaching class.</returns>
        Task<Class> FindClassAsync(int id);

        //Task<int> MergeStudentListAsync(List<Student> students);

        //Task<int[]> CheckStudentIdAsync(IEnumerable<int> ids);

        //Task<int> MergeClassStudentAsync(Class @class, IEnumerable<int> studIds);

        //Task<bool> ClassKickAsync(int classId, int studentId);
    }
}

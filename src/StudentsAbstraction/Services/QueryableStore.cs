using System.Linq;
using Xylab.Tenant.Entities;

namespace Xylab.Tenant.Services
{
    /// <summary>
    /// The queryable store for <see cref="Student"/>, <see cref="Class"/> and <see cref="ClassStudent"/>.
    /// </summary>
    public interface IStudentQueryableStore
    {
        /// <summary>
        /// Gets the query navigation for <see cref="Student"/>s.
        /// </summary>
        IQueryable<Student> Students { get; }

        /// <summary>
        /// Gets the query navigation for <see cref="Class"/>es.
        /// </summary>
        IQueryable<Class> Classes { get; }

        /// <summary>
        /// Gets the query navigation for <see cref="ClassStudent"/>s.
        /// </summary>
        IQueryable<ClassStudent> ClassStudents { get; }

        /// <summary>
        /// Gets the query navigation for <see cref="VerifyCode"/>s.
        /// </summary>
        IQueryable<VerifyCode> VerifyCodes { get; }

        /// <summary>
        /// Gets the query navigation for <see cref="IUserWithStudent"/>s.
        /// </summary>
        IQueryable<IUserWithStudent> Users { get; }
    }
}

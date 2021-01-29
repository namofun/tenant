using System.Linq;
using Tenant.Entities;

namespace Tenant.Services
{
    /// <summary>
    /// The queryable store for <see cref="Affiliation"/>.
    /// </summary>
    public interface IAffiliationQueryableStore
    {
        /// <summary>
        /// Gets the query navigation for <see cref="Affiliation"/>s.
        /// </summary>
        IQueryable<Affiliation> Affiliations { get; }
    }

    /// <summary>
    /// The queryable store for <see cref="Category"/>.
    /// </summary>
    public interface ICategoryQueryableStore
    {
        /// <summary>
        /// Gets the query navigation for <see cref="Category"/>s.
        /// </summary>
        IQueryable<Category> Categories { get; }
    }

    /// <summary>
    /// The queryable store for <see cref="GroupTeam"/> and <see cref="GroupUser"/>.
    /// </summary>
    public interface IGroupQueryableStore
    {
        /// <summary>
        /// Gets the query navigation for <see cref="GroupTeam"/>s.
        /// </summary>
        IQueryable<GroupTeam> GroupTeams { get; }

        /// <summary>
        /// Gets the query navigation for <see cref="GroupUser"/>s.
        /// </summary>
        IQueryable<GroupUser> GroupUsers { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xylab.Tenant.Entities;

namespace Xylab.Tenant.Services
{
    /// <summary>
    /// The store interface for <see cref="Affiliation"/>.
    /// </summary>
    public interface IAffiliationStore
    {
        /// <summary>
        /// Create an instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The created entity.</returns>
        Task<Affiliation> CreateAsync(Affiliation entity);

        /// <summary>
        /// Update the instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The update task.</returns>
        Task UpdateAsync(Affiliation entity);

        /// <summary>
        /// Update the instance of entity.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="expression">The update expression.</param>
        /// <returns>The update task.</returns>
        Task UpdateAsync(int id, Expression<Func<Affiliation, Affiliation>> expression);

        /// <summary>
        /// Delete the instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The delete task.</returns>
        Task DeleteAsync(Affiliation entity);

        /// <summary>
        /// Find the entity.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The find task.</returns>
        Task<Affiliation?> FindAsync(int id);

        /// <summary>
        /// Find the entity.
        /// </summary>
        /// <param name="externalId">The external id.</param>
        /// <returns>The find task.</returns>
        Task<Affiliation?> FindAsync(string externalId);

        /// <summary>
        /// List the entities by predicate.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The entity list.</returns>
        Task<List<Affiliation>> ListAsync(Expression<Func<Affiliation, bool>>? predicate = null);
    }
}

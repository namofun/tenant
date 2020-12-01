using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenant.Entities;

namespace Tenant.Services
{
    /// <summary>
    /// The store interface for <see cref="Category"/>.
    /// </summary>
    public interface ICategoryStore
    {
        /// <summary>
        /// Create an instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The created entity.</returns>
        Task<Category> CreateAsync(Category entity);

        /// <summary>
        /// Update the instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The update task.</returns>
        Task UpdateAsync(Category entity);

        /// <summary>
        /// Update the instance of entity.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="expression">The update expression.</param>
        /// <returns>The update task.</returns>
        Task UpdateAsync(int id, Expression<Func<Category, Category>> expression);

        /// <summary>
        /// Delete the instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The delete task.</returns>
        Task DeleteAsync(Category entity);

        /// <summary>
        /// Find the entity.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <returns>The find task.</returns>
        Task<Category> FindAsync(int id);

        /// <summary>
        /// List the entities by predicate.
        /// </summary>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>The entity list.</returns>
        Task<List<Category>> ListAsync(Expression<Func<Category, bool>>? predicate = null);
    }
}

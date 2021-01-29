using System;
using System.Linq;
using System.Security.Claims;
using Tenant.Entities;
using Tenant.Services;

namespace SatelliteSite
{
    /// <summary>
    /// Extensions for tenant tools.
    /// </summary>
    public static class TenantExtensions
    {
        /// <summary>
        /// Whether the current user is a tenent admin.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The result.</returns>
        public static bool IsTenantAdmin(this ClaimsPrincipal user)
            => user.IsInRole("Administrator") || user.HasClaim(c => c.Type == "tenant_admin");

        /// <summary>
        /// Whether the current user is a tenent admin.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="affiliation">The tenant.</param>
        /// <returns>The result.</returns>
        public static bool IsTenantAdmin(this ClaimsPrincipal user, Affiliation? affiliation)
            => affiliation != null
                && (user.IsInRole("Administrator")
                    || user.HasClaim("tenant_admin", affiliation.Id.ToString()));

        /// <summary>
        /// Create a claim for tenant admin.
        /// </summary>
        /// <param name="affiliation">The tenant.</param>
        /// <returns>The created claim.</returns>
        public static Claim CreateClaim(this Affiliation affiliation)
            => new Claim("tenant_admin", affiliation.Id.ToString());

        /// <summary>
        /// Converts the store as a <see cref="IAffiliationQueryableStore"/>.
        /// </summary>
        /// <param name="store">The <see cref="IAffiliationStore"/>.</param>
        /// <returns>The <see cref="IAffiliationQueryableStore"/>.</returns>
        public static IQueryable<Affiliation> GetQueryable(this IAffiliationStore store)
            => (store as IAffiliationQueryableStore)?.Affiliations
                ?? throw new InvalidOperationException("This store is not a IQueryable store.");

        /// <summary>
        /// Converts the store as a <see cref="ICategoryQueryableStore"/>.
        /// </summary>
        /// <param name="store">The <see cref="ICategoryStore"/>.</param>
        /// <returns>The <see cref="ICategoryQueryableStore"/>.</returns>
        public static IQueryable<Category> GetQueryable(this ICategoryStore store)
            => (store as ICategoryQueryableStore)?.Categories
                ?? throw new InvalidOperationException("This store is not a IQueryable store.");

        /// <summary>
        /// Converts the store as a <see cref="IGroupQueryableStore"/>.
        /// </summary>
        /// <param name="store">The <see cref="IGroupStore"/>.</param>
        /// <returns>The <see cref="IGroupQueryableStore"/>.</returns>
        public static IGroupQueryableStore GetQueryableStore(this IGroupStore store)
            => store as IGroupQueryableStore
                ?? throw new InvalidOperationException("This store is not a IQueryable store.");
    }
}

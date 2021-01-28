using System.Security.Claims;
using Tenant.Entities;

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
        public static bool IsTenantAdmin(this ClaimsPrincipal user, Affiliation affiliation)
            => user.IsInRole("Administrator") || user.HasClaim("tenant_admin", affiliation.Id.ToString());

        /// <summary>
        /// Create a claim for tenant admin.
        /// </summary>
        /// <param name="affiliation">The tenant.</param>
        /// <returns>The created claim.</returns>
        public static Claim CreateClaim(this Affiliation affiliation)
            => new Claim("tenant_admin", affiliation.Id.ToString());
    }
}

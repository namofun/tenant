using SatelliteSite.IdentityModule.Services;

namespace Tenant.Entities
{
    /// <summary>
    /// Support student feature with <see cref="IUser"/>.
    /// </summary>
    public interface IUserWithStudent
    {
        /// <summary>
        /// The affiliation ID
        /// </summary>
        int? AffiliationId { get; set; }

        /// <summary>
        /// The student ID
        /// </summary>
        int? StudentId { get; set; }

        /// <summary>
        /// The student email
        /// </summary>
        string StudentEmail { get; set; }

        /// <summary>
        /// Whether this student has been verified
        /// </summary>
        bool StudentVerified { get; set; }
    }
}

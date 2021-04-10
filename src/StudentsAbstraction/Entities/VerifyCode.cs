using System;

namespace Tenant.Entities
{
    /// <summary>
    /// The entity class for verify code.
    /// </summary>
    public class VerifyCode
    {
        /// <summary>
        /// The verify code ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The verify code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// The belonging affiliation ID
        /// </summary>
        public int AffiliationId { get; set; }

        /// <summary>
        /// The user ID of creator
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The count of redemptions
        /// </summary>
        public int RedeemCount { get; set; }

        /// <summary>
        /// Whether this verify code is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// The creation time of this verify code
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

#pragma warning disable CS8618
        /// <summary>
        /// Initialize a verify code entity.
        /// </summary>
        public VerifyCode()
        {
        }
#pragma warning restore CS8618
    }
}

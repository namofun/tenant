namespace Xylab.Tenant.Entities
{
    /// <summary>
    /// The entity class for team affiliations.
    /// </summary>
    public class Affiliation
    {
        /// <summary>
        /// The affiliation ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The formal name of affiliation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The abbreviation of affiliation
        /// </summary>
        public string Abbreviation { get; set; }

        /// <summary>
        /// The student email suffix of affiliation
        /// </summary>
        public string? EmailSuffix { get; set; }

        /// <summary>
        /// The country code like <c>CHN</c>
        /// </summary>
        public string? CountryCode { get; set; } = "CHN";

#pragma warning disable CS8618
        /// <summary>
        /// Initialize an affiliation entity.
        /// </summary>
        public Affiliation()
        {
        }
#pragma warning restore CS8618
    }
}

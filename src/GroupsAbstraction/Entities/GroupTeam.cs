using System;

namespace Tenant.Entities
{
    /// <summary>
    /// The entity class for training teams.
    /// </summary>
    public class GroupTeam
    {
        /// <summary>
        /// The team ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The team name
        /// </summary>
        public string TeamName { get; set; }

        /// <summary>
        /// The affiliation ID
        /// </summary>
        public int AffiliationId { get; set; }

        /// <summary>
        /// The administrator user ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The creation time
        /// </summary>
        public DateTimeOffset Time { get; set; }

        /// <summary>
        /// The navigation to affiliation.
        /// </summary>
        public Affiliation Affiliation { get; set; }

#pragma warning disable CS8618
        /// <summary>
        /// Create an instance of team.
        /// </summary>
        public GroupTeam()
        {
        }
#pragma warning restore CS8618
    }
}

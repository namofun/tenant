namespace Xylab.Tenant.Entities
{
    /// <summary>
    /// The entity class for user in training teams.
    /// </summary>
    public class GroupUser
    {
        /// <summary>
        /// The team ID
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// The user ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Whether the user accepted invitation
        /// </summary>
        public bool? Accepted { get; set; }

        /// <summary>
        /// [Ignore] The user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// [Ignore] The user email
        /// </summary>
        public string UserEmail { get; set; }

#pragma warning disable CS8618
        /// <summary>
        /// Create an instance of team user for inserting result.
        /// </summary>
        public GroupUser()
        {
        }
#pragma warning restore CS8618

        /// <summary>
        /// Create an instance of team user for querying result.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="userId">The user ID.</param>
        /// <param name="accepted">Whether accepted invitation.</param>
        /// <param name="userName">The user name.</param>
        /// <param name="userEmail">The user email.</param>
        public GroupUser(int teamId, int userId, bool? accepted, string userName, string userEmail)
        {
            TeamId = teamId;
            UserId = userId;
            Accepted = accepted;
            UserName = userName;
            UserEmail = userEmail;
        }
    }
}

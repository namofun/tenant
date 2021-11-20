using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Entities;

namespace Tenant.Services
{
    /// <summary>
    /// The store interface for <see cref="GroupTeam"/>.
    /// </summary>
    public interface IGroupStore
    {
        /// <summary>
        /// Update the instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The update task.</returns>
        Task UpdateAsync(GroupTeam entity);

        /// <summary>
        /// Delete the instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The delete task.</returns>
        Task DeleteAsync(GroupTeam entity);

        /// <summary>
        /// Update the instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The update task.</returns>
        Task UpdateAsync(GroupUser entity);

        /// <summary>
        /// Delete the instance of entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The delete task.</returns>
        Task DeleteAsync(GroupUser entity);

        /// <summary>
        /// Find the training team via ID.
        /// </summary>
        /// <param name="id">The team ID.</param>
        /// <returns>The fetching task.</returns>
        Task<GroupTeam?> FindByIdAsync(int id);

        /// <summary>
        /// List the team members.
        /// </summary>
        /// <param name="team">The training team.</param>
        /// <param name="active">Whether to include active users only.</param>
        /// <returns>The list for team members in this training team.</returns>
        Task<List<GroupUser>> ListMembersAsync(GroupTeam team, bool active = false);

        /// <summary>
        /// Check whether this user is in the team.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="team">The team.</param>
        /// <returns>If user exists, returns the entity. Otherwise, null.</returns>
        Task<GroupUser?> IsInTeamAsync(IUser user, GroupTeam team);

        /// <summary>
        /// List teams and members by user.
        /// </summary>
        /// <param name="uid">The user ID.</param>
        /// <param name="active">Whether to include inactive users.</param>
        /// <returns>The lookup task.</returns>
        Task<ILookup<GroupTeam, GroupUser>> ListByUserAsync(int uid, bool active = false);

        /// <summary>
        /// Check whether the user has permission to create teams.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The check task.</returns>
        Task<bool> CheckCreateAsync(IUser user);

        /// <summary>
        /// Check whether the team can add more members.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <returns>The check task.</returns>
        Task<bool> CheckCreateAsync(GroupTeam team);

        /// <summary>
        /// Invite the user to team.
        /// </summary>
        /// <param name="team">The team.</param>
        /// <param name="user">The user.</param>
        /// <returns>The task for adding.</returns>
        Task AddTeamMemberAsync(GroupTeam team, IUser user);

        /// <summary>
        /// Create a training team with name, affiliation and user.
        /// </summary>
        /// <param name="teamName">The team name.</param>
        /// <param name="user">The user.</param>
        /// <param name="affiliation">The affiliation.</param>
        /// <returns>The task for creating team.</returns>
        Task<GroupTeam> CreateAsync(string teamName, IUser user, Affiliation affiliation);
    }
}

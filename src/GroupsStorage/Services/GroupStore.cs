using Microsoft.EntityFrameworkCore;
using SatelliteSite.IdentityModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Entities;

namespace Tenant.Services
{
    public class GroupStoreImpl<TUser, TContext> : IGroupStore
        where TUser : class, IUser
        where TContext : DbContext
    {
        public const int MaxTeams = 10;
        public const int MaxMembers = 5;

        public TContext Context { get; }

        public GroupStoreImpl(TContext context) => Context = context;

        private DbSet<GroupTeam> Teams => Context.Set<GroupTeam>();
        private DbSet<GroupUser> Users => Context.Set<GroupUser>();

        private async Task<T> CreateEntityAsync<T>(T entity) where T : class
        {
            var e = Context.Set<T>().Add(entity);
            await Context.SaveChangesAsync();
            return e.Entity;
        }

        private Task DeleteEntityAsync<T>(T entity) where T : class
        {
            Context.Set<T>().Remove(entity);
            return Context.SaveChangesAsync();
        }

        private Task UpdateEntityAsync<T>(T entity) where T : class
        {
            Context.Set<T>().Update(entity);
            return Context.SaveChangesAsync();
        }

        public Task AddTeamMemberAsync(GroupTeam team, IUser user)
        {
            return CreateEntityAsync(new GroupUser
            {
                TeamId = team.Id,
                UserId = user.Id,
            });
        }

        public async Task<bool> CheckCreateAsync(IUser user)
        {
            var count = await Teams.CountAsync(t => t.UserId == user.Id);
            return count < MaxTeams;
        }

        public async Task<bool> CheckCreateAsync(GroupTeam team)
        {
            var item = await Users.CountAsync(a => a.TeamId == team.Id);
            return item < MaxMembers;
        }

        public async Task<GroupTeam> CreateAsync(string teamName, IUser user, Affiliation affiliation)
        {
            var t = await CreateEntityAsync(new GroupTeam
            {
                AffiliationId = affiliation.Id,
                TeamName = teamName,
                UserId = user.Id,
                Time = DateTimeOffset.Now,
                Affiliation = affiliation,
            });

            await CreateEntityAsync(new GroupUser
            {
                TeamId = t.Id,
                UserId = user.Id,
                Accepted = true,
            });

            return t;
        }

        public Task<GroupTeam> FindByIdAsync(int id)
        {
            return Teams
                .Include(t => t.Affiliation)
                .Where(t => t.Id == id)
                .SingleOrDefaultAsync();
        }

        public Task<GroupUser?> IsInTeamAsync(IUser user, GroupTeam team)
        {
#pragma warning disable CS8619
            return Users
                .Where(tu => tu.UserId == user.Id && tu.TeamId == team.Id)
                .SingleOrDefaultAsync();
#pragma warning restore CS8619
        }

        public async Task<ILookup<GroupTeam, GroupUser>> ListByUserAsync(int uid, bool active = false)
        {
            var query =
                from ttu in Users
                where ttu.UserId == uid && ttu.Accepted == true
                join t in Teams on ttu.TeamId equals t.Id
                join tu in Users on t.Id equals tu.TeamId
                where tu.Accepted == true || !active
                join u in Context.Set<TUser>() on tu.UserId equals u.Id
                select new { t, tuu = new GroupUser(tu.TeamId, tu.UserId, tu.Accepted, u.UserName, u.Email) };
            var results = await query.ToListAsync();
            return results.ToLookup(k => k.t, v => v.tuu);
        }

        public Task<List<GroupUser>> ListMembersAsync(GroupTeam team, bool active = false)
        {
            var query =
                from tu in Users
                where tu.TeamId == team.Id
                where tu.Accepted == true || !active
                join u in Context.Set<TUser>() on tu.UserId equals u.Id
                select new GroupUser(tu.TeamId, tu.UserId, tu.Accepted, u.UserName, u.Email);
            return query.ToListAsync();
        }

        public Task UpdateAsync(GroupTeam entity)
        {
            return UpdateEntityAsync(entity);
        }

        public Task DeleteAsync(GroupTeam entity)
        {
            return DeleteEntityAsync(entity);
        }

        public Task UpdateAsync(GroupUser entity)
        {
            return UpdateEntityAsync(entity);
        }

        public Task DeleteAsync(GroupUser entity)
        {
            return DeleteEntityAsync(entity);
        }
    }
}

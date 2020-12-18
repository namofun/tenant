using Microsoft.EntityFrameworkCore;
using SatelliteSite.NewsModule.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelliteSite.NewsModule.Services
{
    public interface INewsStore
    {
        Task<IEnumerable<(int id, string title)>> ListActiveAsync(int count);

        Task<List<News>> ListAsync();

        Task<News> FindAsync(int newid);

        Task UpdateAsync(News entity);

        Task DeleteAsync(News entity);

        Task<News> CreateAsync(News entity);
    }

    public class NewsStore<TContext> : INewsStore
        where TContext : DbContext
    {
        public TContext Context { get; }

        public DbSet<News> News => Context.Set<News>();

        public NewsStore(TContext context)
        {
            Context = context;
        }

        public Task UpdateAsync(News entity)
        {
            News.Update(entity);
            return Context.SaveChangesAsync();
        }

        public Task DeleteAsync(News entity)
        {
            News.Remove(entity);
            return Context.SaveChangesAsync();
        }

        public async Task<News> CreateAsync(News entity)
        {
            News.Add(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public Task<News> FindAsync(int newid)
        {
            return News.Where(n => n.Id == newid).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<(int id, string title)>> ListActiveAsync(int count)
        {
            var r = await News
                .Where(n => n.Active)
                .OrderByDescending(n => n.Id)
                .Select(n => new { n.Title, n.Id })
                .Take(count)
                .ToListAsync();
            return r.Select(a => (a.Id, a.Title));
        }

        public Task<List<News>> ListAsync()
        {
            return News
                .Select(
                    selector: n => new News
                    {
                        Id = n.Id,
                        Active = n.Active,
                        Title = n.Title,
                        LastUpdate = n.LastUpdate,
                    })
                .ToListAsync();
        }
    }
}

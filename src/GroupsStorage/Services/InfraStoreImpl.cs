using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Xylab.Tenant.Services
{
    public partial class InfraStoreImpl<TContext>
        where TContext : DbContext
    {
        public TContext Context { get; }

        public InfraStoreImpl(TContext context)
        {
            Context = context;
        }

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
    }
}

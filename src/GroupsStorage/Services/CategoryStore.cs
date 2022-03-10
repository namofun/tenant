using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xylab.Tenant.Entities;

namespace Xylab.Tenant.Services
{
    public partial class InfraStoreImpl<TContext> : ICategoryStore, ICategoryQueryableStore
    {
        public IQueryable<Category> Categories => Context.Set<Category>();

        Task<Category> ICategoryStore.CreateAsync(Category entity) => CreateEntityAsync(entity);

        Task ICategoryStore.DeleteAsync(Category entity) => DeleteEntityAsync(entity);

        Task<Category?> ICategoryStore.FindAsync(int id)
        {
            return Categories.Where(c => c.Id == id).SingleOrDefaultAsync();
        }

        Task<List<Category>> ICategoryStore.ListAsync(Expression<Func<Category, bool>>? predicate)
        {
            return Categories.WhereIf(predicate != null, predicate!).ToListAsync();
        }

        Task ICategoryStore.UpdateAsync(Category entity) => UpdateEntityAsync(entity);

        Task ICategoryStore.UpdateAsync(int id, Expression<Func<Category, Category>> expression)
        {
            return Categories.Where(c => c.Id == id).BatchUpdateAsync(expression);
        }
    }
}

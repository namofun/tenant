using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Tenant.Entities;

namespace Tenant.Services
{
    public partial class InfraStoreImpl<TContext> : IAffiliationStore, IAffiliationQueryableStore
    {
        public IQueryable<Affiliation> Affiliations => Context.Set<Affiliation>();

        Task<Affiliation> IAffiliationStore.CreateAsync(Affiliation entity) => CreateEntityAsync(entity);

        Task IAffiliationStore.DeleteAsync(Affiliation entity) => DeleteEntityAsync(entity);

        Task<Affiliation> IAffiliationStore.FindAsync(int id)
        {
            return Affiliations.Where(a => a.Id == id).SingleOrDefaultAsync();
        }

        Task<Affiliation> IAffiliationStore.FindAsync(string externalId)
        {
            return Affiliations.Where(a => a.Abbreviation == externalId).SingleOrDefaultAsync();
        }

        Task<List<Affiliation>> IAffiliationStore.ListAsync(Expression<Func<Affiliation, bool>>? predicate)
        {
            return Affiliations.WhereIf(predicate != null, predicate!).ToListAsync();
        }

        Task IAffiliationStore.UpdateAsync(Affiliation entity) => UpdateEntityAsync(entity);

        Task IAffiliationStore.UpdateAsync(int id, Expression<Func<Affiliation, Affiliation>> expression)
        {
            return Affiliations.Where(a => a.Id == id).BatchUpdateAsync(expression);
        }
    }
}

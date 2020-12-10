using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Entities;

namespace Tenant.Services
{
    public class SolveRecordStore<TContext> : ISolveRecordStore
        where TContext : DbContext
    {
        private TContext Context { get; }

        public SolveRecordStore(TContext context)
        {
            Context = context;
        }

        public Task<List<SolveRecord>> ListAsync(RecordType type)
        {
            return Context.Set<SolveRecord>().Where(s => s.Category == type).ToListAsync();
        }

        public Task UpdateAsync(SolveRecord record)
        {
            Context.Set<SolveRecord>().Add(record);
            return Context.SaveChangesAsync();
        }
    }
}

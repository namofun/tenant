using System.Collections.Generic;
using System.Threading.Tasks;
using Tenant.Entities;

namespace Tenant.Services
{
    /// <summary>
    /// The store interface for <see cref="SolveRecord"/>.
    /// </summary>
    public interface ISolveRecordStore
    {
        /// <summary>
        /// Find all solve record for category.
        /// </summary>
        /// <param name="type">The category.</param>
        /// <returns>The task for solve record list.</returns>
        Task<List<SolveRecord>> ListAsync(RecordType type);

        /// <summary>
        /// Update the solve record.
        /// </summary>
        /// <param name="record">The solve record.</param>
        /// <returns>The task for updating.</returns>
        Task UpdateAsync(SolveRecord record);
    }
}

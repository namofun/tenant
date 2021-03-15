﻿using Microsoft.EntityFrameworkCore;
using SatelliteSite.OjUpdateModule.Entities;
using SatelliteSite.OjUpdateModule.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelliteSite.OjUpdateModule.Services
{
    /// <summary>
    /// The store interface for <see cref="SolveRecord"/>.
    /// </summary>
    public interface ISolveRecordStore
    {
        /// <summary>
        /// List the existing solve records.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="takeCount">The count per page.</param>
        /// <returns>The task for solve record list.</returns>
        Task<IPagedList<SolveRecord>> ListAsync(int currentPage, int takeCount);

        /// <summary>
        /// Find the record by ID.
        /// </summary>
        /// <param name="id">The record ID.</param>
        /// <returns>The task for fetching record.</returns>
        Task<SolveRecord> FindAsync(int id);

        /// <summary>
        /// Find all solve record for category.
        /// </summary>
        /// <param name="type">The category.</param>
        /// <returns>The task for solve record list.</returns>
        Task<List<SolveRecord>> ListAsync(RecordType type);

        /// <summary>
        /// Create the solve records.
        /// </summary>
        /// <param name="records">The solve records.</param>
        /// <returns>The task for creating, returning the minimal record ID.</returns>
        Task<int> CreateAsync(List<SolveRecord> records);

        /// <summary>
        /// Update the solve record.
        /// </summary>
        /// <param name="record">The solve record.</param>
        /// <param name="resultOnly">Whether to update the result only.</param>
        /// <returns>The task for updating.</returns>
        Task UpdateAsync(SolveRecord record, bool resultOnly);

        /// <summary>
        /// Delete the solve record.
        /// </summary>
        /// <param name="record">The solve record.</param>
        /// <returns>The task for deleting.</returns>
        Task DeleteAsync(SolveRecord record);

        /// <summary>
        /// Delete the solve record.
        /// </summary>
        /// <param name="type">The solve record type.</param>
        /// <param name="ids">The target IDs.</param>
        /// <returns>The task for deleting, returning the deleted items.</returns>
        Task<int> DeleteAsync(RecordType type, int[] ids);

        /// <summary>
        /// Find all OJ account model for category and grade.
        /// </summary>
        /// <param name="type">The category.</param>
        /// <param name="grade">The grade</param>
        /// <returns>The task for OJ account list.</returns>
        Task<List<OjAccount>> ListAsync(RecordType type, int? grade);
    }

    internal class SolveRecordStore<TContext> : ISolveRecordStore
        where TContext : DbContext
    {
        private TContext Context { get; }
        
        public SolveRecordStore(TContext context)
        {
            Context = context;
        }

        public Task<List<SolveRecord>> ListAsync(RecordType type)
        {
            return Context.Set<SolveRecord>()
                .Where(s => s.Category == type)
                .AsNoTracking()
                .ToListAsync();
        }

        public Task UpdateAsync(SolveRecord record, bool resultOnly)
        {
            if (resultOnly)
            {
                return Context.Set<SolveRecord>()
                    .Where(r => r.Id == record.Id)
                    .BatchUpdateAsync(_ => new SolveRecord
                    {
                        Result = record.Result
                    });
            }
            else
            {
                return Context.Set<SolveRecord>()
                    .Where(r => r.Id == record.Id)
                    .BatchUpdateAsync(_ => new SolveRecord
                    {
                        Account = record.Account,
                        Category = record.Category,
                        NickName = record.NickName,
                        Grade = record.Grade,
                        Result = null,
                    });
            }
        }

        public Task<List<OjAccount>> ListAsync(RecordType type, int? grade)
        {
            return Context.Set<SolveRecord>()
                .Where(s => s.Category == type)
                .WhereIf(grade.HasValue, s => s.Grade == grade)
                .Select(s => new OjAccount(s.Account, s.NickName, s.Result, s.Grade))
                .ToListAsync();
        }

        public Task<IPagedList<SolveRecord>> ListAsync(int currentPage, int takeCount)
        {
            return Context.Set<SolveRecord>()
                .AsNoTracking()
                .OrderBy(r => r.Id)
                .ToPagedListAsync(currentPage, takeCount);
        }

        public Task<SolveRecord> FindAsync(int id)
        {
            return Context.Set<SolveRecord>()
                .AsNoTracking()
                .Where(s => s.Id == id)
                .SingleOrDefaultAsync();
        }

        public Task DeleteAsync(SolveRecord record)
        {
            Context.Set<SolveRecord>().Remove(record);
            return Context.SaveChangesAsync();
        }

        public async Task<int> CreateAsync(List<SolveRecord> records)
        {
            if (records.Count == 0)
            {
                return await Context.Set<SolveRecord>().CountAsync();
            }

            Context.Set<SolveRecord>().AddRange(records);
            await Context.SaveChangesAsync();
            var minId = records.Min(m => m.Id);

            return await Context.Set<SolveRecord>()
                .Where(s => s.Id <= minId)
                .CountAsync();
        }

        public Task<int> DeleteAsync(RecordType type, int[] ids)
        {
            return Context.Set<SolveRecord>()
                .Where(r => r.Category == type && ids.Contains(r.Id))
                .BatchDeleteAsync();
        }
    }
}

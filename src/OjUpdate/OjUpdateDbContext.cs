using Microsoft.EntityFrameworkCore;
using SatelliteSite.OjUpdateModule.Entities;

namespace SatelliteSite.OjUpdateModule
{
    public interface IOJUpdateDbContext
    {
        DbSet<SolveRecord> SolveRecords { get; set; }
    }
}

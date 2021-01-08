using Microsoft.EntityFrameworkCore;
using SatelliteSite.NewsModule.Entities;

namespace SatelliteSite.NewsModule
{
    public interface INewsDbContext
    {
        DbSet<News> News { get; set; }
    }
}

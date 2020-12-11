using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.Entities;
using SatelliteSite.OjUpdateModule.Services;

namespace SatelliteSite.OjUpdateModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Roles = "Administrator,Teacher")]
    [Route("[area]/[controller]")]
    [AuditPoint(AuditlogType.User)]
    public class ExternalRanklistController : ViewControllerBase
    {
        private ISolveRecordStore Store { get; }

        public ExternalRanklistController(ISolveRecordStore store)
        {
            Store = store;
        }
    }
}

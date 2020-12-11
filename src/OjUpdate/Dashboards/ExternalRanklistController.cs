using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.Entities;
using SatelliteSite.OjUpdateModule.Services;
using System.Threading.Tasks;

namespace SatelliteSite.OjUpdateModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Roles = "Administrator,Teacher")]
    [Route("[area]/[controller]")]
    [AuditPoint(AuditlogType.User)]
    public class ExternalRanklistController : ViewControllerBase
    {
        const int ItemsPerPage = 50;

        private ISolveRecordStore Store { get; }

        public ExternalRanklistController(ISolveRecordStore store)
        {
            Store = store;
        }


        [HttpGet]
        public async Task<IActionResult> List(int page = 1)
        {
            var model = await Store.ListAsync(page, ItemsPerPage);
            return View(model);
        }
    }
}

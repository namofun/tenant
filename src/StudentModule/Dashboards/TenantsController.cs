using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tenant.Services;

namespace SatelliteSite.StudentModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Roles = "Administrator,Teacher")]
    [Route("[area]/affiliation/tenant-switch")]
    public class TenantsController : TenantControllerBase
    {
        private IAffiliationStore Store { get; }
        public TenantsController(IAffiliationStore store) => Store = store;

        [HttpGet]
        public async Task<IActionResult> Switch(string returnUrl)
        {
            var affs = await Store.ListAsync();
            ViewBag.ReturnUrl = returnUrl;
            return View(affs);
        }
    }
}

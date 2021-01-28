using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Tenant.Services;

namespace SatelliteSite.StudentModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Policy = "TenantAdmin")]
    [Route("[area]/affiliations/tenant-switch")]
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Switch(
            [Required] string returnUrl,
            [Required] int tenantId)
        {
            var aff = await Store.FindAsync(tenantId);

            if (aff == null)
            {
                StatusMessage = "Error tenant not found.";
                return RedirectToAction(nameof(Switch), new { returnUrl });
            }
            else
            {
                HttpContext.Session.SetInt32("TenantId", aff.Id);
                return Redirect(returnUrl);
            }
        }
    }
}

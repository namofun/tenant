using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

            if (!User.IsInRole("Administrator"))
            {
                var supportId = User.FindAll("tenant_admin").Select(c => int.Parse(c.Value)).ToHashSet();
                affs = affs.Where(a => supportId.Contains(a.Id)).ToList();
            }

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

            if (!User.IsTenantAdmin(aff))
            {
                StatusMessage = "Error tenant not found.";
                return RedirectToAction(nameof(Switch), new { returnUrl });
            }
            else
            {
                HttpContext.Response.Cookies.Append(_cookieName, aff.Id.ToString());
                return Redirect(returnUrl);
            }
        }
    }
}

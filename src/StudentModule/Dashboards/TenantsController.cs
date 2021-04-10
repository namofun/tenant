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
    public class TenantsController : TenantControllerBase
    {
        [HttpGet("/[area]/affiliations/current")]
        public async Task<IActionResult> Current(
            [FromServices] IStudentStore store)
        {
            ViewBag.Administrators = await store.GetAdministratorsAsync(Affiliation);
            ViewBag.UserRoles = await store.GetAdministratorRolesAsync(Affiliation);
            ViewBag.VerifyCodes = await store.GetVerifyCodesAsync(Affiliation, validOnly: false);
            return View(Affiliation);
        }


        [HttpGet("/[area]/affiliations/tenant-switch")]
        public async Task<IActionResult> Switch(
            string returnUrl,
            [FromServices] IAffiliationStore store)
        {
            var affs = await store.ListAsync();

            if (!User.IsInRole("Administrator"))
            {
                var supportId = User.FindAll("tenant_admin").Select(c => int.Parse(c.Value)).ToHashSet();
                affs = affs.Where(a => supportId.Contains(a.Id)).ToList();
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(affs);
        }


        [HttpPost("/[area]/affiliations/tenant-switch")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Switch(
            [Required] string returnUrl,
            [Required] int tenantId,
            [FromServices] IAffiliationStore store)
        {
            var aff = await store.FindAsync(tenantId);

            if (!User.IsTenantAdmin(aff))
            {
                StatusMessage = "Error tenant not found.";
                return RedirectToAction(nameof(Switch), new { returnUrl });
            }
            else
            {
                HttpContext.Response.Cookies
                    .Append(
                        _cookieName,
                        aff.Id.ToString(),
                        new CookieBuilder
                        {
                            HttpOnly = true,
                            SameSite = SameSiteMode.Lax
                        }
                        .Build(HttpContext));

                return Redirect(returnUrl);
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Services;

namespace SatelliteSite.StudentModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Policy = "TenantAdmin")]
    [AuditPoint(AuditlogType.Affiliation)]
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


        [HttpGet("/[area]/affiliations/current/verify-codes/create")]
        public IActionResult CreateVerifyCode(string returnUrl = null)
        {
            return AskPost(
                title: "Create student verify code",
                message: "You can share verify codes to the unverified students.\n" +
                    "They can use this code to verify their student identity.\n" +
                    "And it is preferred to keep only one active code for one teacher.\n" +
                    "Please invalidate the code since its usage is finished.\n" +
                    "Are you sure to create a student verify code?",
                routeValues: new { returnUrl });
        }


        [HttpPost("/[area]/affiliations/current/verify-codes/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVerifyCode(
            [FromServices] IStudentStore store,
            [FromQuery] string returnUrl = null)
        {
            var userId = int.Parse(User.GetUserId());
            var code = await store.CreateVerifyCodeAsync(Affiliation, userId);
            await HttpContext.AuditAsync("create verify code", Affiliation.Id.ToString(), code.Code);

            return Url.IsLocalUrl(returnUrl)
                ? (IActionResult)Redirect(returnUrl)
                : RedirectToAction(nameof(Current));
        }
    }
}

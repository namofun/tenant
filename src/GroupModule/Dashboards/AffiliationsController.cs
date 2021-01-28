using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using SatelliteSite.GroupModule.Models;
using System.Threading.Tasks;
using Tenant.Entities;
using Tenant.Services;

namespace SatelliteSite.GroupModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Roles = "Administrator")]
    [Route("[area]/[controller]")]
    [AuditPoint(AuditlogType.TeamAffiliation)]
    public class AffiliationsController : ViewControllerBase
    {
        private IAffiliationStore Store { get; }
        private IUserManager UserManager { get; }
        public AffiliationsController(IAffiliationStore tm, IUserManager um)
            => (Store, UserManager) = (tm, um);


        [HttpGet]
        public async Task<IActionResult> List()
        {
            return View(await Store.ListAsync());
        }


        [HttpGet("{affid}")]
        public async Task<IActionResult> Detail(int affid)
        {
            var aff = await Store.FindAsync(affid);
            if (aff == null) return NotFound();
            var claim = aff.CreateClaim();
            ViewBag.Administrators = await UserManager.GetUsersForClaimAsync(claim);
            return View(aff);
        }


        [HttpGet("[action]")]
        public IActionResult Add()
        {
            return View("Edit", new AffiliationModel());
        }


        [HttpGet("{affid}/[action]")]
        public async Task<IActionResult> Edit(int affid)
        {
            var aff = await Store.FindAsync(affid);
            if (aff == null) return NotFound();

            return View(new AffiliationModel
            {
                Abbreviation = aff.Abbreviation,
                CountryCode = aff.CountryCode,
                Id = aff.Id,
                Name = aff.Name,
                EmailSuffix = aff.EmailSuffix,
            });
        }


        private async Task SolveLogo(IFormFile logo, string extid)
        {
            if (logo != null && logo.FileName.EndsWith(".png"))
            {
                try
                {
                    var fp = HttpContext.RequestServices.GetRequiredService<IWwwrootFileProvider>();
                    using var fileContent = logo.OpenReadStream();
                    await fp.WriteStreamAsync($"images/affiliations/{extid}.png", fileContent);
                }
                catch
                {
                    StatusMessage = "Error, logo upload failed!";
                }
            }
            else if (logo != null)
            {
                StatusMessage = "Error, logo should be png!";
            }
        }


        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AffiliationModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var aff = await Store.FindAsync(model.Id.Value);
            if (aff != null)
            {
                StatusMessage = "Error affiliation ID already exists. No changes were made.";
                return RedirectToAction(nameof(Detail), new { affid = aff.Id });
            }

            if (string.IsNullOrWhiteSpace(model.EmailSuffix))
                model.EmailSuffix = null;

            var e = await Store.CreateAsync(new Affiliation
            {
                Id = model.Id.Value,
                Abbreviation = model.Abbreviation,
                CountryCode = model.CountryCode,
                Name = model.Name,
                EmailSuffix = model.EmailSuffix,
            });

            await SolveLogo(model.Logo, model.Abbreviation);
            await HttpContext.AuditAsync("created", $"{e.Id}", "as " + e.Abbreviation);
            return RedirectToAction(nameof(Detail), new { affid = e.Id });
        }


        [HttpPost("{affid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int affid, AffiliationModel model)
        {
            var aff = await Store.FindAsync(affid);
            if (aff == null) return NotFound();

            if (!ModelState.IsValid) return View(model);
            if (string.IsNullOrWhiteSpace(model.EmailSuffix))
                model.EmailSuffix = null;

            aff.Abbreviation = model.Abbreviation;
            aff.Name = model.Name;
            aff.CountryCode = model.CountryCode;
            aff.EmailSuffix = model.EmailSuffix;
            await Store.UpdateAsync(aff);

            await SolveLogo(model.Logo, model.Abbreviation);
            await HttpContext.AuditAsync("updated", $"{affid}", "as " + model.Abbreviation);
            return RedirectToAction(nameof(Detail), new { affid });
        }


        [HttpGet("{affid}/[action]")]
        public async Task<IActionResult> Delete(int affid)
        {
            var desc = await Store.FindAsync(affid);
            if (desc == null) return NotFound();

            return AskPost(
                title: $"Delete team affiliation {desc.Abbreviation} - \"{desc.Name}\"",
                message: $"You're about to delete team affiliation {desc.Abbreviation} - \"{desc.Name}\".\n" +
                    "Are you sure?",
                area: "Dashboard", controller: "Affiliations", action: "Delete",
                type: BootstrapColor.danger);
        }


        [HttpPost("{affid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int affid, int inajax)
        {
            var desc = await Store.FindAsync(affid);
            if (desc == null) return NotFound();

            try
            {
                await Store.DeleteAsync(desc);
                StatusMessage = $"Team affiliation {desc.Abbreviation} deleted successfully.";
                await HttpContext.AuditAsync("deleted", $"{affid}");
                return RedirectToAction(nameof(List));
            }
            catch
            {
                StatusMessage = $"Error deleting team affiliation {desc.Abbreviation}, foreign key constraints failed.";
                return RedirectToAction(nameof(Detail), new { affid });
            }
        }


        [HttpGet("{affid}/[action]/{userName?}")]
        public async Task<IActionResult> TestUser(int affid, string userName)
        {
            if (userName != null)
            {
                var user = await UserManager.FindByNameAsync(userName);
                if (user == null)
                    return Content("No such user.", "text/html");
                return Content("", "text/html");
            }
            else
            {
                return Content("Please enter the username.", "text/html");
            }
        }


        [HttpGet("{affid}/[action]")]
        public async Task<IActionResult> Assign(int affid)
        {
            var desc = await Store.FindAsync(affid);
            if (desc == null) return NotFound();

            return Window(new TenantAssignModel());
        }


        [HttpPost("{affid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int affid, TenantAssignModel model)
        {
            var desc = await Store.FindAsync(affid);
            if (desc == null) return NotFound();

            var user = await UserManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                StatusMessage = "Error user not found.";
            }
            else
            {
                var claim = desc.CreateClaim();
                await UserManager.RemoveClaimAsync(user, claim);
                await UserManager.AddClaimAsync(user, claim);

                StatusMessage = $"Tenant administrator role of user {user.UserName} assigned.";
                await HttpContext.AuditAsync("assigned admin", claim.Value, $"u{user.Id}");
            }

            return RedirectToAction(nameof(Detail));
        }


        [HttpGet("{affid}/[action]/{userid}")]
        public async Task<IActionResult> Unassign(int affid, int userid)
        {
            var desc = await Store.FindAsync(affid);
            if (desc == null) return NotFound();

            var user = await UserManager.FindByIdAsync(userid);
            if (user == null) return NotFound();

            return AskPost(
                title: "Unassign jury",
                message: $"Do you want to unassign tenant administrator {user.UserName} (u{userid})?",
                area: "Dashboard", controller: "Affiliations", action: "Unassign",
                routeValues: new { userid, affid },
                type: BootstrapColor.danger);
        }


        [HttpPost("{affid}/[action]/{userid}")]
        [ValidateAntiForgeryToken]
        [ActionName("Unassign")]
        public async Task<IActionResult> UnassignConfirmation(int affid, int userid)
        {
            var desc = await Store.FindAsync(affid);
            if (desc == null) return NotFound();

            var user = await UserManager.FindByIdAsync(userid);
            if (user == null) return NotFound();

            var claim = desc.CreateClaim();
            await UserManager.RemoveClaimAsync(user, claim);

            StatusMessage = $"Tenant administrator role of user {user.UserName} unassigned.";
            await HttpContext.AuditAsync("unassigned admin", claim.Value, $"u{userid}");
            return RedirectToAction(nameof(Detail));
        }
    }
}

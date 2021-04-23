using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.StudentModule.Models;
using System.Threading.Tasks;
using Tenant.Services;

namespace SatelliteSite.StudentModule.Dashboards
{
    [Area("Dashboard")]
    [Authorize(Roles = "Administrator")]
    [Route("[area]/affiliations")]
    [AuditPoint(AuditlogType.TeamAffiliation)]
    public class Affiliations2Controller : ViewControllerBase
    {
        private IAffiliationStore Store { get; }
        private IUserManager UserManager { get; }
        public Affiliations2Controller(IAffiliationStore tm, IUserManager um)
            => (Store, UserManager) = (tm, um);


        [HttpGet("[action]/{userName?}")]
        public async Task<IActionResult> TestUser(string userName)
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

            return RedirectToAction("Detail", "Affiliations", new { area = "Dashboard", affid });
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
            return RedirectToAction("Detail", "Affiliations", new { area = "Dashboard", affid });
        }
    }
}

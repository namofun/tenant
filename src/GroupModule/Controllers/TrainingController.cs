using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SatelliteSite.GroupModule.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xylab.Tenant.Entities;
using Xylab.Tenant.Services;

namespace SatelliteSite.GroupModule.Controllers
{
    [Area("Tenant")]
    [Authorize]
    [Route("teams")]
    public class TrainingController : ViewControllerBase
    {
        private IUserManager UserManager { get; }
        private IGroupStore Store { get; }
        private IAffiliationStore Affiliations { get; }
        private new IUser User { get; set; }

        public TrainingController(IUserManager um, IGroupStore store, IAffiliationStore affs)
        {
            UserManager = um;
            Store = store;
            Affiliations = affs;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var user = await UserManager.GetUserAsync(base.User);
            ViewBag.User = User = user ?? throw new ApplicationException($"Unable to load user with ID '{base.User.GetUserId()}'.");
            await base.OnActionExecutionAsync(context, next);
        }


        [HttpGet]
        public async Task<IActionResult> List()
        {
            return View(await Store.ListByUserAsync(User.Id));
        }


        [HttpGet("{teamid}")]
        public async Task<IActionResult> Detail(int teamid)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null) return NotFound();
            team.Users = await Store.ListMembersAsync(team);
            return View(team);
        }


        [HttpGet("{teamid}/[action]")]
        public async Task<IActionResult> Edit(int teamid)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null || team.UserId != User.Id) return NotFound();
            ViewBag.Affiliations = await Affiliations.ListAsync();
            team.Users = await Store.ListMembersAsync(team);
            return View(team);
        }


        [HttpPost("{teamid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int teamid, GroupTeam model)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null || team.UserId != User.Id) return NotFound();

            var aff = await Affiliations.FindAsync(model.AffiliationId);
            if (aff == null)
            {
                StatusMessage = "Error affiliation not found.";
                return RedirectToAction(nameof(Edit));
            }

            if (string.IsNullOrEmpty(model.TeamName))
            {
                StatusMessage = "Error team name is empty.";
                return RedirectToAction(nameof(Edit));
            }

            team.Affiliation = aff;
            team.AffiliationId = aff.Id;
            team.TeamName = model.TeamName;
            await Store.UpdateAsync(team);
            StatusMessage = "Team info updated.";
            return RedirectToAction(nameof(Edit));
        }


        [HttpGet("[action]")]
        public async Task<IActionResult> Create()
        {
            if (!await Store.CheckCreateAsync(User))
            {
                return Message(
                    title: "Create team",
                    message: "Team count limit exceeded.",
                    type: BootstrapColor.danger);
            }
            else
            {
                return Window(new CreateTeamModel
                {
                    Affiliations = await Affiliations.ListAsync(),
                });
            }
        }


        [HttpPost("[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamModel model)
        {
            if (!await Store.CheckCreateAsync(User))
            {
                StatusMessage = "Error max team count exceeded.";
                return RedirectToAction(nameof(List));
            }

            var affiliation = await Affiliations.FindAsync(model.AffiliationId);
            if (affiliation == null)
            {
                StatusMessage = "Error no such affiliation.";
                return RedirectToAction(nameof(List));
            }

            var team = await Store.CreateAsync(model.TeamName, User, affiliation);
            return RedirectToAction(nameof(Detail), new { teamid = team.Id });
        }


        [HttpPost("{teamid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Invite(
            int teamid,
            [FromForm, Required] string username)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null || team.UserId != User.Id) return NotFound();

            var user = await UserManager.FindByNameAsync(username);

            if (user == null)
            {
                StatusMessage = "Error user not found.";
                return RedirectToAction(nameof(Edit));
            }

            if (await Store.IsInTeamAsync(user, team) != null)
            {
                StatusMessage = "Error team member already invited.";
                return RedirectToAction(nameof(Edit));
            }

            if (!await Store.CheckCreateAsync(team))
            {
                StatusMessage = "Error team member count limitation exceeded.";
                return RedirectToAction(nameof(Edit));
            }

            await Store.AddTeamMemberAsync(team, user);
            StatusMessage = "Invitition sent. The invitee should open this team page to accept your invitation.";

            return RedirectToAction(nameof(Edit));
        }


        [HttpPost("{teamid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int teamid)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null || team.UserId == User.Id) return NotFound();
            var user = await Store.IsInTeamAsync(User, team);
            if (user == null) return NotFound();

            user.Accepted = true;
            await Store.UpdateAsync(user);
            StatusMessage = "Team invitation accepted.";
            return RedirectToAction(nameof(Detail));
        }


        [HttpPost("{teamid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int teamid)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null || team.UserId == User.Id) return NotFound();
            var user = await Store.IsInTeamAsync(User, team);
            if (user == null) return NotFound();

            user.Accepted = false;
            await Store.UpdateAsync(user);
            StatusMessage = "Team invitation rejected.";
            return RedirectToAction(nameof(Detail));
        }


        [HttpGet("{teamid}/[action]")]
        public async Task<IActionResult> Dismiss(int teamid)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null || team.UserId != User.Id) return NotFound();
            return AskPost(
                title: "Dismiss team",
                message: $"Are you sure to dismiss team {team.TeamName}?",
                area: "Tenant", controller: "Training", action: "Dismiss");
        }


        [HttpPost("{teamid}/[action]")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dismiss(int teamid, bool post = true)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null || team.UserId != User.Id) return NotFound();
            await Store.DeleteAsync(team);
            StatusMessage = "Team dismissed.";
            return RedirectToAction(nameof(List));
        }


        [HttpGet("{teamid}/[action]/{username}")]
        public async Task<IActionResult> Delete(int teamid, string username)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null || team.UserId != User.Id) return NotFound();

            var user = await UserManager.FindByNameAsync(username);
            if (user == null) return NotFound();
            
            if (user.Id == User.Id)
                return Message("Delete team member", "You can't remove yourself out of team.", BootstrapColor.warning);

            var tu = await Store.IsInTeamAsync(user, team);
            if (tu == null) return NotFound();

            return AskPost(
                title: "Delete team member",
                message: $"Are you sure to remove {username} out of this team?",
                area: "Tenant", controller: "Training", action: "Delete");
        }


        [HttpPost("{teamid}/[action]/{username}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string username, int teamid)
        {
            var team = await Store.FindByIdAsync(teamid);
            if (team == null || team.UserId != User.Id) return NotFound();

            var user = await UserManager.FindByNameAsync(username);
            if (user == null) return NotFound();

            if (user.Id == User.Id)
            {
                StatusMessage = "You can't remove yourself out of team.";
                return RedirectToAction(nameof(Edit));
            }

            var tu = await Store.IsInTeamAsync(user, team);
            if (tu == null) return NotFound();
            await Store.DeleteAsync(tu);
            StatusMessage = "Team member deleted.";
            return RedirectToAction(nameof(Edit));
        }
    }
}

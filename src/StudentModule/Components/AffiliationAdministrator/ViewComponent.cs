using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Xylab.Tenant.Entities;

namespace SatelliteSite.StudentModule.Components.AffiliationAdministrator
{
    public class AffiliationAdministratorViewComponent : ViewComponent
    {
        private readonly IUserManager _userManager;

        public AffiliationAdministratorViewComponent(IUserManager userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var aff = (Affiliation)ViewData.Model;
            var claim = aff.CreateClaim();
            var model = await _userManager.GetUsersForClaimAsync(claim);
            return View("Default", model);
        }
    }
}

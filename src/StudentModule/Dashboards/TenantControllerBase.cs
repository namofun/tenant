using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xylab.Tenant.Entities;
using Xylab.Tenant.Services;

namespace SatelliteSite.StudentModule.Dashboards
{
    public abstract class TenantControllerBase : ViewControllerBase
    {
        private Affiliation _affiliation;
        protected const string _cookieName = ".AspNet.Tenant.CurrentTenant";

        protected Affiliation Affiliation => _affiliation;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller != this) throw new NotImplementedException();

            if (HttpContext.Request.Cookies.TryGetValue(_cookieName, out var cookieValue)
                && int.TryParse(cookieValue, out int tenantId))
            {
                var service = HttpContext.RequestServices.GetRequiredService<IAffiliationStore>();
                _affiliation = await service.FindAsync(tenantId);
            }

            var cad = (ControllerActionDescriptor)context.ActionDescriptor;
            var isSwitcher = cad.ControllerName == "Tenants" && cad.ActionName == "Switch";
            if (!isSwitcher && !User.IsTenantAdmin(_affiliation))
            {
                var returnUrl = context.HttpContext.Request.Path.Value;
                context.Result = RedirectToAction("Switch", "Tenants", new { returnUrl });
            }
            else
            {
                ViewBag.Affiliation = Affiliation;
                await base.OnActionExecutionAsync(context, next);
            }
        }
    }
}

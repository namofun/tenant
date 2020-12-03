﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Tenant.Entities;
using Tenant.Services;

namespace SatelliteSite.StudentModule.Dashboards
{
    public class TenantControllerBase : ViewControllerBase
    {
        private Affiliation _affiliation;
        private const string _cookieName = "TenantId";

        protected Affiliation Affiliation => _affiliation;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller != this) throw new NotImplementedException();

            if (Request.Cookies.TryGetValue(_cookieName, out var cookieValue) &&
                int.TryParse(cookieValue, out int tenantId))
            {
                var service = HttpContext.RequestServices.GetRequiredService<IAffiliationStore>();
                _affiliation = await service.FindAsync(tenantId);
                // TODO: some permission check here.
            }

            var cad = (ControllerActionDescriptor)context.ActionDescriptor;
            var isSwitcher = cad.ControllerName == "Tenants" && cad.ActionName == "Switch";
            if (_affiliation == null && !isSwitcher)
            {
                var returnUrl = context.HttpContext.Request.Path.Value;
                context.Result = RedirectToAction("Switch", "Tenant", new { returnUrl });
            }
            else
            {
                ViewBag.Affiliation = Affiliation;
                await base.OnActionExecutionAsync(context, next);
            }
        }
    }
}

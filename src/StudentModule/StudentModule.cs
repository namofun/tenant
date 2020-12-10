﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SatelliteSite.IdentityModule.Entities;
using Tenant.Entities;
using Tenant.Services;

namespace SatelliteSite.StudentModule
{
    public class StudentModule<TUser, TRole, TContext> : AbstractModule
        where TUser : User, IUserWithStudent
        where TRole : Role
        where TContext : IdentityDbContext<TUser, TRole, int>
    {
        public override string Area => "Tenant";

        public override void Initialize()
        {
        }

        public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbModelSupplier<TContext, StudentEntityConfiguration<TUser, TContext>>();
            services.AddScoped<IStudentStore, StudentStore<TUser, TContext>>();

            if (configuration.GetValue<bool>("EnableOjUpdate"))
            {
                services.AddOjUpdateService<SolveRecordStore<TContext>>();
                AvailabilityModelAttribute.Enabled = true;
            }
        }

        public override void RegisterEndpoints(IEndpointBuilder endpoints)
        {
            endpoints.MapControllers();
        }

        public override void RegisterMenu(IMenuContributor menus)
        {
            menus.Submenu(MenuNameDefaults.DashboardUsers, menu =>
            {
                menu.HasEntry(250)
                    .HasLink("Dashboard", "Students", "List")
                    .HasTitle(string.Empty, "Students")
                    .RequireRoles("Administrator");

                menu.HasEntry(251)
                    .HasLink("Dashboard", "Classes", "List")
                    .HasTitle(string.Empty, "Student Groups")
                    .RequireRoles("Administrator");
            });
        }
    }
}

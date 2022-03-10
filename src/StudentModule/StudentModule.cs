using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SatelliteSite;
using SatelliteSite.IdentityModule.Entities;
using SatelliteSite.StudentModule.Components.AffiliationAdministrator;
using System;
using Xylab.Tenant.Entities;
using Xylab.Tenant.Services;

[assembly: RoleDefinition(16, "Student", "student", "Verified Student")]

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
            services.AddScoped<IStudentStore, StudentStore<TUser, TRole, TContext>>();
            services.AddScoped<IUserClaimsProvider, StudentTenantClaimsProvider>();
            services.AddStudentEmailTokenProvider<TUser>();
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
                    .RequireThat(ctx => ctx.HttpContext.User.IsTenantAdmin());

                menu.HasEntry(251)
                    .HasLink("Dashboard", "Classes", "List")
                    .HasTitle(string.Empty, "Student Groups")
                    .RequireThat(ctx => ctx.HttpContext.User.IsTenantAdmin());
            });

            menus.Menu(MenuNameDefaults.DashboardNavbar, menu =>
            {
                menu.HasSubmenu(399, menu =>
                {
                    menu.HasLink("#")
                        .HasTitle("fas fa-graduation-cap", "Schools")
                        .RequireThat(c => c.HttpContext.User.IsTenantAdminOnly())
                        .ActiveWhenController("Students,Classes");

                    menu.HasEntry(0)
                        .HasLink("Dashboard", "Students", "List")
                        .HasTitle("fas fa-user-graduate fa-fw", "students");

                    menu.HasEntry(1)
                        .HasLink("Dashboard", "Classes", "List")
                        .HasTitle("fas fa-school fa-fw", "student groups");
                });
            });

            menus.Menu(IdentityModule.ExtensionPointDefaults.UserDetailMenu, menu =>
            {
                menu.HasEntry(100)
                    .HasLink("Tenant", "StudentVerify", "Main")
                    .HasTitle("info", "Student verify")
                    .RequireThat(c => c.HttpContext.User.GetUserName() == ((IUser)c.ViewData["User"]).UserName);
            });

            menus.Component("Component_AffiliationAttach")
                .HasComponent<AffiliationAdministratorViewComponent>(0);
        }
    }
}

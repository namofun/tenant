using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tenant.Entities;
using Tenant.Services;

namespace SatelliteSite.GroupModule
{
    public class GroupModule<TContext> : AbstractModule
        where TContext : DbContext
    {
        public override string Area => "Training";

        public override void Initialize()
        {
        }

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<InfraStoreImpl<TContext>>();
            services.AddScoped<IAffiliationStore>(s => s.GetRequiredService<InfraStoreImpl<TContext>>());
            services.AddScoped<ICategoryStore>(s => s.GetRequiredService<InfraStoreImpl<TContext>>());
            services.AddDbModelSupplier<TContext, InfraEntityConfiguration<TContext>>();
        }

        public override void RegisterEndpoints(IEndpointBuilder endpoints)
        {
            endpoints.MapControllers();
        }

        public override void RegisterMenu(IMenuContributor menus)
        {
            menus.Submenu(MenuNameDefaults.DashboardUsers, menu =>
            {
                menu.HasEntry(200)
                    .HasLink("Dashboard", "Affiliations", "List")
                    .HasTitle(string.Empty, "Team Affiliations")
                    .RequireRoles("Administrator");

                menu.HasEntry(201)
                    .HasLink("Dashboard", "Categories", "List")
                    .HasTitle(string.Empty, "Team Categories")
                    .RequireRoles("Administrator");
            });
        }
    }
}

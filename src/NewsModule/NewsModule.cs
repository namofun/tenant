using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SatelliteSite.NewsModule.Services;

namespace SatelliteSite.NewsModule
{
    public class NewsModule<TContext> : AbstractModule
        where TContext : DbContext
    {
        public override string Area => "Tenant";

        public override void Initialize()
        {
        }

        public override void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<INewsStore, NewsStore<TContext>>();
            services.AddDbModelSupplier<TContext, NewsEntityConfiguration<TContext>>();
        }

        public override void RegisterEndpoints(IEndpointBuilder endpoints)
        {
            endpoints.MapControllers();
        }

        public override void RegisterMenu(IMenuContributor menus)
        {
            menus.Submenu(MenuNameDefaults.DashboardUsers, menu =>
            {
                menu.HasEntry(-100)
                    .HasLink("Dashboard", "News", "List")
                    .HasTitle(string.Empty, "News")
                    .RequireRoles("Administrator,Teacher");
            });

            menus.Menu(MenuNameDefaults.DashboardNavbar, menu =>
            {
                menu.HasEntry(800)
                    .HasLink("Dashboard", "News", "List")
                    .HasTitle("fab fa-markdown", "News")
                    .RequireRoles("Administrator,Teacher");
            });
        }
    }
}

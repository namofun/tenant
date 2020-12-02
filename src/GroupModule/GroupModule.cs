using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tenant.Entities;
using Tenant.Services;

namespace SatelliteSite.GroupModule
{
    public class GroupModule<TContext> : AbstractModule
        where TContext : DbContext
    {
        public override string Area => "Tenant";

        public override void Initialize()
        {
        }

        public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<InfraStoreImpl<TContext>>();
            services.AddScoped<IAffiliationStore>(s => s.GetRequiredService<InfraStoreImpl<TContext>>());
            services.AddScoped<ICategoryStore>(s => s.GetRequiredService<InfraStoreImpl<TContext>>());
            services.AddDbModelSupplier<TContext, InfraEntityConfiguration<TContext>>();

            if (configuration.GetValue<bool>("EnableTrainingTeam"))
            {
                Type userType = TrainingTeamEntityConfiguration<TContext>.UserType;
                if (userType == null)
                    throw new InvalidOperationException(
                        $"\"{typeof(TContext).Name}\" doesn't inherit from \"IdentityDbContext<,,>\".");

                var newType = typeof(GroupStoreImpl<,>);
                newType = newType.MakeGenericType(userType, typeof(TContext));
                services.Add(ServiceDescriptor.Scoped(typeof(IGroupStore), newType));
                services.AddDbModelSupplier<TContext, TrainingTeamEntityConfiguration<TContext>>();
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

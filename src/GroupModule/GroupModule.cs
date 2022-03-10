using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xylab.Tenant.Entities;
using Xylab.Tenant.Services;

namespace SatelliteSite.GroupModule
{
    public class GroupModule<TContext> : AbstractModule, IAuthorizationPolicyRegistry
        where TContext : DbContext
    {
        public override string Area => "Tenant";

        public override void Initialize()
        {
        }

        public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<InfraStoreImpl<TContext>>();
            services.AddScopedUpcast<IAffiliationStore, InfraStoreImpl<TContext>>();
            services.AddScopedUpcast<ICategoryStore, InfraStoreImpl<TContext>>();
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
            }
            else
            {
                services.AddSingleton(
                    new FeatureAvailabilityConvention(
                        false,
                        typeof(Controllers.TrainingController)));
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

            menus.Component(TenantDefaults.AffiliationAttach);
        }

        public void RegisterPolicies(IAuthorizationPolicyContainer container)
        {
            container.AddPolicy2("HasDashboard", b => b.AcceptClaim("tenant_admin"));
            container.AddPolicy("TenantAdmin", b => b.RequireAssertion(c => c.User.IsTenantAdmin()));
        }
    }
}

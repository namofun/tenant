using SatelliteSite;
using System;
using Tenant.OjUpdate;
using Tenant.Services;

[assembly: ConfigurationItem(0, "Tenant", "oj_Codeforces_update_time", typeof(DateTimeOffset?), null!, "Last update time of Codeforces.", IsPublic = false)]
[assembly: ConfigurationItem(1, "Tenant", "oj_Vjudge_update_time", typeof(DateTimeOffset?), null!, "Last update time of Vjudge.", IsPublic = false)]
[assembly: ConfigurationItem(2, "Tenant", "oj_Hdoj_update_time", typeof(DateTimeOffset?), null!, "Last update time of HDOJ.", IsPublic = false)]

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// The service collection extension methods for external OJ updating.
    /// </summary>
    public static class OjUpdateServiceCollectionExtensions
    {
        /// <summary>
        /// Add external OJ update service to dependency injection.
        /// </summary>
        /// <typeparam name="TStore">The store implementation type.</typeparam>
        /// <param name="services">The service collection.</param>
        /// <param name="sleepLength">The sleep length.</param>
        /// <returns>The service collection to chain operations.</returns>
        public static IServiceCollection AddOjUpdateService<TStore>(
            this IServiceCollection services,
            int sleepLength = 24 * 7 * 60)
            where TStore : class, ISolveRecordStore
        {
            OjUpdateService.SleepLength = sleepLength;
            services.AddScoped<ISolveRecordStore, TStore>();
            services.AddHostedService<HdojUpdateService>();
            services.AddHostedService<CfUpdateService>();
            services.AddHostedService<VjUpdateService>();
            return services;
        }
    }
}

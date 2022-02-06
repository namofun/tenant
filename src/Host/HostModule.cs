using Markdig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SatelliteSite.Services;
using System.Threading.Tasks;

namespace SatelliteSite
{
    public class HostModule : AbstractModule
    {
        public override string Area => string.Empty;

        public override void Initialize()
        {
        }

        public override void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMarkdown();

            services.ConfigureIdentityAdvanced(options =>
            {
                options.ShortenedClaimName = true;
            });

            services.Configure<ApplicationInsightsDisplayOptions>(options =>
            {
                options.ApiKey = configuration["AppInsights:Key"] ?? "DEMO_KEY";
                options.ApplicationId = configuration["AppInsights:App"] ?? "DEMO_APP";
            });
        }

        public override void RegisterEndpoints(IEndpointBuilder endpoints)
        {
            endpoints.MapRequestDelegate("/", context =>
            {
                context.Response.Redirect("/dashboard");
                return Task.CompletedTask;
            })
            .WithDisplayName("Home Page");
        }
    }
}

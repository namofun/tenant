using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

[assembly: AffiliateTo(
    typeof(Tenant.Connector.CountryFlag.CountryFlagConnector),
    typeof(SatelliteSite.GroupModule.GroupModule<>))]

namespace Tenant.Connector.CountryFlag
{
    public class CountryFlagConnector : AbstractConnector
    {
        public override string Area => "Contest";

        public override void RegisterServices(IServiceCollection services)
        {
            SatelliteSite.GroupModule.TenantDefaults.EnableCountryLogo = true;
        }
    }
}

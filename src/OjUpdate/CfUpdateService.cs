using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using Tenant.Entities;

namespace Tenant.OjUpdate
{
    /// <summary>
    /// The update service for <see cref="RecordType.Hdoj"/>.
    /// </summary>
    public class CfUpdateService : OjUpdateService
    {
        /// <summary>
        /// Construct a <see cref="HdojUpdateService"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public CfUpdateService(
            ILogger<CfUpdateService> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider,
                   RecordType.Codeforces, "Codeforces",
                   "http://codeforces.com/profile/{0}")
        {
            ColumnName = "Rating";
        }

        private class Rootobject
        {
            public string status { get; set; }
            public Result[] result { get; set; }

            public class Result
            {
                public int? rating { get; set; }
                public string handle { get; set; }
            }
        }

        /// <inheritdoc />
        public override string RankTemplate(int? rk)
        {
            if (!rk.HasValue) return "N/A";
            if (rk == -1000) return "Unrated";
            if (rk < 1200) return $"<b><font color=\"#808080\">{rk}</font></b>";
            if (rk < 1400) return $"<b><font color=\"#008000\">{rk}</font></b>";
            if (rk < 1600) return $"<b><font color=\"#03a89e\">{rk}</font></b>";
            if (rk < 1900) return $"<b><font color=\"#0000ff\">{rk}</font></b>";
            if (rk < 2100) return $"<b><font color=\"#a0a\">{rk}</font></b>";
            if (rk < 2400) return $"<b><font color=\"#ff8c00\">{rk}</font></b>";
            if (rk < 3000) return $"<b><font color=\"#ff0000\">{rk}</font></b>";
            return $"<b><font color=\"#aa0000\">{rk}</font></b>";
        }

        /// <inheritdoc />
        protected override void ConfigureHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("http://codeforces.com/");
            httpClient.Timeout = TimeSpan.FromSeconds(20);
        }

        /// <inheritdoc />
        protected override string GenerateGetSource(string account)
        {
            return "api/user.info?handles=" + account;
        }

        /// <inheritdoc />
        protected override int? MatchCount(string html)
        {
            var obj = html.AsJson<Rootobject>();
            if (obj == null || obj.status != "OK" || obj.result.Length != 1)
                return null; // User not ready?
            return obj.result[0].rating ?? -1000;
        }
    }
}

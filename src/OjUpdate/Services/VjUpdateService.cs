﻿#nullable enable
using Microsoft.Extensions.Logging;
using SatelliteSite.OjUpdateModule.Entities;
using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace SatelliteSite.OjUpdateModule.Services
{
    /// <summary>
    /// The update service for <see cref="RecordType.Vjudge"/>.
    /// </summary>
    public class VjUpdateService : OjUpdateService
    {
        /// <summary>
        /// Construct a <see cref="VjUpdateService"/>.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="serviceProvider">The service provider.</param>
        public VjUpdateService(
            ILogger<VjUpdateService> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider,
                   RecordType.Vjudge, "Vjudge",
                   "https://vjudge.net/user/{0}")
        {
        }

        /// <inheritdoc />
        public override string RankTemplate(int? rk)
        {
            return rk == null ? "N/A" : rk.Value.ToString();
        }

        /// <inheritdoc />
        protected override void ConfigureHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri("https://vjudge.net/");
            httpClient.DefaultRequestHeaders.Add("User-Agent", 
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                "Chrome/64.0.3282.140 Safari/537.36 Edge/18.17763");
            httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        /// <inheritdoc />
        protected override string GenerateGetSource(string account)
        {
            return "user/" + account;
        }

        /// <inheritdoc />
        protected override int? MatchCount(string html)
        {
            var cnt = Regex.Match(html,
                @"title=""Overall solved"" target=""_blank"">(\S+)</a>"
            ).Groups[1].Value;
            
            var success = int.TryParse(cnt, out int ans);
            return success ? ans : default(int?);
        }
    }
}

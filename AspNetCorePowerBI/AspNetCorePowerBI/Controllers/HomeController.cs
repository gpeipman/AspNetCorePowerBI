using AspNetCorePowerBI.Consts;
using AspNetCorePowerBI.Models;
using AspNetCorePowerBI.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AspNetCorePowerBI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult IFrame()
        {
            return View();
        }

        public async Task<IActionResult> CsJs([FromServices] PowerBISettings powerBISettings)
        {
            var result = new PowerBIEmbedConfig { Username = this.HttpContext.User.Identity.Name ?? this.HttpContext.User.FindFirst(ClaimTypes.Preferred_Username)?.Value };
            var accessToken = await this.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var tokenCredentials = new TokenCredentials(accessToken, "Bearer");

            using (var client = new PowerBIClient(new Uri(powerBISettings.ApiUrl), tokenCredentials))
            {
                var groupId = powerBISettings.GroupId;
                var reportId = powerBISettings.ReportId;

                var report = await client.Reports.GetReportInGroupAsync(groupId, reportId);

                result.Token = accessToken;
                var expiresAt = await this.HttpContext.GetTokenAsync("expires_at");
                var expire = DateTime.Parse(expiresAt);
                result.Expiration = expire;
                result.EmbedUrl = report.EmbedUrl;
                result.Id = report.Id;

            }

            return View(result);
        }


        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

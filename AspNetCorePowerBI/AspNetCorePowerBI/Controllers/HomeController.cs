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
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCorePowerBI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult IFrame()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> CsJs([FromServices]PowerBISettings powerBISettings)
        {
            var result = new PowerBIEmbedConfig { Username = this.HttpContext.User.Identity.Name ?? this.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value};
            var accessToken = await this.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var tokenCredentials = new TokenCredentials(accessToken, "Bearer");

            using (var client = new PowerBIClient(new Uri(powerBISettings.ApiUrl), tokenCredentials))
            {
                var groupId = powerBISettings.GroupId;
                var reportId = powerBISettings.ReportId;
                var report = await client.Reports.GetReportInGroupAsync(groupId, reportId);
                var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                var tokenResponse = await client.Reports.GenerateTokenAsync(groupId, reportId, generateTokenRequestParameters);

                result.EmbedToken = tokenResponse;
                result.EmbedUrl = report.EmbedUrl;
                result.Id = report.Id;
            }

            return View(result);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

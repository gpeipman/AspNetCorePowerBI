using System;
using System.Diagnostics;
using System.Threading.Tasks;
using AspNetCorePowerBI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;

namespace AspNetCorePowerBI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
    
    public async Task<IActionResult> Index([FromServices] PowerBISettings powerBISettings)
    {
        var result = new PowerBIEmbedConfig { Username = powerBISettings.UserName };
        var accessToken = await new PowerBiService().GetPowerBIAccessToken(powerBISettings);
        var tokenCredentials = new TokenCredentials(accessToken, "Bearer");

        using (var client = new PowerBIClient(new Uri(powerBISettings.ApiUrl), tokenCredentials))
        {
            var workspaceId = powerBISettings.WorkspaceId;
            var reportId = powerBISettings.ReportId;
            var report = await client.Reports.GetReportInGroupAsync(workspaceId, reportId);
            var generateTokenRequestParameters = new GenerateTokenRequest("view");
            var tokenResponse = await client.Reports.GenerateTokenAsync(workspaceId, reportId, generateTokenRequestParameters);

            result.EmbedToken = tokenResponse;
            result.EmbedUrl = report.EmbedUrl;
            result.Id = report.Id;
        }

        return View(result);
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
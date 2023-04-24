using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCorePowerBI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;

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
        var accessToken = await GetPowerBIAccessToken(powerBISettings);
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

    private async Task<string> GetPowerBIAccessToken(PowerBISettings powerBISettings)
    {
        using var client = new HttpClient();

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["resource"] = powerBISettings.ResourceUrl,
            ["username"] = powerBISettings.UserName,
            ["password"] = powerBISettings.Password,
            ["client_id"] = powerBISettings.ApplicationId.ToString(),
            ["client_secret"] = powerBISettings.ApplicationSecret,
            ["scope"] = "openid"
        };

        client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

        using var formContent = new FormUrlEncodedContent(form);

        using var response = await client.PostAsync(powerBISettings.AuthorityUrl, formContent);

        var body = await response.Content.ReadAsStringAsync();
        var jsonBody = JObject.Parse(body);

        var errorToken = jsonBody.SelectToken("error");

        if (errorToken != null)
        {
            throw new Exception(errorToken.Value<string>());
        }

        return jsonBody.SelectToken("access_token")?.Value<string>();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
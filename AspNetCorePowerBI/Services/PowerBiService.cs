using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;

namespace AspNetCorePowerBI.Services;

public class PowerBiService
{
    private readonly PowerBISettings _settings;

    public PowerBiService(PowerBISettings settings)
    {
        _settings = settings;
    }

    public async Task<PowerBIEmbedConfig> GetConfig()
    {
        var accessToken = await GetAccessToken(_settings);
        var tokenCredentials = new TokenCredentials(accessToken, "Bearer");

        using var client = new PowerBIClient(new Uri(_settings.ApiUrl), tokenCredentials);

        var workspaceId = _settings.WorkspaceId;
        var reportId = _settings.ReportId;
        var report = await client.Reports.GetReportInGroupAsync(workspaceId, reportId);
        var generateTokenRequestParameters = new GenerateTokenRequest("view");
        var tokenResponse = await client.Reports.GenerateTokenAsync(workspaceId, reportId, generateTokenRequestParameters);

        var result = new PowerBIEmbedConfig
        {
            Username = _settings.UserName,
            EmbedToken = tokenResponse,
            EmbedUrl = report.EmbedUrl,
            Id = report.Id
        };

        return result;
    }

    private async Task<string> GetAccessToken(PowerBISettings powerBISettings)
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
}
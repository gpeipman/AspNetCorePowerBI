using AspNetCorePowerBI.Consts;
using AspNetCorePowerBI.Settings;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {

        public static IServiceCollection AddPowerBiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            IdentityModelEventSource.ShowPII = true;
            services.Configure<AzureAdSettings>(configuration.GetSection(ConfigurationConsts.CONFIGURATION_AUTHENTICATION));
            AzureAdSettings azureadoptions = configuration.GetSection(ConfigurationConsts.CONFIGURATION_AUTHENTICATION).Get<AzureAdSettings>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddOpenIdConnect(options =>
            {
                options.ClientId = azureadoptions.ApplicationId.ToString();
                options.Authority = azureadoptions.AuthorityUrl;
                options.UseTokenLifetime = true;
                options.CallbackPath = azureadoptions.CallbackPath;
                options.ClientSecret = azureadoptions.ApplicationSecret;
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.SaveTokens = true;
                options.RequireHttpsMetadata = false;
                options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
                foreach (var scope in azureadoptions.Scopes)
                    options.Scope.Add(scope);
            })
            .AddCookie();

            return services;
        }

    }
}

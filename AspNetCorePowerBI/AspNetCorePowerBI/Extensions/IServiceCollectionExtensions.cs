using AspNetCorePowerBI.Consts;
using AspNetCorePowerBI.Settings;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AspNetCorePowerBI.Extensions
{
    public static class IServiceCollectionExtensions
    {

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
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
                options.Scope.Add(azureadoptions.ResourceUrl);
            })
                .AddCookie();
            return services;
        }

    }
}

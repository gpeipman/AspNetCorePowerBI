using System;

namespace AspNetCorePowerBI.Models
{
    public class PowerBIEmbedConfig
    {
        public Guid Id { get; set; }
        public string EmbedUrl { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set;  }

        public int MinutesToExpiration
        {
            get
            {
                var minutesToExpiration = Expiration - DateTime.UtcNow;
                return minutesToExpiration.Minutes;
            }
        }

        public bool? IsEffectiveIdentityRolesRequired { get; set; }
        public bool? IsEffectiveIdentityRequired { get; set; }
        public bool EnableRLS { get; set; }
        public string Username { get; set; }
        public string Roles { get; set; }
        public string ErrorMessage { get; internal set; }
    }
}

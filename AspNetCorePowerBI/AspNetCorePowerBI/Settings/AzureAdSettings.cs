using System;

namespace AspNetCorePowerBI.Settings
{
    public class AzureAdSettings
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationSecret { get; set; }
        public string AuthorityUrl { get; set; }
        public string ResourceUrl { get; set; }
        public string CallbackPath {  get; set; }
    }
}

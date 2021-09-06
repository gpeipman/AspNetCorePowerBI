using System;

namespace AspNetCorePowerBI
{
    public class PowerBISettings
    {
        public Guid ApplicationId { get; set; }
        public string ApplicationSecret { get; set; }
        public Guid ReportId { get; set; }
        public Guid GroupId { get; set; }
        public string AuthorityUrl { get; set; }
        public string ResourceUrl { get; set; }
        public string ApiUrl { get; set; }
        public string EmbedUrlBase { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

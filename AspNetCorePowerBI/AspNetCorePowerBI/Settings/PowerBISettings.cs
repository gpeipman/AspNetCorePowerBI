using System;

namespace AspNetCorePowerBI.Settings
{
    public class PowerBISettings
    {
        public Guid ReportId { get; set; }
        public Guid GroupId { get; set; }
        public string EmbedUrlBase { get; set; }
        public string ApiUrl { get; set; }

    }
}

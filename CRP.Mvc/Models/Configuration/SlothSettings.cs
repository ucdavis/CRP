using System;

namespace CRP.Mvc.Models.Configuration
{
    public class SlothSettings
    {
        public string BaseUrl { get; set; }
        public string BaseUrlV2 { get; set; }

        public string ApiKey { get; set; }

        public bool UseCoa { get; set; }
    }
}
// Models/UpdateConfig.cs
using System;

namespace GeradorXML.Models
{
    public class UpdateConfig
    {
        public string UpdateUrl { get; set; } = "";
        public int CheckIntervalHours { get; set; } = 24;
        public bool AutoCheck { get; set; } = true;
        public string IgnoredVersion { get; set; } = "";
        public DateTime LastCheck { get; set; }
        public bool IsFirstRun { get; set; } = true;
        public string LastUpdateUrl { get; set; } = "";
        public DateTime LastSuccessfulUpdate { get; set; }
    }
}
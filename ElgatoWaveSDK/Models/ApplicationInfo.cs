using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveSDK.Models
{
    public class ApplicationInfo
    {
        [JsonProperty("appId")]
        public string? Id { get; set; }

        [JsonProperty("appName")]
        public string? Name { get; set; }

        [JsonProperty("appVersion")]
        public AppVersion? AppVersion { get; set; }

        [JsonIgnore]
        public Version? Version => new(AppVersion?.MajorRelease ?? 0, AppVersion?.MinorRelease ?? 0, AppVersion?.BuildNumber ?? 0, AppVersion?.PatchLevel ?? 0);

        [JsonProperty("interfaceRevision")]
        public int? InterfaceRevision { get; set; }
    }

    public class AppVersion
    {
        [JsonProperty("appVersionBuildNumber")]
        public int BuildNumber { get; set; }

        [JsonProperty("appVersionMajorRelease")]
        public int MajorRelease { get; set; }

        [JsonProperty("appVersionMinorRelease")]
        public int MinorRelease { get; set; }

        [JsonProperty("appVersionPatchLevel")]
        public int PatchLevel { get; set; }
    }
}

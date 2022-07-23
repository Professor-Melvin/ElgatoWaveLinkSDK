using Newtonsoft.Json;
using System;

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
        public Version? Version => AppVersion is not null ? new Version(AppVersion?.MajorRelease ?? 0, AppVersion?.MinorRelease ?? 0, AppVersion?.BuildNumber ?? 0, AppVersion?.PatchLevel ?? 0) : null;

        [JsonProperty("interfaceRevision")]
        public int? InterfaceRevision { get; set; }

    }

    public class AppVersion
    {
        [JsonProperty("appVersionBuildNumber")]
        public int? BuildNumber { get; set; }

        [JsonProperty("appVersionMajorRelease")]
        public int? MajorRelease { get; set; }

        [JsonProperty("appVersionMinorRelease")]
        public int? MinorRelease { get; set; }

        [JsonProperty("appVersionPatchLevel")]
        public int? PatchLevel { get; set; }
    }
}
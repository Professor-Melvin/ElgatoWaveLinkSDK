using System;
using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models.Old
{
    public class ApplicationInfo
    {
        [JsonPropertyName("appId")]
        public string? Id
        {
            get; set;
        }

        [JsonPropertyName("appName")]
        public string? Name
        {
            get; set;
        }

        [JsonPropertyName("appVersion")]
        public AppVersion? AppVersion
        {
            get; set;
        }

        [JsonIgnore]
        public Version? Version => AppVersion is not null ? new Version(AppVersion?.MajorRelease ?? 0, AppVersion?.MinorRelease ?? 0, AppVersion?.BuildNumber ?? 0, AppVersion?.PatchLevel ?? 0) : null;

        [JsonPropertyName("interfaceRevision")]
        public int? InterfaceRevision
        {
            get; set;
        }

    }

    public class AppVersion
    {
        [JsonPropertyName("appVersionBuildNumber")]
        public int? BuildNumber
        {
            get; set;
        }

        [JsonPropertyName("appVersionMajorRelease")]
        public int? MajorRelease
        {
            get; set;
        }

        [JsonPropertyName("appVersionMinorRelease")]
        public int? MinorRelease
        {
            get; set;
        }

        [JsonPropertyName("appVersionPatchLevel")]
        public int? PatchLevel
        {
            get; set;
        }
    }
}
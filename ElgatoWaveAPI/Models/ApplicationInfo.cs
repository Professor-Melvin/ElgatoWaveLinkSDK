using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveAPI.Models
{
    public class ApplicationInfo
    {
        [JsonProperty("appId")]
        public string AppId { get; set; }

        [JsonProperty("appName")]
        public string AppName { get; set; }

        [JsonProperty("build")]
        public long Build { get; set; }

        [JsonProperty("interfaceRevision")]
        public long InterfaceRevision { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }
}

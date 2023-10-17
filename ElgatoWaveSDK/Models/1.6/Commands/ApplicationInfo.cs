using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models._1._6.Commands
{
    public class ApplicationInfo
    {
        [JsonPropertyName(ElgatoConstants.Properties.ApplicationInfo.AppID)]
        public string AppId
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.ApplicationInfo.AppName)]
        public string AppName
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.ApplicationInfo.InterfaceRevision)]
        public string InterfaceRevision
        {
            get;
            set;
        }
    }
}

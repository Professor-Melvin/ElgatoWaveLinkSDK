using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models._1._6.Commands
{
    public class Output
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.Identifier)]
        public string? Id
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.Name)]
        public string? Name
        {
            get;
            set;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models._1._6.Commands
{
    public class SwitchState
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.Value)]
        public string? Mixer
        {
            get;
            set;
        }
    }
}

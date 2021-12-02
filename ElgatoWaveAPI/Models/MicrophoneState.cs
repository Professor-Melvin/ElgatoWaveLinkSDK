using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveAPI.Models
{
    public class MicrophoneState
    {
        [JsonProperty("isMicrophoneConnected")]
        public bool IsMicrophoneConnected { get; set; }
    }
}

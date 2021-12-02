using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveAPI.Models
{
    public class MicrophoneSettings
    {
        [JsonProperty("isMicrophoneClipguardOn")]
        public bool IsMicrophoneClipguardOn { get; set; }

        [JsonProperty("isMicrophoneLowcutOn")]
        public bool IsMicrophoneLowcutOn { get; set; }

        [JsonProperty("microphoneBalance")]
        public long MicrophoneBalance { get; set; }

        [JsonProperty("microphoneGain")]
        public long MicrophoneGain { get; set; }

        [JsonProperty("microphoneOutputVolume")]
        public long MicrophoneOutputVolume { get; set; }
    }
}

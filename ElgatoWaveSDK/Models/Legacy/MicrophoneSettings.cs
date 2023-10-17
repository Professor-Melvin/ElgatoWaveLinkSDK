using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models.Old
{
    public class MicrophoneSettings
    {
        [JsonPropertyName("isMicrophoneClipguardOn")]
        public bool? IsMicrophoneClipguardOn
        {
            get; set;
        }

        [JsonPropertyName("isMicrophoneLowcutOn")]
        public bool? IsMicrophoneLowcutOn
        {
            get; set;
        }

        [JsonPropertyName("microphoneBalance")]
        public int? MicrophoneBalance
        {
            get; set;
        }

        [JsonPropertyName("microphoneGain")]
        public int? MicrophoneGain
        {
            get; set;
        }

        [JsonPropertyName("microphoneOutputVolume")]
        public int? MicrophoneOutputVolume
        {
            get; set;
        }
    }
}

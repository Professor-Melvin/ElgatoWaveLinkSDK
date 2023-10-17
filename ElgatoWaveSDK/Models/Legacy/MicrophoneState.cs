using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models.Old
{
    public class MicrophoneState
    {
        [JsonPropertyName("isMicrophoneConnected")]
        public bool? IsMicrophoneConnected
        {
            get; set;
        }
    }
}

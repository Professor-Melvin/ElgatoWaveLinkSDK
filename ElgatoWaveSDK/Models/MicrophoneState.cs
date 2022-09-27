using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models
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

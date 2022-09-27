using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models
{
    public class MonitoringState
    {
        [JsonPropertyName("isLocalOutMuted")]
        public bool? IsLocalOutMuted
        {
            get; set;
        }

        [JsonPropertyName("isStreamOutMuted")]
        public bool? IsStreamOutMuted
        {
            get; set;
        }

        [JsonPropertyName("localVolumeOut")]
        public long? LocalVolumeOut
        {
            get; set;
        }

        [JsonPropertyName("streamVolumeOut")]
        public long? StreamVolumeOut
        {
            get; set;
        }
    }
}

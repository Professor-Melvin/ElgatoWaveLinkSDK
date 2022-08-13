using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("System.Text.Json")]
namespace ElgatoWaveSDK.Models
{
    public class ChannelInfo
    {
        [JsonInclude]
        [JsonPropertyName("slider")] 
        public string? Slider { get; internal set; }

        [JsonInclude]
        [JsonPropertyName("bgColor")]
        public string? BgColor { get; internal set; }

        [JsonPropertyName("filters")]
        public List<Filter>? Filters { get; set; }

        [JsonPropertyName("iconData")]
        public string? IconData { get; set; }

        [JsonPropertyName("inputType")]
        public int? InputType { get; set; }

        [JsonPropertyName("isAvailable")]
        public bool? IsAvailable { get; set; }

        [JsonPropertyName("isLocalInMuted")]
        public bool? IsLocalInMuted { get; set; }

        [JsonPropertyName("isStreamInMuted")]
        public bool? IsStreamInMuted { get; set; }

        [JsonPropertyName("localMixFilterBypass")]
        public bool? LocalMixFilterBypass { get; set; }

        [JsonPropertyName("localVolumeIn")]
        public int? LocalVolumeIn { get; set; }

        [JsonPropertyName("mixId")]
        public string? MixId { get; set; }

        [JsonPropertyName("mixerName")]
        public string? MixerName { get; set; }

        [JsonPropertyName("streamMixFilterBypass")]
        public bool? StreamMixFilterBypass { get; set; }

        [JsonPropertyName("streamVolumeIn")]
        public int? StreamVolumeIn { get; set; }
    }
}

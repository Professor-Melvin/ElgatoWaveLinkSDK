using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveSDK.Models
{
    public class ChannelInfo
    {
        [JsonProperty("bgColor")]
        public string? BgColor { get; internal set; }

        [JsonProperty("filters")]
        public List<Filter>? Filters { get; set; }

        [JsonProperty("slider")] 
        internal string? Slider { get; set; }

        [JsonProperty("deltaLinked")]
        public long? DeltaLinked { get; internal set; }

        [JsonProperty("iconData")]
        public string? IconData { get; internal set; }

        [JsonProperty("inputType")]
        public long? InputType { get; internal set; }

        [JsonProperty("isAvailable")]
        public bool? IsAvailable { get; internal set; }

        [JsonProperty("isLinked")]
        public bool? IsLinked { get; internal set; }

        [JsonProperty("isLocalInMuted")]
        public bool? IsLocalInMuted { get; set; }

        [JsonProperty("isStreamInMuted")]
        public bool? IsStreamInMuted { get; set; }

        [JsonProperty("localVolumeIn")]
        public long? LocalVolumeIn { get; set; }

        [JsonProperty("localMixFilterBypass")] 
        public bool? LocalMixFilterBypass { get; set; }

        [JsonProperty("mixId")]
        public string? MixId { get; set; }

        [JsonProperty("mixerName")]
        public string? MixerName { get; internal set; }

        [JsonProperty("streamMixFilterBypass")]
        public bool? StreamMixFilterBypass { get; set; }

        [JsonProperty("streamVolumeIn")]
        public long? StreamVolumeIn { get; set; }
    }
}

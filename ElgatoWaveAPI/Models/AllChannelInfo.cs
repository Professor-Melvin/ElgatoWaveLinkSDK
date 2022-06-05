using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveAPI.Models
{
    public class ChannelInfo
    {
        [JsonProperty("bgColor")]
        public string BgColor { get; set; }
        
        [JsonProperty("filters")]
        public List<Filter> Filters { get; set; }
        
        [JsonProperty("iconData")]
        public string IconData { get; set; }

        /*[JsonProperty("deltaLinked")]
        public long DeltaLinked { get; set; }*/
        
        [JsonProperty("inputType")]
        public int InputType { get; set; }

        [JsonProperty("isAvailable")]
        public bool IsAvailable { get; set; }

        /*[JsonProperty("isLinked")]
        public bool IsLinked { get; set; }*/

        [JsonProperty("isLocalInMuted")]
        public bool IsLocalInMuted { get; set; }

        [JsonProperty("isStreamInMuted")]
        public bool IsStreamInMuted { get; set; }
        
        [JsonProperty("localMixFilterBypass")]
        public bool LocalMixFilterBypass { get; set; }

        [JsonProperty("localVolumeIn")]
        public int LocalVolumeIn { get; set; }

        [JsonProperty("mixId")]
        public string MixId { get; set; }

        [JsonProperty("mixerName")]
        public string MixerName { get; set; }
        
        [JsonProperty("streamMixFilterBypass")]
        public bool StreamMixFilterBypass { get; set; }

        [JsonProperty("streamVolumeIn")]
        public int StreamVolumeIn { get; set; }
    }
}
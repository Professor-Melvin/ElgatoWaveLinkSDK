using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models._1._6.Commands
{
    public class OutputConfig
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.LocalMixer)]
        public List<object> LocalMixerRaw
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.StreamMixer)]
        public List<object> StreamMixerRaw
        {
            get;
            set;
        }

        [JsonIgnore]
        public OutputMixerConfig LocalOutputMixer => OutputMixerConfig.Parse(LocalMixerRaw);

        [JsonIgnore]
        public OutputMixerConfig StreamOutputMixer => OutputMixerConfig.Parse(StreamMixerRaw);
    }

    public class OutputMixerConfig
    {
        public bool IsMute
        {
            get;
            set;
        }

        public int Volume
        {
            get;
            set;
        }

        public static OutputMixerConfig Parse(List<object> values)
        {
            return new OutputMixerConfig()
            {
                IsMute = bool.Parse(values.First().ToString()),
                Volume = int.Parse(values.Last().ToString()),
            };
        }
    }
}

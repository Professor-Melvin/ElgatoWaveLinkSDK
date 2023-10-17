using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models._1._6.Commands
{
    public class InputConfig
    {
        [JsonPropertyName("bgColor")]
        public string BgColor
        {
            get; set;
        }

        [JsonPropertyName("iconData")]
        public string IconData
        {
            get; set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.Identifier)]
        public string Id
        {
            get; set;
        }

        [JsonPropertyName("inputType")]
        public int InputType
        {
            get; set;
        }

        [JsonPropertyName("isAvailable")]
        public bool IsAvailable
        {
            get; set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.LocalMixer)]
        public List<object> LocalMixerRaw
        {
            get; set;
        }

        [JsonIgnore]
        public InputMixerConfig LocalMixer => InputMixerConfig.Parse(LocalMixerRaw);

        [JsonPropertyName(ElgatoConstants.Properties.Common.Name)]
        public string Name
        {
            get; set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.StreamMixer)]
        public List<object> StreamMixerRaw
        {
            get; set;
        }

        [JsonIgnore]
        public InputMixerConfig StreamMixer => InputMixerConfig.Parse(StreamMixerRaw);
    }

    public class InputMixerConfig : OutputMixerConfig
    {
        public bool IsFilterActive
        {
            get;
            set;
        }

        public static new InputMixerConfig Parse(List<object> values)
        {
            return new InputMixerConfig()
            {
                IsMute = bool.Parse(values.First().ToString()),
                Volume = int.Parse(values.Skip(1).First().ToString()),
                IsFilterActive = bool.Parse(values.Last().ToString()),
            };
        }
    }
}

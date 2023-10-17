using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using ElgatoWaveSDK.Models.Interfaces;

namespace ElgatoWaveSDK.Models._1._6.Commands
{
    public class MicrophoneConfig : IHasChannel
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.Identifier)]
        public string ChannelId
        {
            get;
            set;
        }

        [JsonPropertyName("isClipGuardOn")]
        public bool ClipGaurd
        {
            get;
            set;
        }

        [JsonPropertyName("isLowCutOn")]
        public bool Lowcut
        {
            get;
            set;
        }

        [JsonPropertyName("isWaveLink")]
        public bool IsWaveLink
        {
            get;
            set;
        }

        [JsonPropertyName("isWaveXLR")]
        public bool IsWaveXLR
        {
            get;
            set;
        }

        [JsonPropertyName("lowCutType")]
        public int LowcutType
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.Name)]
        public string Name
        {
            get;
            set;
        }
    }
}

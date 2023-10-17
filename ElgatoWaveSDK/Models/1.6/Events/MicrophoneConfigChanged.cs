using System.Text.Json.Serialization;
using ElgatoWaveSDK.Models.Interfaces;

namespace ElgatoWaveSDK.Models._1._6.Events
{
    public class MicrophoneConfigChanged : IHasChannel
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.Identifier)]
        public string? ChannelId
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.Property)]
        public string? Property
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.Value)]
        public string? Value
        {
            get;
            set;
        }
    }
}

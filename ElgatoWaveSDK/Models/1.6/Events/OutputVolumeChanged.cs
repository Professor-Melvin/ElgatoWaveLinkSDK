using System.Text.Json.Serialization;
using ElgatoWaveSDK.Models.Interfaces;

namespace ElgatoWaveSDK.Models._1._6.Events
{
    public class OutputVolumeChanged: IHasChannel, IHasMixer
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.Identifier)]
        public string? ChannelId
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.MixerID)]
        public string? MixerId
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.Value)]
        public int? Value
        {
            get;
            set;
        }
    }
}

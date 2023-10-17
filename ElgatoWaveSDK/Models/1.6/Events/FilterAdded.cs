using System.Text.Json.Serialization;
using ElgatoWaveSDK.Models.Interfaces;

namespace ElgatoWaveSDK.Models._1._6.Events
{
    public class FilterAdded : IHasChannel, IHasFilter
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.Identifier)]
        public string? ChannelId
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.FilterID)]
        public string? FilterId
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.IsActive)]
        public bool? IsActive
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.Name)]
        public string? Name
        {
            get;
            set;
        }

        [JsonPropertyName(ElgatoConstants.Properties.Common.PluginID)]
        public string? PluginId
        {
            get;
            set;
        }
    }
}

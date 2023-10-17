using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models
{
    public class FilterBypassStateChanged
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.Value)]
        public bool? IsActive
        {
            get;
            set;
        }
    }
}

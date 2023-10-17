using System.Text.Json.Serialization;
using ElgatoWaveSDK.Models.Interfaces;

namespace ElgatoWaveSDK.Models._1._6.Events
{
    public class OutputSwitched : IHasMixer
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.Value)]
        public string? MixerId { get; set; }
    }
}

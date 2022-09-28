using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models
{
    public class SwitchState
    {
        [JsonPropertyName("switchState")]
        public string? CurrentState
        {
            get; set;
        }
    }
}

using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models.Old
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

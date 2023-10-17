using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models._1._6.Events
{
    public class SelectedOutputChanged
    {
        [JsonPropertyName(ElgatoConstants.Properties.Common.Value)]
        public string DeviceId
        {
            get;
            set;
        }
    }
}

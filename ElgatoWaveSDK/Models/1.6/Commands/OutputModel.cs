using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ElgatoWaveSDK.Models._1._6.Commands
{
    public class OutputModel
    {
        [JsonPropertyName("outputs")]
        public List<Output> Outputs
        {
            get;
            set;
        }

        [JsonPropertyName("selectedOutput")]
        public string SelectedOutputId
        {
            get;
            set;
        }
    }
}

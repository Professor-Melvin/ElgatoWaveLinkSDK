using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ElgatoWaveAPI.Models
{
    public class SwitchState
    {
        [JsonProperty("switchState")]
        public string switchState { get; set; }
    }
}

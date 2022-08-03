using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: InternalsVisibleTo("ElgatoWaveSDK.Tests")]
[assembly: InternalsVisibleTo("ElgatoWaveLinkEmulator")]
namespace ElgatoWaveSDK
{
    internal class SocketBaseObject<InT, OutT>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string? JsonRpc { get; } = "2.0";

        [JsonProperty("method")]
        public string? Method { get; set; }

        [JsonProperty("params")]
        public InT? Obj { get; set; }

        [JsonProperty("result")]
        public OutT? Result { get; set; }

        [JsonIgnore] 
        public DateTime? ReceivedAt { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}

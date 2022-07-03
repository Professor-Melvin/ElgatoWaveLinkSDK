using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

[assembly: InternalsVisibleTo("ElgatoWaveSDK.Tests")]
namespace ElgatoWaveSDK
{
    internal class SocketBaseObject<T>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string? JsonRpc { get; } = "2.0";

        [JsonProperty("method")]
        public string? Method { get; set; }

        [JsonProperty("params")]
        public T? Obj { get; set; }

        [JsonProperty("result")]
        public T? Result { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}

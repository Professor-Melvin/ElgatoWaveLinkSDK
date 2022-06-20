using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ElgatoWaveAPI
{
    internal class SocketBaseObject<T>
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("jsonrpc")]
        public string? JsonRpc { get; private set; } = "2.0";

        [JsonProperty("method")]
        public string? Method { get; set; }

        [JsonProperty("params")]
        public T? Obj { get; set; }

        [JsonProperty("result")]
        public T? Result { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}

using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("ElgatoWaveSDK.Tests")]
[assembly: InternalsVisibleTo("ElgatoWaveLinkEmulator")]
namespace ElgatoWaveSDK
{
    internal class SocketBaseObject<InT, OutT>
    {
        [JsonPropertyName("id")]
        public int Id
        {
            get; set;
        }

        [JsonPropertyName("jsonrpc")]
        public string? JsonRpc { get; } = "2.0";

        [JsonPropertyName("method")]
        public string? Method
        {
            get; set;
        }

        [JsonPropertyName("params")]
        public InT? Obj
        {
            get; set;
        }

        [JsonPropertyName("result")]
        public OutT? Result
        {
            get; set;
        }

        [JsonIgnore]
        public DateTime? ReceivedAt
        {
            get; set;
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
    }
}

﻿using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using ElgatoWaveSDK.HumbleObjects;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveSDK
{
    internal interface IReceiverUtils
    {
        Task<SocketBaseObject<JsonNode?, JsonDocument?>?> WaitForData(IHumbleClientWebSocket? socket, ClientConfig config, CancellationToken cancellationToken);
    }

    internal class ReceiverUtils : IReceiverUtils
    {
        public async Task<SocketBaseObject<JsonNode?, JsonDocument?>?> WaitForData(IHumbleClientWebSocket? socket, ClientConfig config, CancellationToken cancellationToken)
        {
            var buffer = new byte[config.BufferSize];
            var offset = 0;
            var free = buffer.Length;


            if (socket is not { State: WebSocketState.Open })
            {
                throw new WebSocketException("Socket Not Connected");
            }

            WebSocketReceiveResult? receivedData;
            do
            {
                receivedData = await socket.ReceiveAsync(new ArraySegment<byte>(buffer, offset, free), cancellationToken).ConfigureAwait(false);
                offset += receivedData.Count;
                free -= receivedData.Count;
                if (free == 0)
                {
                    var newSize = buffer.Length + config.BufferSize;
                    if (newSize > config.MaxBufferSize)
                    {
                        throw new ElgatoException("Maximum receive buffer size exceeded", socket.State);
                    }
                    var newBuffer = new byte[newSize];
                    Array.Copy(buffer, 0, newBuffer, 0, offset);
                    buffer = newBuffer;
                    free = buffer.Length - offset;
                }

            } while (!receivedData?.EndOfMessage ?? false);

            var json = Encoding.UTF8.GetString(buffer).Replace("\0", "");

            return JsonSerializer.Deserialize<SocketBaseObject<JsonNode?, JsonDocument?>?>(json);
        }
    }
}

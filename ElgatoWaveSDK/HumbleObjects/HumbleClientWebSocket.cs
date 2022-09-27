using System;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace ElgatoWaveSDK.HumbleObjects
{
    internal interface IHumbleClientWebSocket : IDisposable
    {
        WebSocketState State
        {
            get;
        }

        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

        Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);

        Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);
    }

    internal sealed class HumbleClientWebSocket : IHumbleClientWebSocket
    {
        private ClientWebSocket Socket
        {
            get; set;
        }

        public HumbleClientWebSocket()
        {
            Socket = new ClientWebSocket();
        }

        public HumbleClientWebSocket(ClientWebSocket socket)
        {
            Socket = socket;
        }

        public WebSocketState State => Socket.State;

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken) => Socket.ConnectAsync(uri, cancellationToken);

        public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken) => Socket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);

        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken) => Socket.ReceiveAsync(buffer, cancellationToken);

        public void Dispose()
        {
            Socket.Dispose();
        }
    }
}

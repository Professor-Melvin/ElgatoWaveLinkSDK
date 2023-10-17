using System;
using System.Net.WebSockets;

namespace ElgatoWaveSDK.Models
{
    public class ElgatoException : Exception
    {
        public WebSocketState? WebSocketState
        {
            get; set;
        }

        public ElgatoException(string message) : this(message, System.Net.WebSockets.WebSocketState.None)
        {
        }

        public ElgatoException(string message, WebSocketState? socketState) : base(message)
        {
            WebSocketState = socketState;
        }

        public ElgatoException(string message, Exception innerException, WebSocketState? socketState) : base(message, innerException)
        {
            WebSocketState = socketState;
        }
    }
}

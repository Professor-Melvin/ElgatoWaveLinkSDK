using System;
using System.Net.WebSockets;


namespace ElgatoWaveSDK.Models
{
#pragma warning disable S3925 // "ISerializable" should be implemented correctly
    public class ElgatoException : Exception
#pragma warning restore S3925 // "ISerializable" should be implemented correctly
    {
        public WebSocketState? WebSocketState { get; set; }

        public ElgatoException(string message, WebSocketState? socketState): base(message)
        {
            WebSocketState = socketState;
        }

        public ElgatoException(string message, Exception innerException, WebSocketState? socketState) : base(message, innerException)
        {
            WebSocketState = socketState;
        }
    }
}

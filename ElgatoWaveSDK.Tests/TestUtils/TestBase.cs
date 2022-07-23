using System.Net.WebSockets;
using System.Text;
using ElgatoWaveSDK.HumbleObjects;
using Moq;

namespace ElgatoWaveSDK.Tests.TestUtils
{
    public class TestBase
    {
        internal ElgatoWaveClient Subject { get; set; }

        internal Mock<IHumbleClientWebSocket> mockSocket { get; set; }
        internal Mock<ITransactionTracker> mockTracker { get; set; }

        internal int CommandId { get; set; }

        private byte[]? receiveData { get;set;}
        private int receiveDataCount => receiveData?.Length ?? 0;

        internal TestBase()
        {
            CommandId = new Random().Next(1000000);

            mockSocket = new Mock<IHumbleClientWebSocket>();
            mockTracker = new Mock<ITransactionTracker>();

            mockTracker.Setup(c => c.NextTransactionId()).Returns(CommandId);

            Subject = new ElgatoWaveClient(mockSocket.Object, mockTracker.Object);
        }

        internal void SetupConnection(WebSocketState value = WebSocketState.Open)
        {
            mockSocket.Setup(c => c.State).Returns(value);
        }

        internal void SetupReply(string replyObjectJson, string? method = null)
        {
            var replyObject = new SocketBaseObject<string, string>()
            {
                Id= CommandId,
                Method = method,
                Result = replyObjectJson
            };

            receiveData = Encoding.UTF8.GetBytes(replyObject.ToJson());

            var setupSequence = new MockSequence();
            int timesX = receiveDataCount / 1024;

            for (int i = 0; i < timesX; i++)
            {
                mockSocket.InSequence(setupSequence)
                    .Setup(c => c.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                    .Callback(ReturnCallback)
                    .ReturnsAsync(new WebSocketReceiveResult(1024, WebSocketMessageType.Text, false));
            }

            mockSocket.InSequence(setupSequence)
                    .Setup(c => c.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                    .Callback(ReturnCallback)
                    .ReturnsAsync(new WebSocketReceiveResult(receiveDataCount % 1024, WebSocketMessageType.Text, true));

#pragma warning disable S1172 // Unused method parameters should be removed
            void ReturnCallback(ArraySegment<byte> array, CancellationToken token)
#pragma warning restore S1172 // Unused method parameters should be removed
            {
                if ((array.Offset + array.Count) > receiveDataCount) 
                {
                    Array.Copy(receiveData, array.Offset, array.Array!, array.Offset, receiveDataCount - array.Offset);
                }
                else
                {
                    Array.Copy(receiveData, array.Offset, array.Array!, 0, array.Count);
                }
            }
        }
    }
}

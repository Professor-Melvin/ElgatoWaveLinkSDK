using System.Net.WebSockets;
using System.Text;
using ElgatoWaveSDK.HumbleObjects;
using Moq;

namespace ElgatoWaveSDK.Tests.TestUtils;

public class TestBase
{
    internal ElgatoWaveClient Subject { get; set; }

    internal Mock<IHumbleClientWebSocket> MockSocket { get; set; }
    internal Mock<ITransactionTracker> MockTracker { get; set; }

    internal int CommandId { get; set; }

    private byte[]? ReceiveData { get;set;}
    private int ReceiveDataCount => ReceiveData?.Length ?? 0;

    internal TestBase()
    {
        CommandId = new Random().Next(1000000);

        MockSocket = new Mock<IHumbleClientWebSocket>();
        MockTracker = new Mock<ITransactionTracker>();

        MockTracker.Setup(c => c.NextTransactionId()).Returns(CommandId);

        Subject = new ElgatoWaveClient(MockSocket.Object, MockTracker.Object);
    }

    internal void SetupConnection(WebSocketState value = WebSocketState.Open)
    {
        MockSocket.Setup(c => c.State).Returns(value);
    }

    internal void SetupReply(string replyObjectJson, string? method = null)
    {
        var replyObject = new SocketBaseObject<string, string>()
        {
            Id= CommandId,
            Method = method,
            Result = replyObjectJson
        };

        ReceiveData = Encoding.UTF8.GetBytes(replyObject.ToJson());

        var setupSequence = new MockSequence();
        var timesX = ReceiveDataCount / 1024;

        for (var i = 0; i < timesX; i++)
        {
            MockSocket.InSequence(setupSequence)
                .Setup(c => c.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .Callback(ReturnCallback)
                .ReturnsAsync(new WebSocketReceiveResult(1024, WebSocketMessageType.Text, false));
        }

        MockSocket.InSequence(setupSequence)
                .Setup(c => c.ReceiveAsync(It.IsAny<ArraySegment<byte>>(), It.IsAny<CancellationToken>()))
                .Callback(ReturnCallback)
                .ReturnsAsync(new WebSocketReceiveResult(ReceiveDataCount % 1024, WebSocketMessageType.Text, true));

#pragma warning disable S1172 // Unused method parameters should be removed
        void ReturnCallback(ArraySegment<byte> array, CancellationToken token)
#pragma warning restore S1172 // Unused method parameters should be removed
        {
            if ((array.Offset + array.Count) > ReceiveDataCount) 
            {
                Array.Copy(ReceiveData, array.Offset, array.Array!, array.Offset, ReceiveDataCount - array.Offset);
            }
            else
            {
                Array.Copy(ReceiveData, array.Offset, array.Array!, 0, array.Count);
            }
        }
    }
}

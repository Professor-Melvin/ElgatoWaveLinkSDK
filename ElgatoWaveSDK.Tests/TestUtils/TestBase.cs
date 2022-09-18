using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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

    internal void SetupReply(object replyObjectJson, string? method = null, bool isEvent = false)
    {
        var replyObject = new SocketBaseObject<object, object>()
        {
            Id= isEvent ? 0 : CommandId,
            Method = method,
            Result = isEvent ? null : replyObjectJson,
            Obj = isEvent ? replyObjectJson : null
        };

        var json = replyObject.ToJson();
        ReceiveData = Encoding.UTF8.GetBytes(json);

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

        void ReturnCallback(ArraySegment<byte> array, CancellationToken token)
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

using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Nodes;
using ElgatoWaveSDK.Clients;
using ElgatoWaveSDK.HumbleObjects;
using Moq;

namespace ElgatoWaveSDK.Tests.TestUtils;

public class TestBase
{
    internal LegacyElgatoWaveClient Subject
    {
        get; set;
    }

    internal Mock<IHumbleClientWebSocket> MockSocket
    {
        get; set;
    }
    internal Mock<ITransactionTracker> MockTracker
    {
        get; set;
    }
    internal Mock<IReceiverUtils> MockReceiver
    {
        get; set;
    }

    internal int CommandId
    {
        get; set;
    }

    internal TestBase()
    {
        CommandId = new Random().Next(1000000);

        MockSocket = new Mock<IHumbleClientWebSocket>();
        MockTracker = new Mock<ITransactionTracker>();
        MockReceiver = new Mock<IReceiverUtils>();

        MockTracker.Setup(c => c.NextTransactionId()).Returns(CommandId);

        Subject = new LegacyElgatoWaveClient(MockSocket.Object, MockReceiver.Object, MockTracker.Object);
    }

    internal void SetupConnection(WebSocketState value = WebSocketState.Open)
    {
        MockSocket.Setup(c => c.State).Returns(value);
    }

    internal void SetupReply(object replyObjectJson, string? method = null, bool isEvent = false)
    {
        var replyObject = new SocketBaseObject<JsonNode?, JsonDocument?>()
        {
            Id = isEvent ? 0 : CommandId,
            Method = method,
            Result = JsonDocument.Parse(JsonSerializer.Serialize(replyObjectJson)),
            Obj = JsonNode.Parse(JsonSerializer.Serialize(replyObjectJson))
        };

        MockReceiver.Setup(c => c.WaitForData(
                It.IsAny<IHumbleClientWebSocket?>(),
                It.IsAny<ClientConfig>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((SocketBaseObject<JsonNode?, JsonDocument?>?)replyObject);
    }
}

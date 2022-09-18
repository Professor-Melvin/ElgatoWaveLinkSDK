using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using ElgatoWaveSDK.HumbleObjects;
using Moq;
using Xunit.Abstractions;

namespace ElgatoWaveSDK.Tests.TestUtils;

public class TestBase
{
    internal ElgatoWaveClient Subject { get; set; }

    internal Mock<IHumbleClientWebSocket> MockSocket { get; set; }
    internal Mock<ITransactionTracker> MockTracker { get; set; }
    internal Mock<IReceiverUtils> MockReceiver { get; set; }

    internal int CommandId { get; set; }

    internal ITestOutputHelper? _testOutput;

    internal TestBase(ITestOutputHelper? output = null)
    {
        _testOutput = output;

        CommandId = new Random().Next(1000000);
        _testOutput?.WriteLine("Setting Command ID: " + CommandId);

        MockSocket = new Mock<IHumbleClientWebSocket>();
        MockTracker = new Mock<ITransactionTracker>();
        MockReceiver = new Mock<IReceiverUtils>();

        MockTracker.Setup(c => c.NextTransactionId()).Returns(CommandId);

        Subject = new ElgatoWaveClient(MockSocket.Object, MockReceiver.Object, MockTracker.Object);

        Subject.ExceptionOccurred += (_, exception) =>
        {
            _testOutput?.WriteLine("Exception Occurred: " + exception.Message + "\nState: " + exception.WebSocketState + "\n" + exception.StackTrace);
        };

        Subject.TestMessages += (_, s) =>
        {
            _testOutput?.WriteLine(s);
        };
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
                It.IsAny<IHumbleClientWebSocket>(), 
                It.IsAny<ClientConfig>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(replyObject);
    }
}

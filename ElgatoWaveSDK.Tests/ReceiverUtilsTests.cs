using System.Net.WebSockets;
using System.Text;
using ElgatoWaveSDK.HumbleObjects;
using FluentAssertions;
using Moq;
using Xunit;

namespace ElgatoWaveSDK.Tests;

public class ReceiverUtilsTests
{
    private readonly ReceiverUtils Subject;

    private Mock<IHumbleClientWebSocket> MockSocket
    {
        get; set;
    }

    private ClientConfig Config
    {
        get;
    }

    private int CommandId
    {
        get; set;
    }

    public ReceiverUtilsTests()
    {
        MockSocket = new Mock<IHumbleClientWebSocket>();
        MockSocket.Setup(c => c.State).Returns(WebSocketState.Open);

        CommandId = new Random().Next(1000000);

        Config = new ClientConfig();
        Subject = new ReceiverUtils();
    }

    [Fact]
    public async Task GoodJson()
    {
        SetupSocket(new
        {
            hello = "World",
            This = "Is",
            A = "Test",
            Json = "Woo",
            Which = "Needs",
            Long = "String",
            Stirng = "_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!_!"
        }, "helloWorld");

        var reply = await Subject.WaitForData(MockSocket.Object, Config, CancellationToken.None).ConfigureAwait(false);

        reply.Should().NotBeNull();
        reply?.Method.Should().Be("helloWorld");
    }

    private byte[]? ReceiveData
    {
        get; set;
    }
    private int ReceiveDataCount => ReceiveData?.Length ?? 0;

    private void SetupSocket(object? replyObjectJson, string? method = null, bool isEvent = false)
    {
        var replyObject = new SocketBaseObject<object?, object?>()
        {
            Id = isEvent ? 0 : CommandId,
            Method = method,
            Result = replyObjectJson,
            Obj = replyObjectJson
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

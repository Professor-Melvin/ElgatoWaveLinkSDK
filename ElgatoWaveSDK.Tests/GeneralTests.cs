using System.Net.WebSockets;
using ElgatoWaveSDK.Models;
using ElgatoWaveSDK.Tests.TestUtils;
using FluentAssertions;
using Xunit;

namespace ElgatoWaveSDK.Tests
{
    public class GeneralTests : TestBase
    {
        [Theory]
        [
            InlineData(WebSocketState.Aborted),
            InlineData(WebSocketState.CloseReceived),
            InlineData(WebSocketState.CloseSent),
            InlineData(WebSocketState.Closed),
            InlineData(WebSocketState.Connecting),
            InlineData(WebSocketState.None),
        ]
        public void ConnectFails(WebSocketState state)
        {
            SetupConnection(state);

            var action = () => Subject.ConnectAsync();

            action.Should().ThrowExactlyAsync<ElgatoException>();
        }

        [Fact]
        public void ConnectSuccess()
        {
            SetupConnection();

            var action = () => Subject.ConnectAsync();

            action.Should().NotThrowAsync();
        }
    }
}

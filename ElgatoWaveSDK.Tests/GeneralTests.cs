using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
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
        public async Task ConnectFails(WebSocketState state)
        {
            SetupConnection(state);

            try
            {
                var connectReply = await Subject.ConnectAsync().ConfigureAwait(false);
                connectReply.Should().BeFalse();
            }
            catch(Exception ex)
            {
                ex.Message.Should().Be("Looped through possible ports 2 times and couldn't connect [1824-1834]");
                ex.Should().BeOfType<ElgatoException>();
                ((ElgatoException)ex).WebSocketState.Should().Be(state);
            }
        }

        [Fact]
        public async Task ConnectSuccess()
        {
            SetupConnection();

            var connectReply = await Subject.ConnectAsync().ConfigureAwait(false);
            connectReply.Should().BeTrue();
        }
    }
}

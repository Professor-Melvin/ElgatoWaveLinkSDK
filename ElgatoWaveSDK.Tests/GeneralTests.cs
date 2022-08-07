using System.Net.WebSockets;
using ElgatoWaveSDK.Models;
using ElgatoWaveSDK.Tests.TestUtils;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;

namespace ElgatoWaveSDK.Tests;

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

        var testTask = new TaskCompletionSource<Task>();
        testTask.SetResult(Subject.ConnectAsync());

        await testTask.Should().CompleteWithinAsync(1.Seconds());

        _ = await Subject.Invoking(c => c.ConnectAsync())
            .Should().ThrowExactlyAsync<ElgatoException>();
    }

    [Fact]
    public async Task ConnectSuccess()
    {
        SetupConnection();

        var testTask = new TaskCompletionSource<Task>();
        testTask.SetResult(Subject.ConnectAsync());

        await testTask.Should().CompleteWithinAsync(1.Seconds());

        _ = await Subject.Invoking(c => c.ConnectAsync())
            .Should().NotThrowAsync();
    }
}

using System.Net.WebSockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using ElgatoWaveSDK.Models;
using ElgatoWaveSDK.Tests.TestUtils;
using FluentAssertions;
using FluentAssertions.Extensions;
using Moq;
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

    [Fact]
    public async Task DisconnectSuccess()
    {
        SetupConnection();

        var testTask = new TaskCompletionSource<Task>();
        testTask.SetResult(Subject.ConnectAsync());

        await testTask.Should().CompleteWithinAsync(1.Seconds());

        _ = await Subject.Invoking(c => c.ConnectAsync())
            .Should().NotThrowAsync();

        Subject.Disconnect();

        MockSocket.Verify(c => c.Dispose(), Times.Once);
    }

    [Fact]
    public async Task DisconnectedSendReturnsDefault()
    {
        SetupConnection(WebSocketState.Closed);

        var reply = await Subject.GetAppInfo();
        reply.Should().BeNull();
    }

    //[Fact]
    //public async Task LargeDataReceived()
    //{
    //    SetupConnection();

    //    var returnObject = new List<ChannelInfo>();

    //    for (int i = 0; i < 15; i++)
    //    {
    //        var newObj = new ChannelInfo()
    //        {
    //            MixerName = "Name____________________" + i, BgColor = "Color____________________" + i, IconData = "Icon____________________" + i, MixId = "Id____________________" + i, Slider = "Slider____________________" + i,
    //            InputType = int.MaxValue, LocalVolumeIn = int.MaxValue, StreamVolumeIn = int.MaxValue, 
    //            IsAvailable = false, IsLocalInMuted = false, IsStreamInMuted = false, LocalMixFilterBypass = false, StreamMixFilterBypass = false,
    //            Filters = new List<Filter>()
    //        };

    //        for(int j = 0; j < 3; j++)
    //        {
    //            newObj.Filters.Add(new Filter()
    //            {
    //                FilterId = "Id____________________" + i + "_" + j, Name = "Name____________________" + i + "_" + j, PluginId = "Plugin____________________" + i + "_" + j,
    //                Active = false, 
    //            });
    //        }

    //        returnObject.Add(newObj);
    //    }

    //    SetupReply(returnObject);

    //    await Subject.ConnectAsync().ConfigureAwait(false);
    //    var result = await Subject.GetAllChannelInfo().ConfigureAwait(false);

    //    MockSocket.Verify(c => c.SendAsync(
    //        Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
    //        {
    //            Method = "getAllChannelInfo",
    //            Id = CommandId
    //        }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

    //    result.Should().NotBeNull();
    //    result?.Should().HaveCount(10);
    //    result?.ForEach(c => c.Filters.Should().HaveCount(3));
    //}
}

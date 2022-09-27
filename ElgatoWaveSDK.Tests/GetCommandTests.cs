using System.Net.WebSockets;
using System.Text;
using ElgatoWaveSDK.Models;
using ElgatoWaveSDK.Tests.TestUtils;
using FluentAssertions;
using Moq;
using Xunit;

namespace ElgatoWaveSDK.Tests;

public class GetCommandTests : TestBase
{
    public GetCommandTests() : base()
    {
        SetupConnection();
    }

    [Fact]
    public async Task GetAppInfo()
    {
        SetupReply(new ApplicationInfo()
        {
            Name = "TestName",
            Id = "TestId",
            AppVersion = new AppVersion()
            {
                MajorRelease = 1,
                MinorRelease = 2,
                BuildNumber = 3,
                PatchLevel = 4
            },
            InterfaceRevision = 5
        });

        await StartMock();

        var result = await Subject.GetAppInfo().ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
            {
                Method = "getApplicationInfo",
                Id = CommandId
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.Id.Should().Be("TestId");
        result?.Name.Should().Be("TestName");
        result?.AppVersion.Should().BeEquivalentTo(new AppVersion()
        {
            MajorRelease = 1,
            MinorRelease = 2,
            BuildNumber = 3,
            PatchLevel = 4
        });
        result?.InterfaceRevision.Should().Be(5);
        result?.Version.Should().BeEquivalentTo(new Version(1, 2, 3, 4));
    }

    [Fact]
    public async Task GetAllChannelInfo()
    {
        SetupReply(new List<ChannelInfo>()
        {
            new ChannelInfo()
            {
                BgColor = "Color-1",
                IconData = "IconData-1",
                InputType = 11,
                IsAvailable = true,
                IsLocalInMuted = false,
                IsStreamInMuted = true,
                LocalMixFilterBypass = false,
                StreamMixFilterBypass = true,
                LocalVolumeIn = 12,
                StreamVolumeIn = 13,
                MixId = "MixId-1",
                MixerName = "MixName-1",
            },
            new ChannelInfo()
            {
                BgColor = "Color-2",
                IconData = "IconData-2",
                InputType = 21,
                IsAvailable = false,
                IsLocalInMuted = true,
                IsStreamInMuted = false,
                LocalMixFilterBypass = true,
                StreamMixFilterBypass = false,
                LocalVolumeIn = 22,
                StreamVolumeIn = 23,
                MixId = "MixId-2",
                MixerName = "MixName-2",
                Filters = new List<Filter>()
                {
                    new Filter()
                    {
                        Name = "FilterName-1",
                        Active = false,
                        FilterId = "FilterId-1",
                        PluginId = "PluginId-1"
                    },
                    new Filter()
                    {
                        Name = "FilterName-2",
                        Active = true,
                        FilterId = "FilterId-2",
                        PluginId = "PluginId-2"
                    }
                }
            }
        });

        await StartMock();

        var result = await Subject.GetAllChannelInfo().ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
            {
                Method = "getAllChannelInfo",
                Id = CommandId
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.Should().HaveCount(2);

        var channelOne = result?.First();
        channelOne.Should().NotBeNull();
        channelOne?.BgColor.Should().Be("Color-1");
        channelOne?.IconData.Should().Be("IconData-1");
        channelOne?.InputType.Should().Be(11);
        channelOne?.IsAvailable.Should().BeTrue();
        channelOne?.IsLocalInMuted.Should().BeFalse();
        channelOne?.IsStreamInMuted.Should().BeTrue();
        channelOne?.LocalMixFilterBypass.Should().BeFalse();
        channelOne?.LocalVolumeIn.Should().Be(12);
        channelOne?.MixId.Should().Be("MixId-1");
        channelOne?.MixerName.Should().Be("MixName-1");
        channelOne?.Slider.Should().BeNull();
        channelOne?.StreamVolumeIn.Should().Be(13);
        channelOne?.StreamMixFilterBypass.Should().BeTrue();
        channelOne?.Filters.Should().BeNull();

        var channelTwo = result?.Last();
        channelTwo.Should().NotBeNull();
        channelTwo?.BgColor.Should().Be("Color-2");
        channelTwo?.IconData.Should().Be("IconData-2");
        channelTwo?.InputType.Should().Be(21);
        channelTwo?.IsAvailable.Should().BeFalse();
        channelTwo?.IsLocalInMuted.Should().BeTrue();
        channelTwo?.IsStreamInMuted.Should().BeFalse();
        channelTwo?.LocalMixFilterBypass.Should().BeTrue();
        channelTwo?.LocalVolumeIn.Should().Be(22);
        channelTwo?.MixId.Should().Be("MixId-2");
        channelTwo?.MixerName.Should().Be("MixName-2");
        channelTwo?.Slider.Should().BeNull();
        channelTwo?.StreamVolumeIn.Should().Be(23);
        channelTwo?.StreamMixFilterBypass.Should().BeFalse();
        channelTwo?.Filters.Should().HaveCount(2);

        var filterOne = channelTwo?.Filters?.First();
        filterOne.Should().NotBeNull();
        filterOne?.Name.Should().Be("FilterName-1");
        filterOne?.Active.Should().Be(false);
        filterOne?.FilterId.Should().Be("FilterId-1");
        filterOne?.PluginId.Should().Be("PluginId-1");

        var filterTwo = channelTwo?.Filters?.Last();
        filterTwo.Should().NotBeNull();
        filterTwo?.Name.Should().Be("FilterName-2");
        filterTwo?.Active.Should().Be(true);
        filterTwo?.FilterId.Should().Be("FilterId-2");
        filterTwo?.PluginId.Should().Be("PluginId-2");
    }

    [Fact]
    public async Task GetMicrophoneState()
    {
        SetupReply(new MicrophoneState()
        {
            IsMicrophoneConnected = true
        });

        await StartMock();

        var result = await Subject.GetMicrophoneState().ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
            {
                Method = "getMicrophoneState",
                Id = CommandId
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.IsMicrophoneConnected.Should().BeTrue();
    }

    [Fact]
    public async Task GetMicrophoneSettings()
    {
        SetupReply(new MicrophoneSettings()
        {
            MicrophoneBalance = 1,
            MicrophoneGain = 2,
            MicrophoneOutputVolume = 3,
            IsMicrophoneClipguardOn = true,
            IsMicrophoneLowcutOn = false
        });

        await StartMock();

        var result = await Subject.GetMicrophoneSettings().ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
            {
                Method = "getMicrophoneSettings",
                Id = CommandId
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.IsMicrophoneClipguardOn.Should().BeTrue();
        result?.IsMicrophoneLowcutOn.Should().BeFalse();
        result?.MicrophoneBalance.Should().Be(1);
        result?.MicrophoneGain.Should().Be(2);
        result?.MicrophoneOutputVolume.Should().Be(3);
    }

    [Fact]
    public async Task GetMonitoringState()
    {
        SetupReply(new MonitoringState()
        {
            IsLocalOutMuted = false,
            IsStreamOutMuted = true,
            LocalVolumeOut = 1,
            StreamVolumeOut = 2
        });

        await StartMock();

        var result = await Subject.GetMonitoringState().ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
            {
                Method = "getMonitoringState",
                Id = CommandId
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.IsLocalOutMuted.Should().BeFalse();
        result?.IsStreamOutMuted.Should().BeTrue();
        result?.LocalVolumeOut.Should().Be(1);
        result?.StreamVolumeOut.Should().Be(2);
    }

    [Fact]
    public async Task GetMonitorMixOutputList()
    {
        SetupReply(new MonitorMixOutputList()
        {
            MonitorMix = "SelectedMix",
            MonitorMixList = new List<MonitorMixList>()
            {
                new MonitorMixList()
                {
                    MonitorMix = "SelectedMix-1"
                },
                new MonitorMixList()
                {
                    MonitorMix = "SelectedMix-2"
                }
            }
        });

        await StartMock();

        var result = await Subject.GetMonitorMixOutputList().ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
            {
                Method = "getMonitorMixOutputList",
                Id = CommandId
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.MonitorMix.Should().Be("SelectedMix");
        result?.MonitorMixList.Should().HaveCount(2);

        var mixListOne = result?.MonitorMixList?.First();
        mixListOne.Should().NotBeNull();
        mixListOne?.MonitorMix.Should().Be("SelectedMix-1");

        var mixListTwo = result?.MonitorMixList?.Last();
        mixListTwo.Should().NotBeNull();
        mixListTwo?.MonitorMix.Should().Be("SelectedMix-2");
    }

    [Fact]
    public async Task GetSwitchState()
    {
        SetupReply(new SwitchState()
        {
            CurrentState = "LocalMix"
        });

        await StartMock();

        var result = await Subject.GetSwitchState().ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
            {
                Method = "getSwitchState",
                Id = CommandId
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().Be(MixType.LocalMix);
    }

    [Fact]
    public async Task GetSwitchState_Null()
    {
        SetupReply(new SwitchState()
        {
            CurrentState = ""
        });

        await StartMock();

        var result = await Subject.GetSwitchState().ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
            {
                Method = "getSwitchState",
                Id = CommandId
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().BeNull();
    }

    private async Task StartMock()
    {
        await Subject.ConnectAsync().ConfigureAwait(false);
        await Subject.WaitForReceiverToStart(2000);
    }
}

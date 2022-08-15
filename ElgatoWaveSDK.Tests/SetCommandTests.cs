using System.Net.WebSockets;
using System.Text;
using ElgatoWaveSDK.Models;
using ElgatoWaveSDK.Tests.TestUtils;
using FluentAssertions;
using Moq;
using Xunit;

namespace ElgatoWaveSDK.Tests;

public class SetCommandTests : TestBase
{
    public SetCommandTests() : base()
    {
        SetupConnection();
    }

    [Fact]
    public async Task SetMonitorMixOutput()
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

        await Subject.ConnectAsync().ConfigureAwait(false);
        var result = await Subject.SetMonitorMixOutput("input").ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<MonitorMixOutputList, MonitorMixOutputList>()
            {
                Method = "setMonitorMixOutput",
                Id = CommandId,
                Obj = new MonitorMixOutputList
                {
                    MonitorMix = "input"
                }
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
    public async Task SetMonitoringState()
    {
        SetupReply(new SwitchState()
        {
            CurrentState = MixType.LocalMix.ToString()
        });

        await Subject.ConnectAsync().ConfigureAwait(false);
        var result = await Subject.SetMonitoringState(MixType.LocalMix).ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<SwitchState, SwitchState>()
            {
                Method = "switchMonitoring",
                Id = CommandId,
                Obj = new SwitchState
                {
                    CurrentState = "LocalMix"
                }
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.CurrentState.Should().Be(MixType.LocalMix.ToString());
    }

    [Fact]
    public async Task SetMicrophoneSettings()
    {
        SetupReply(new MicrophoneSettings()
        {
            IsMicrophoneClipguardOn = false,
            IsMicrophoneLowcutOn = true,
            MicrophoneBalance = 1,
            MicrophoneGain = 2,
            MicrophoneOutputVolume = 3
        });

        await Subject.ConnectAsync().ConfigureAwait(false);
        var result = await Subject.SetMicrophoneSettings(2, 3,1,true,false).ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<MicrophoneSettings, MicrophoneSettings>()
            {
                Method = "setMicrophoneSettings",
                Id = CommandId,
                Obj = new MicrophoneSettings()
                {
                    IsMicrophoneClipguardOn = false,
                    IsMicrophoneLowcutOn = true,
                    MicrophoneBalance = 1,
                    MicrophoneGain = 2,
                    MicrophoneOutputVolume = 3
                }
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.IsMicrophoneClipguardOn.Should().BeFalse();
        result?.IsMicrophoneLowcutOn.Should().BeTrue();
        result?.MicrophoneBalance.Should().Be(1);
        result?.MicrophoneGain.Should().Be(2);
        result?.MicrophoneOutputVolume.Should().Be(3);
    }

    [Fact]
    public async Task SetOutputMixer()
    {
        SetupReply(new MonitoringState()
        {
            IsLocalOutMuted = false,
            IsStreamOutMuted = true,
            LocalVolumeOut = 1,
            StreamVolumeOut = 2
        });

        await Subject.ConnectAsync().ConfigureAwait(false);
        var result = await Subject.SetOutputMixer(1, true, 2, false).ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(
            Encoding.UTF8.GetBytes(new SocketBaseObject<MonitoringState, MonitoringState>()
            {
                Method = "setOutputMixer",
                Id = CommandId,
                Obj = new MonitoringState()
                {
                    IsLocalOutMuted = true,
                    IsStreamOutMuted = false,
                    LocalVolumeOut = 1,
                    StreamVolumeOut = 2
                }
            }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.IsLocalOutMuted.Should().BeFalse();
        result?.IsStreamOutMuted.Should().BeTrue();
        result?.LocalVolumeOut.Should().Be(1);
        result?.StreamVolumeOut.Should().Be(2);
    }

    [Fact]
    public async Task SetInputMixer()
    {
        SetupReply(new ChannelInfo()
        {
            BgColor = "Color-2",
            IconData = "IconData-2",
            InputType = 23,
            IsAvailable = false,
            IsLocalInMuted = false,
            IsStreamInMuted = false,
            LocalMixFilterBypass = false,
            LocalVolumeIn = 24,
            MixId = "MixId-2",
            MixerName = "MixerName-2",
            Slider = "Slider-2",
            StreamVolumeIn = 25,
            StreamMixFilterBypass = false,
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
        });

        await Subject.ConnectAsync().ConfigureAwait(false);
        var result = await Subject
            .SetInputMixer("id1", 1, false, 2, true, new List<Filter>(), false, true, MixType.LocalMix)
            .ConfigureAwait(false);

        MockSocket.Verify(c => c.SendAsync(Encoding.UTF8.GetBytes(new SocketBaseObject<ChannelInfo, ChannelInfo>()
        {
            Method = "setInputMixer",
            Id = CommandId,
            Obj = new ChannelInfo()
            {
                MixId = "id1",
                LocalVolumeIn = 1,
                IsLocalInMuted = false,
                StreamVolumeIn = 2,
                IsStreamInMuted = true,
                Filters = new List<Filter>(),
                LocalMixFilterBypass = false,
                StreamMixFilterBypass = true,
                Slider = "local"
            }
        }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

        result.Should().NotBeNull();
        result?.BgColor.Should().Be("Color-2");
        result?.IconData.Should().Be("IconData-2");
        result?.InputType.Should().Be(23);
        result?.IsAvailable.Should().BeFalse();
        result?.IsLocalInMuted.Should().BeFalse();
        result?.IsStreamInMuted.Should().BeFalse();
        result?.LocalMixFilterBypass.Should().BeFalse();
        result?.LocalVolumeIn.Should().Be(24);
        result?.MixId.Should().Be("MixId-2");
        result?.MixerName.Should().Be("MixerName-2");
        result?.Slider.Should().Be("Slider-2");
        result?.StreamVolumeIn.Should().Be(25);
        result?.StreamMixFilterBypass.Should().BeFalse();
        result?.Filters.Should().HaveCount(2);

        var filterOne = result?.Filters?.First();
        filterOne.Should().NotBeNull();
        filterOne?.Name.Should().Be("FilterName-1");
        filterOne?.Active.Should().BeFalse();
        filterOne?.FilterId.Should().Be("FilterId-1");
        filterOne?.PluginId.Should().Be("PluginId-1");

        var filterTwo = result?.Filters?.Last();
        filterTwo.Should().NotBeNull();
        filterTwo?.Name.Should().Be("FilterName-2");
        filterTwo?.Active.Should().BeTrue();
        filterTwo?.FilterId.Should().Be("FilterId-2");
        filterTwo?.PluginId.Should().Be("PluginId-2");
    }
}
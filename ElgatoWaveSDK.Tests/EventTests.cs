using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using ElgatoWaveSDK.Models;
using ElgatoWaveSDK.Tests.TestUtils;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace ElgatoWaveSDK.Tests;

public class EventTests : TestBase
{
    private int waitTime = 150;

    public EventTests() : base()
    {
        SetupConnection();
    }

    [Fact]
    public async Task MicrophoneStateChanged()
    {
        using var monitoredObject = Subject.Monitor();

        SetupReply(new MicrophoneState()
        {
            IsMicrophoneConnected = false
        }, "microphoneStateChanged", true);

        await Subject.ConnectAsync().ConfigureAwait(false);

        await Task.Delay(waitTime).ConfigureAwait(false);

        var eventMonitor = monitoredObject.Should().Raise(nameof(Subject.MicStateChanged));
        eventMonitor.WithArgs<MicrophoneState>(c => c.IsMicrophoneConnected == (bool?)false);
    }

    [Fact]
    public async Task MicrophoneSettingsChanged()
    {
        using var monitoredObject = Subject.Monitor();

        SetupReply(new MicrophoneSettings()
        {
            IsMicrophoneClipguardOn = false,
            IsMicrophoneLowcutOn = true,
            MicrophoneBalance = 1,
            MicrophoneGain = 2,
            MicrophoneOutputVolume = 3
        }, "microphoneSettingsChanged", true);

        await Subject.ConnectAsync().ConfigureAwait(false);

        await Task.Delay(waitTime).ConfigureAwait(false);

        var eventMonitor = monitoredObject.Should().Raise(nameof(Subject.MicSettingsChanged));
        eventMonitor.WithArgs<MicrophoneSettings>(c => 
            c.IsMicrophoneClipguardOn == false &&
            c.IsMicrophoneLowcutOn == true &&
            c.MicrophoneBalance == 1 &&
            c.MicrophoneGain == 2 &&
            c.MicrophoneOutputVolume == 3);
    }

    [Fact]
    public async Task LocalMonitorOutputChanged()
    {
        using var monitoredObject = Subject.Monitor();

        SetupReply(new
        {
            monitorMix = "Test"
        }, "localMonitorOutputChanged", true);

        await Subject.ConnectAsync().ConfigureAwait(false);

        await Task.Delay(waitTime).ConfigureAwait(false);

        var eventMonitor = monitoredObject.Should().Raise(nameof(Subject.LocalMonitorOutputChanged));
        eventMonitor.WithArgs<string>(c => c == "Test");
    }

    [Fact]
    public async Task MonitorSwitchOutputChanged()
    {
        using var monitoredObject = Subject.Monitor();

        SetupReply(new
        {
            switchState = "LocalMix"
        }, "monitorSwitchOutputChanged", true);

        await Subject.ConnectAsync().ConfigureAwait(false);

        await Task.Delay(waitTime).ConfigureAwait(false);

        var eventMonitor = monitoredObject.Should().Raise(nameof(Subject.MonitorSwitchOutputChanged));
        eventMonitor.WithArgs<MixType>(c => c == MixType.LocalMix);
    }

    [Fact]
    public async Task ChannelsChanged()
    {
        using var monitoredObject = Subject.Monitor();

        SetupReply(new
        {
            channels = new List<ChannelInfo>()
            {
                new ChannelInfo()
                {
                    MixerName = "Test-1"
                },
                new ChannelInfo()
                {
                    MixerName = "Test-2"
                },
            }
        }, "channelsChanged", true);

        await Subject.ConnectAsync().ConfigureAwait(false);

        await Task.Delay(waitTime).ConfigureAwait(false);

        var eventMonitor = monitoredObject.Should().Raise(nameof(Subject.ChannelsChanged));
        eventMonitor.WithArgs<List<ChannelInfo>>(c => c.Last().MixerName == "Test-2" && c.Count == 2);
    }

    [Fact]
    public async Task OutputMixerChanged()
    {
        using var monitoredObject = Subject.Monitor();

        SetupReply(new MonitoringState()
        {
            IsLocalOutMuted = true,
            IsStreamOutMuted = false,
            LocalVolumeOut = 10,
            StreamVolumeOut = 90
        }, "outputMixerChanged", true);

        await Subject.ConnectAsync().ConfigureAwait(false);

        await Task.Delay(waitTime).ConfigureAwait(false);

        var eventMonitor = monitoredObject.Should().Raise(nameof(Subject.OutputMixerChanged));
        eventMonitor.WithArgs<MonitoringState>(c => 
            c.IsLocalOutMuted == true &&
            c.IsStreamOutMuted == false &&
            c.LocalVolumeOut == 10 &&
            c.StreamVolumeOut == 90);
    }

    [Fact]
    public async Task InputMixerChanged()
    {
        using var monitoredObject = Subject.Monitor();

        SetupReply(new ChannelInfo()
        {
            MixerName = "Test-1"
        }, "inputMixerChanged", true);

        await Subject.ConnectAsync().ConfigureAwait(false);

        await Task.Delay(waitTime).ConfigureAwait(false);

        var eventMonitor = monitoredObject.Should().Raise(nameof(Subject.InputMixerChanged));
        eventMonitor.WithArgs<ChannelInfo>(c => c.MixerName == "Test-1");
    }
}

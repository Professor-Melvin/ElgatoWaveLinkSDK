using System.Net.WebSockets;
using System.Text;
using ElgatoWaveSDK.Models;
using ElgatoWaveSDK.Tests.TestUtils;
using FluentAssertions;
using Moq;
using Xunit;

namespace ElgatoWaveSDK.Tests
{
    public class GetCommandTests : TestBase
    {
        public GetCommandTests() : base()
        {
            SetupConnection();
        }

        [Fact]
        public async Task GetAppInfo()
        {
            SetupReply("{  \"appId\": \"TestId\",  \"appName\": \"TestName\",  \"appVersion\": {    \"appVersionBuildNumber\": 3,    \"appVersionMajorRelease\": 1,    \"appVersionMinorRelease\": 2,    \"appVersionPatchLevel\": 4  },  \"interfaceRevision\": 5}");

            await Subject.ConnectAsync().ConfigureAwait(false);
            var result = await Subject.GetAppInfo().ConfigureAwait(false);

            mockSocket.Verify(c => c.SendAsync(
                Encoding.UTF8.GetBytes(new SocketBaseObject<ApplicationInfo, string>()
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
            SetupReply("[    {      \"bgColor\": \"Color-1\",      \"filters\": [],      \"slider\": \"Slider-1\",      \"deltaLinked\": 12,      \"iconData\": \"IconData-1\",      \"inputType\": 13,      \"isAvailable\": true,      \"isLinked\": true,      \"isLocalInMuted\": true,      \"isStreamInMuted\": true,      \"localVolumeIn\": 14,      \"localMixFilterBypass\": true,      \"mixId\": \"MixId-1\",      \"mixerName\": \"MixerName-1\",      \"streamMixFilterBypass\": true,      \"streamVolumeIn\": 15    },    {      \"bgColor\": \"Color-2\",      \"filters\": [        {          \"active\": false,          \"filterID\": \"FilterID-1\",          \"name\": \"FilterName-1\",          \"pluginID\": \"PluginId-1\"        },        {          \"active\": true,          \"filterID\": \"FilterID-2\",          \"name\": \"FilterName-2\",          \"pluginID\": \"PluginId-2\"        }      ],      \"slider\": \"Slider-2\",      \"deltaLinked\": 22,      \"iconData\": \"IconData-2\",      \"inputType\": 23,      \"isAvailable\": false,      \"isLinked\": false,      \"isLocalInMuted\": false,      \"isStreamInMuted\": false,      \"localVolumeIn\": 24,      \"localMixFilterBypass\": false,      \"mixId\": \"MixId-2\",      \"mixerName\": \"MixerName-2\",      \"streamMixFilterBypass\": false,      \"streamVolumeIn\": 25    }  ]");

            await Subject.ConnectAsync().ConfigureAwait(false);
            var result = await Subject.GetAllChannelInfo().ConfigureAwait(false);

            mockSocket.Verify(c => c.SendAsync(
                Encoding.UTF8.GetBytes(new SocketBaseObject<ApplicationInfo, string>()
                {
                    Method = "getAllChannelInfo",
                    Id = CommandId
                }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

            result.Should().NotBeNull();
            result?.Should().HaveCount(2);

            var channelOne = result?.First();
            channelOne.Should().NotBeNull();
            channelOne?.DeltaLinked.Should().Be(12);
            channelOne?.BgColor.Should().Be("Color-1");
            channelOne?.IconData.Should().Be("IconData-1");
            channelOne?.InputType.Should().Be(13);
            channelOne?.IsAvailable.Should().BeTrue();
            channelOne?.IsLinked.Should().BeTrue();
            channelOne?.IsLocalInMuted.Should().BeTrue();
            channelOne?.IsStreamInMuted.Should().BeTrue();
            channelOne?.LocalMixFilterBypass.Should().BeTrue();
            channelOne?.LocalVolumeIn.Should().Be(14);
            channelOne?.MixId.Should().Be("MixId-1");
            channelOne?.MixerName.Should().Be("MixerName-1");
            channelOne?.Slider.Should().Be("Slider-1");
            channelOne?.StreamVolumeIn.Should().Be(15);
            channelOne?.StreamMixFilterBypass.Should().BeTrue();
            channelOne?.Filters.Should().BeEmpty();

            var channelTwo = result?.Last();
            channelTwo.Should().NotBeNull();
            channelTwo?.DeltaLinked.Should().Be(22);
            channelTwo?.BgColor.Should().Be("Color-2");
            channelTwo?.IconData.Should().Be("IconData-2");
            channelTwo?.InputType.Should().Be(23);
            channelTwo?.IsAvailable.Should().BeFalse();
            channelTwo?.IsLinked.Should().BeFalse();
            channelTwo?.IsLocalInMuted.Should().BeFalse();
            channelTwo?.IsStreamInMuted.Should().BeFalse();
            channelTwo?.LocalMixFilterBypass.Should().BeFalse();
            channelTwo?.LocalVolumeIn.Should().Be(24);
            channelTwo?.MixId.Should().Be("MixId-2");
            channelTwo?.MixerName.Should().Be("MixerName-2");
            channelTwo?.Slider.Should().Be("Slider-2");
            channelTwo?.StreamVolumeIn.Should().Be(25);
            channelTwo?.StreamMixFilterBypass.Should().BeFalse();
            channelTwo?.Filters.Should().HaveCount(2);

            var filterOne = channelTwo?.Filters?.First();
            filterOne.Should().NotBeNull();
            filterOne?.Name.Should().Be("FilterName-1");
            filterOne?.Active.Should().Be(false);
            filterOne?.FilterId.Should().Be("FilterID-1");
            filterOne?.PluginId.Should().Be("PluginId-1");

            var filterTwo = channelTwo?.Filters?.Last();
            filterTwo.Should().NotBeNull();
            filterTwo?.Name.Should().Be("FilterName-2");
            filterTwo?.Active.Should().Be(true);
            filterTwo?.FilterId.Should().Be("FilterID-2");
            filterTwo?.PluginId.Should().Be("PluginId-2");
        }

        [Fact]
        public async Task GetMicrophoneState()
        {
            SetupReply("{  \"isMicrophoneConnected\": false}");

            await Subject.ConnectAsync().ConfigureAwait(false);
            var result = await Subject.GetMicrophoneState().ConfigureAwait(false);

            mockSocket.Verify(c => c.SendAsync(
                Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
                {
                    Method = "getMicrophoneState",
                    Id = CommandId
                }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

            result.Should().NotBeNull();
            result?.IsMicrophoneConnected.Should().BeFalse();
        }

        [Fact]
        public async Task GetMicrophoneSettings()
        {
            SetupReply("{    \"isMicrophoneClipguardOn\": false,    \"isMicrophoneLowcutOn\": true,    \"microphoneBalance\": 1,    \"microphoneGain\": 2,    \"microphoneOutputVolume\": 3  }");

            await Subject.ConnectAsync().ConfigureAwait(false);
            var result = await Subject.GetMicrophoneSettings().ConfigureAwait(false);

            mockSocket.Verify(c => c.SendAsync(
                Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
                {
                    Method = "getMicrophoneSettings",
                    Id = CommandId
                }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

            result.Should().NotBeNull();
            result?.IsMicrophoneClipguardOn.Should().BeFalse();
            result?.IsMicrophoneLowcutOn.Should().BeTrue();
            result?.MicrophoneBalance.Should().Be(1);
            result?.MicrophoneGain.Should().Be(2);
            result?.MicrophoneOutputVolume.Should().Be(3);
        }

        [Fact]
        public async Task GetMonitoringState()
        {
            SetupReply("{    \"isLocalOutMuted\": false,    \"isStreamOutMuted\": true,    \"localVolumeOut\": 1,    \"streamVolumeOut\": 2  }");

            await Subject.ConnectAsync().ConfigureAwait(false);
            var result = await Subject.GetMonitoringState().ConfigureAwait(false);

            mockSocket.Verify(c => c.SendAsync(
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
            SetupReply("{    \"monitorMix\": \"SelectedMix\",    \"monitorMixList\": [      {        \"monitorMix\": \"SelectedMix-1\"      },      {        \"monitorMix\": \"SelectedMix-2\"      }    ]  }");

            await Subject.ConnectAsync().ConfigureAwait(false);
            var result = await Subject.GetMonitorMixOutputList().ConfigureAwait(false);

            mockSocket.Verify(c => c.SendAsync(
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
            SetupReply("{  \"switchState\": \"localMix\"}");

            await Subject.ConnectAsync().ConfigureAwait(false);
            var result = await Subject.GetSwitchState().ConfigureAwait(false);

            mockSocket.Verify(c => c.SendAsync(
                Encoding.UTF8.GetBytes(new SocketBaseObject<string, string>()
                {
                    Method = "getSwitchState",
                    Id = CommandId
                }.ToJson()), WebSocketMessageType.Text, true, It.IsAny<CancellationToken>()), Times.Once);

            result.Should().NotBeNull();
            result?.switchState.Should().Be("localMix");
        }
    }
}

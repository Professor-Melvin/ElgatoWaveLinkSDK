using ElgatoWaveSDK.Models;
using ElgatoWaveSDK.Tests.TestUtils;
using FluentAssertions;
using Xunit;

namespace ElgatoWaveSDK.Tests
{
    public class ModelTests
    {
        [Theory]
        [InlineData(typeof(ApplicationInfo)),
         InlineData(typeof(ChannelInfo)),
         InlineData(typeof(MicrophoneSettings)),
         InlineData(typeof(MicrophoneState)),
         InlineData(typeof(MonitoringState)),
         InlineData(typeof(MonitorMixOutputList)),
         InlineData(typeof(MonitorMixList)),
         InlineData(typeof(SwitchState))
        ]
        public void SdkModels(Type classType)
        {

            var testData = ModelUtils.GenerateValues(classType);

            ModelUtils.Verify(classType, testData.Item1, testData.values);

            if (classType == typeof(ApplicationInfo))
            {
                var appInfo = testData.Item1 as ApplicationInfo;
                var versionInputValue = testData.values[nameof(appInfo.AppVersion)] as AppVersion;

                appInfo?.Should().NotBeNull();
                appInfo?.Version.Should().Be(new Version(versionInputValue?.MajorRelease ?? 0,
                    versionInputValue?.MinorRelease ?? 0, versionInputValue?.BuildNumber ?? 0,
                    versionInputValue?.PatchLevel ?? 0));
            }
        }

        [Fact]
        public void SocketBaseObject()
        {
            Type classType = typeof(SocketBaseObject<SwitchState, SwitchState>);
            var testData = ModelUtils.GenerateValues(classType);

            ModelUtils.Verify(classType,testData.Item1,testData.values);

            var obj = testData.Item1 as SocketBaseObject<SwitchState, SwitchState>;
            obj.Should().NotBeNull();
            obj?.JsonRpc.Should().Be("2.0");
        }

        [Fact]
        public void SocketBaseObject_Json_String()
        {
            var subject = new SocketBaseObject<string, string>()
            {
                Obj = "Obj",
                Result = "Result",
                Id = 123,
                Method = "Method"
            };

            var json = subject.ToJson();
            json.Should().NotBeNull();
            json.Should().Be("{\"id\":123,\"jsonrpc\":\"2.0\",\"method\":\"Method\",\"params\":\"Obj\",\"result\":\"Result\"}");
        }

        [Fact]
        public void SocketBaseObject_Json_SwitchState()
        {
            var subject = new SocketBaseObject<SwitchState, SwitchState>()
            {
                Obj = new SwitchState()
                {
                    switchState = "ObjState"
                },
                Result = new SwitchState()
                {
                    switchState = "ResultState"
                },
                Id = 123,
                Method = "Method"
            };

            var json = subject.ToJson();
            json.Should().NotBeNull();
            json.Should().Be("{\"id\":123,\"jsonrpc\":\"2.0\",\"method\":\"Method\",\"params\":{\"switchState\":\"ObjState\"},\"result\":{\"switchState\":\"ResultState\"}}");
        }
    }
}

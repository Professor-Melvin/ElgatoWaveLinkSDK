using System.Text.Json;
using System.Threading.Channels;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveLinkEmulator;

internal static class ObjectGenerator
{
    public static ApplicationInfo GetApplicationInfo() =>
        new()
        {
            AppVersion = new AppVersion()
            {
                MajorRelease = 90,
                BuildNumber = 90,
                MinorRelease = 90,
                PatchLevel = 90
            },
            Id = "Emulator90",
            InterfaceRevision = 90,
            Name = "Emulator"
        };

    public static List<ChannelInfo>? GetChannels()
    {
        return JsonSerializer.Deserialize<List<ChannelInfo>?>("[{\"bgColor\":\"#1817B8\",\"filters\":[{\"active\":true,\"filterID\":\"34D51668-6801-4836-832D-EA66BDD9451C\",\"name\":\"RoughRider3\",\"pluginID\":\"e4022018\"}],\"iconData\":\"\",\"inputType\":1,\"isAvailable\":true,\"isLocalInMuted\":true,\"isStreamInMuted\":false,\"localMixFilterBypass\":true,\"localVolumeIn\":40,\"mixId\":\"pcm_in_01_c_00_sd1\",\"mixerName\":\"Elgato Wave:3\",\"streamMixFilterBypass\":true,\"streamVolumeIn\":100},{\"bgColor\":\"#1817B8\",\"filters\":[],\"iconData\":\"\",\"inputType\":3,\"isAvailable\":true,\"isLocalInMuted\":false,\"isStreamInMuted\":false,\"localMixFilterBypass\":false,\"localVolumeIn\":44,\"mixId\":\"pcm_out_01_v_00_sd2\",\"mixerName\":\"System\",\"streamMixFilterBypass\":false,\"streamVolumeIn\":100},{\"bgColor\":\"#CFD924\",\"filters\":[],\"iconData\":\"\",\"inputType\":3,\"isAvailable\":true,\"isLocalInMuted\":false,\"isStreamInMuted\":false,\"localMixFilterBypass\":false,\"localVolumeIn\":100,\"mixId\":\"pcm_out_01_v_06_sd5\",\"mixerName\":\"Voice Chat\",\"streamMixFilterBypass\":false,\"streamVolumeIn\":100},{\"bgColor\":\"#FF00E8\",\"filters\":[],\"iconData\":\"\",\"inputType\":3,\"isAvailable\":true,\"isLocalInMuted\":false,\"isStreamInMuted\":false,\"localMixFilterBypass\":false,\"localVolumeIn\":100,\"mixId\":\"pcm_out_01_v_02_sd3\",\"mixerName\":\"Music\",\"streamMixFilterBypass\":false,\"streamVolumeIn\":100},{\"bgColor\":\"#B521FF\",\"filters\":[],\"iconData\":\"\",\"inputType\":3,\"isAvailable\":true,\"isLocalInMuted\":false,\"isStreamInMuted\":false,\"localMixFilterBypass\":false,\"localVolumeIn\":100,\"mixId\":\"pcm_out_01_v_04_sd4\",\"mixerName\":\"Browser\",\"streamMixFilterBypass\":false,\"streamVolumeIn\":100},{\"bgColor\":\"#F2315B\",\"filters\":[],\"iconData\":\"\",\"inputType\":3,\"isAvailable\":true,\"isLocalInMuted\":false,\"isStreamInMuted\":false,\"localMixFilterBypass\":false,\"localVolumeIn\":30,\"mixId\":\"pcm_out_01_v_10_sd7\",\"mixerName\":\"Game\",\"streamMixFilterBypass\":false,\"streamVolumeIn\":100},{\"bgColor\":\"#FF6C3E\",\"filters\":[],\"iconData\":\"\",\"inputType\":3,\"isAvailable\":true,\"isLocalInMuted\":false,\"isStreamInMuted\":false,\"localMixFilterBypass\":false,\"localVolumeIn\":100,\"mixId\":\"pcm_out_01_v_08_sd6\",\"mixerName\":\"SFX\",\"streamMixFilterBypass\":false,\"streamVolumeIn\":100},{\"bgColor\":\"#23DE63\",\"filters\":[],\"iconData\":\"\",\"inputType\":3,\"isAvailable\":true,\"isLocalInMuted\":false,\"isStreamInMuted\":false,\"localMixFilterBypass\":false,\"localVolumeIn\":100,\"mixId\":\"pcm_out_01_v_12_sd8\",\"mixerName\":\"Aux 1\",\"streamMixFilterBypass\":false,\"streamVolumeIn\":100},{\"bgColor\":\"#23DEC0\",\"filters\":[],\"iconData\":\"\",\"inputType\":3,\"isAvailable\":true,\"isLocalInMuted\":false,\"isStreamInMuted\":false,\"localMixFilterBypass\":false,\"localVolumeIn\":100,\"mixId\":\"pcm_out_01_v_14_sd9\",\"mixerName\":\"Aux 2\",\"streamMixFilterBypass\":false,\"streamVolumeIn\":100}]");
    }

    public static MicrophoneState GetMicrophoneState() =>
        new()
        {
            IsMicrophoneConnected = true
        };

    public static MicrophoneSettings GetMicrophoneSettings() =>
        new ()
        {
            IsMicrophoneLowcutOn = false,
            MicrophoneBalance = 50,
            MicrophoneGain = 50,
            MicrophoneOutputVolume = 50,
            IsMicrophoneClipguardOn = true
        };

    public static MonitoringState GetMonitoringState() =>
        new ()
        {
            IsStreamOutMuted = true,
            LocalVolumeOut = 75,
            StreamVolumeOut = 100,
            IsLocalOutMuted = false
        };

    public static SwitchState GetSwitchState() =>
        new ()
        {
            CurrentState = MixType.LocalMix.ToString()
        };

    public static MonitorMixOutputList GetMonitorMixOutputList() =>
        new ()
        {
            MonitorMix = "SelectedOutput",
            MonitorMixList = new List<MonitorMixList>()
            {
                new ()
                {
                    MonitorMix = "SelectedOutput"
                },
                new ()
                {
                    MonitorMix = "PossibleOutput-1"
                },
                new ()
                {
                    MonitorMix = "PossibleOutput-2"
                }
            }
        };

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveLinkEmulator;
internal static class ObjectGenerator
{
    public static ApplicationInfo GetApplicationInfo()
    {
        return new ApplicationInfo()
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
    }

    public static List<ChannelInfo> GetChannels()
    {
        List<ChannelInfo> reply = new List<ChannelInfo>();

        reply
    }
}

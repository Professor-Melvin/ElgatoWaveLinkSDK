using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using ElgatoWaveAPI;
using ElgatoWaveAPI.Models;

namespace TestConsole
{
    internal class Program
    {
        static async Task Main()
        {
            var client = new ElgatoWaveAPIClient("localhost", 1825);

            if (await client.ConnectAsync())
            {
                Console.WriteLine("Connected!\n\n");

                var appInfo = await client.GetAppInfo();
                Console.WriteLine($"App Settings:" +
                                  $"\n\tApp ID: {appInfo.AppId}" +
                                  $"\n\tApp Name: {appInfo.AppName}" +
                                  $"\n\tBuild: {appInfo.Build}" +
                                  $"\n\tVersion: {appInfo.Version}" +
                                  $"\n\tInterface Revision: {appInfo.InterfaceRevision}\n");

                var channelInfos = await client.GetAllChannelInfo();
                Console.WriteLine($"All Channel Info:");
                channelInfos.ForEach(c =>
                {
                    Console.Write($"\n\t{c.MixerName}:" +
                                  $"\n\t\tID: {c.MixId}" +
                                  $"\n\t\tColor: ");
                    Color channelColor = ColorTranslator.FromHtml(c.BgColor);
                    Console.ForegroundColor = ClosestConsoleColor(channelColor.R, channelColor.G, channelColor.B);
                    Console.Write(c.BgColor);
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write($"\n\t\tInput Type: {c.InputType}" +
                                  $"\n\t\tIcon Data: {c.IconData}" +
                                  $"\n\t\tLocal Vol: {c.LocalVolumeIn}" +
                                  $"\n\t\tIs Local Muted: {c.IsLocalInMuted}" +
                                  $"\n\t\tStream Vol: {c.StreamVolumeIn}" +
                                  $"\n\t\tIs Stream Muted: {c.IsStreamInMuted}" +
                                  $"\n\t\tDelta Linked: {c.DeltaLinked}" +
                                  $"\n\t\tIs Available: {c.IsAvailable}");
                });

                //var newSetting = await client.SetInputMixer(channelInfos[3].MixId, channelInfos[3].);

                //var newSetting = await client.SetOutputMixer(50, false, 50, false);

                var micState = await client.GetMicrophoneState();
                Console.WriteLine($"\nMic Connected: {micState.IsMicrophoneConnected}");

                var micSettings = await client.GetMicrophoneSettings();
                Console.WriteLine($"Mic Setting:" +
                                  $"\n\tMic Balance: {micSettings.MicrophoneBalance}" +
                                  $"\n\tMic Gain: {micSettings.MicrophoneGain}" +
                                  $"\n\tMic Output Vol: {micSettings.MicrophoneOutputVolume}" +
                                  $"\n\tLowCut On: {micSettings.IsMicrophoneLowcutOn}" +
                                  $"\n\tClipGuard On: {micSettings.IsMicrophoneClipguardOn}");

                var monitoringState = await client.GetMonitoringState();
                Console.WriteLine($"Monitoring State:" +
                                  $"\n\tLocal Vol: {monitoringState.LocalVolumeOut}" +
                                  $"\n\tLocal Muted: {monitoringState.IsLocalOutMuted}" +
                                  $"\n\tStream Vol: {monitoringState.StreamVolumeOut}" +
                                  $"\n\tStream Muted: {monitoringState.IsStreamOutMuted}");

                var monitorMixOutputList = await client.GetMonitorMixOutputList();
                Console.WriteLine($"Monitor Output: {monitorMixOutputList.MonitorMix}" +
                                  $"\nPossible Outputs:");
                monitorMixOutputList.MonitorMixList.ForEach(o => Console.WriteLine($"\t{o.MonitorMix}"));

                var switchState = await client.GetSwitchState();
                Console.WriteLine($"Switch State: {switchState.switchState}");

                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();

                client.Disconnect();
            }
            else
            {
                Console.WriteLine("Failed to connect");
            }
        }

        private static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
        {
            ConsoleColor ret = 0;
            double rr = r, gg = g, bb = b, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = System.Drawing.Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }
            return ret;
        }
    }
}

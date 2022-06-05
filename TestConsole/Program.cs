using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ElgatoWaveAPI;
using ElgatoWaveAPI.Models;
using Newtonsoft.Json;
using Websocket.Client;

namespace TestConsole
{
    internal class Program
    {
        static async Task Main()
        {
            ElgatoWaveClient client = new ElgatoWaveClient();
            client.MicStateChanged += (sender, state) =>
            {
                Console.WriteLine($"\nUpdate | Mic Connected: {state.IsMicrophoneConnected}");
            };
            client.MicSettingsChanged += (sender, micSettings) =>
            {
                Console.WriteLine($"Update | Mic Setting:" +
                                  $"\n\tMic Balance: {micSettings.MicrophoneBalance}" +
                                  $"\n\tMic Gain: {micSettings.MicrophoneGain}" +
                                  $"\n\tMic Output Vol: {micSettings.MicrophoneOutputVolume}" +
                                  $"\n\tLowCut On: {micSettings.IsMicrophoneLowcutOn}" +
                                  $"\n\tClipGuard On: {micSettings.IsMicrophoneClipguardOn}");
            };
            client.OutputMixerChanged += (sender, monitoringState) =>
            {
                Console.WriteLine($"Update | Monitoring State:" +
                                  $"\n\tLocal Vol: {monitoringState.LocalVolumeOut}" +
                                  $"\n\tLocal Muted: {monitoringState.IsLocalOutMuted}" +
                                  $"\n\tStream Vol: {monitoringState.StreamVolumeOut}" +
                                  $"\n\tStream Muted: {monitoringState.IsStreamOutMuted}");
            };
            client.MonitorSwitchOutputChanged += (sender, mix) =>
            {
                Console.WriteLine($"Update | Switch State: {mix}");
            };
            client.LocalMonitorOutputChanged += (sender, s) =>
            {
                Console.WriteLine($"Update | Monitor Output: {s}");
            };
            client.InputMixerChanged += (sender, c) =>
            {
                Console.WriteLine($"Update | Input Changed:" +
                                  $"\n\tInput Type: {c.InputType}" +
                                  $"\n\tIcon Data: {c.IconData}" +
                                  $"\n\tLocal Vol: {c.LocalVolumeIn}" +
                                  $"\n\tIs Local Muted: {c.IsLocalInMuted}" +
                                  $"\n\tStream Vol: {c.StreamVolumeIn}" +
                                  $"\n\tIs Stream Muted: {c.IsStreamInMuted}" +
                                  $"\n\tIs LocalMixFilterBypass: {c.LocalMixFilterBypass}" +
                                  $"\n\tIs StreamMixFilterBypass: {c.StreamMixFilterBypass}" +
                                  $"\n\tFilter Count: {c.Filters.Count}" +
                                  $"\n\tIs Available: {c.IsAvailable}");
            };
            client.ChannelsChanged += (sender, list) =>
            {
                list.ForEach(c =>
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
                                  $"\n\tIs LocalMixFilterBypass: {c.LocalMixFilterBypass}" +
                                  $"\n\tIs StreamMixFilterBypass: {c.StreamMixFilterBypass}" +
                                  $"\n\tFilter Count: {c.Filters.Count}" +
                                  $"\n\t\tIs Available: {c.IsAvailable}");
                });
            };


            if (await client.ConnectAsync().ConfigureAwait(false))
            {
                Console.WriteLine("Connected!\n\n");

                Console.WriteLine("\nPress any key to start test...");
                Console.ReadKey();

                var appInfo = await client.GetAppInfo();
                Console.WriteLine($"App Settings:" +
                                  $"\n\tApp ID: {appInfo.Id}" +
                                  $"\n\tApp Name: {appInfo.Name}" +
                                  $"\n\tVersion: {appInfo.Version}" +
                                  $"\n\tInterface Revision: {appInfo.InterfaceRevision}\n");

                var channelInfos = await client.GetAllChannelInfo().ConfigureAwait(false);
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
                                  $"\n\tIs LocalMixFilterBypass: {c.LocalMixFilterBypass}" +
                                  $"\n\tIs StreamMixFilterBypass: {c.StreamMixFilterBypass}" +
                                  $"\n\tFilter Count: {c.Filters.Count}" +
                                  $"\n\t\tIs Available: {c.IsAvailable}");
                });


                //var newSetting = await client.SetInputMixer(channelInfos[3].MixId, channelInfos[3].);

                //var newSetting = await client.SetOutputMixer(80, false, 50, true);

                var micState = await client.GetMicrophoneState();
                Console.WriteLine($"\nMic Connected: {micState.IsMicrophoneConnected}");

                var micSettings = await client.GetMicrophoneSettings();
                Console.WriteLine($"Mic Setting:" +
                                  $"\n\tMic Balance: {micSettings.MicrophoneBalance}" +
                                  $"\n\tMic Gain: {micSettings.MicrophoneGain}" +
                                  $"\n\tMic Output Vol: {micSettings.MicrophoneOutputVolume}" +
                                  $"\n\tLowCut On: {micSettings.IsMicrophoneLowcutOn}" +
                                  $"\n\tClipGuard On: {micSettings.IsMicrophoneClipguardOn}");
                micSettings.MicrophoneGain = 100;


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

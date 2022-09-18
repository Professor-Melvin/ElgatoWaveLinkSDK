using System;
using System.Drawing;
using System.Threading.Tasks;

namespace ElgatoWaveSDK.TestRig;

internal static class Program
{
    public static async Task Main()
    {
        ElgatoWaveClient client = new();
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
                              $"\n\tIs Available: {c.IsAvailable}");
        };
        client.ChannelsChanged += (sender, list) =>
        {
            list.ForEach(c =>
            {
                Console.Write($"\n\t{c.MixerName}:" +
                              $"\n\t\tID: {c.MixId}" +
                              $"\n\t\tColor: ");
                var channelColor = ColorTranslator.FromHtml(c.BgColor);
                Console.ForegroundColor = ClosestConsoleColour(channelColor.R, channelColor.G, channelColor.B);
                Console.Write(c.BgColor);
                Console.ForegroundColor = ConsoleColor.White;

                Console.Write($"\n\t\tInput Type: {c.InputType}" +
                              $"\n\t\tIcon Data: {c.IconData}" +
                              $"\n\t\tLocal Vol: {c.LocalVolumeIn}" +
                              $"\n\t\tIs Local Muted: {c.IsLocalInMuted}" +
                              $"\n\t\tStream Vol: {c.StreamVolumeIn}" +
                              $"\n\t\tIs Stream Muted: {c.IsStreamInMuted}" +
                              $"\n\t\tIs Available: {c.IsAvailable}");
            });
        };

        try
        {
            await client.ConnectAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to connect: " + ex.Message);
            return;
        }

        Console.WriteLine("Connected!\n\n");

        Console.WriteLine("\nPress any key to start test...");
        Console.ReadKey();

        var appInfo = await client.GetAppInfo().ConfigureAwait(false);
        Console.WriteLine($"App Settings:" +
                          $"\n\tApp ID: {appInfo.Id}" +
                          $"\n\tApp Name: {appInfo.Name}" +
                          $"\n\tVersion: {appInfo.Version}" +
                          $"\n\tInterface Revision: {appInfo.InterfaceRevision}\n");

        var channelInfos = await client.GetAllChannelInfo().ConfigureAwait(false);
        Console.WriteLine($"All Channel Info:");
        channelInfos?.ForEach(c =>
        {
            Console.Write($"\n\t{c.MixerName}:" +
                          $"\n\t\tID: {c.MixId}" +
                          $"\n\t\tColor: ");
            var channelColour = ColorTranslator.FromHtml(c.BgColor ?? "");
            Console.ForegroundColor = ClosestConsoleColour(channelColour.R, channelColour.G, channelColour.B);
            Console.Write(c.BgColor);
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write($"\n\t\tInput Type: {c.InputType}" +
                          $"\n\t\tIcon Data: {c.IconData}" +
                          $"\n\t\tLocal Vol: {c.LocalVolumeIn}" +
                          $"\n\t\tIs Local Muted: {c.IsLocalInMuted}" +
                          $"\n\t\tStream Vol: {c.StreamVolumeIn}" +
                          $"\n\t\tIs Stream Muted: {c.IsStreamInMuted}" +
                          $"\n\t\tIs Available: {c.IsAvailable}");
        });

        var micState = await client.GetMicrophoneState().ConfigureAwait(false);
        Console.WriteLine($"\nMic Connected: {micState?.IsMicrophoneConnected ?? false}");

        var micSettings = await client.GetMicrophoneSettings().ConfigureAwait(false);
        Console.WriteLine($"Mic Setting:" +
                          $"\n\tMic Balance: {micSettings?.MicrophoneBalance}" +
                          $"\n\tMic Gain: {micSettings?.MicrophoneGain}" +
                          $"\n\tMic Output Vol: {micSettings?.MicrophoneOutputVolume}" +
                          $"\n\tLowCut On: {micSettings?.IsMicrophoneLowcutOn}" +
                          $"\n\tClipGuard On: {micSettings?.IsMicrophoneClipguardOn}");
        if (micSettings != null)
        {
            micSettings.MicrophoneGain = 100;
        }


        var monitoringState = await client.GetMonitoringState().ConfigureAwait(false);
        Console.WriteLine($"Monitoring State:" +
                          $"\n\tLocal Vol: {monitoringState?.LocalVolumeOut}" +
                          $"\n\tLocal Muted: {monitoringState?.IsLocalOutMuted}" +
                          $"\n\tStream Vol: {monitoringState?.StreamVolumeOut}" +
                          $"\n\tStream Muted: {monitoringState?.IsStreamOutMuted}");

        var monitorMixOutputList = await client.GetMonitorMixOutputList().ConfigureAwait(false);
        Console.WriteLine($"Monitor Output: {monitorMixOutputList?.MonitorMix}" +
                          $"\nPossible Outputs:");
        monitorMixOutputList?.MonitorMixList?.ForEach(o => Console.WriteLine($"\t{o.MonitorMix}"));

        var switchState = await client.GetSwitchState().ConfigureAwait(false);
        Console.WriteLine($"Switch State: {switchState}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();

        client.Disconnect();
    }

    private static ConsoleColor ClosestConsoleColour(byte r, byte g, byte b)
    {
        ConsoleColor ret = 0;
        double rr = r, gg = g, bb = b, delta = double.MaxValue;

        foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
        {
            var n = Enum.GetName(typeof(ConsoleColor), cc);
            var c = System.Drawing.Color.FromName(n == "DarkYellow" ? "Orange" : n ?? "white");
            var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
            if (t == 0.0)
            {
                return cc;
            }

            if (t < delta)
            {
                delta = t;
                ret = cc;
            }
        }

        return ret;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace ElgatoWaveSDK.Emulator.Utils;
internal class AudioInterface
{
    private List<CoreAudioDevice> AudioDevices;
    private CoreAudioController controller = new ();

    public AudioInterface()
    {
        Refresh();     
    }

    public CoreAudioDevice? GetDevice(string name)
    {
        return AudioDevices.FirstOrDefault(c => c.Name == name);
    }

    public void Refresh()
    {
        AudioDevices = controller.GetDevices(DeviceState.All).ToList();
    }
}

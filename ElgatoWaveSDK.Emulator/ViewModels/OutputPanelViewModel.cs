using ElgatoWaveSDK.Models;

namespace ElgatoWaveSDK.Emulator.ViewModels;

interface IOutputPanelViewModel
{
    MixType Type { get; set; }

    string OutputName { get; }
}

internal class OutputPanelViewModel : IOutputPanelViewModel
{
    public MixType Type { get; set; }

    public string OutputName => Type == MixType.LocalMix ? "MONITOR MIX" : "STREAM MIX";

    public OutputPanelViewModel()
    {
    }


}

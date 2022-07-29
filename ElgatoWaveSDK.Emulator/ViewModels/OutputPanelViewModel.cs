using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveSDK.Emulator.ViewModels
{
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
}

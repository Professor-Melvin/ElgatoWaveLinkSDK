using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI.System.Windows.Input;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveSDK.Emulator.ViewModels
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<ChannelInfo> Channels { get; set; }

        
    }

    public class MainWindowViewModel : IMainWindowViewModel
    {
        public ObservableCollection<ChannelInfo> Channels { get; set; } = new();

        

        public MainWindowViewModel()
        {
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ElgatoWaveSDK.Emulator.Utils;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveSDK.Emulator.ViewModels
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<IChannelPanelViewModel> Channels { get; }
        ICommand ConnectCommand { get; set; }
    }

    public class MainWindowViewModel : IMainWindowViewModel
    {
        private ElgatoWaveClient Client { get; set; }

        public ICommand ConnectCommand { get; set; }

        public ObservableCollection<IChannelPanelViewModel> Channels { get; } = new();

        public MainWindowViewModel()
        {
            Client = new ElgatoWaveClient();

            ConnectCommand = new CommandHandler(async (obj) =>
            {
                try
                {
                    await Client.ConnectAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow!, "Failed to connect:\n" + ex.Message, "Failed to connect", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                await LoadChannels().ConfigureAwait(false);
            }, () => true);
        }

        private async Task LoadChannels()
        {
            Application.Current.Dispatcher.Invoke(() => Channels.Clear());

            (await Client.GetAllChannelInfo().ConfigureAwait(false))?.ForEach(info =>
            {
                Application.Current.Dispatcher.Invoke(() => Channels.Add(new ChannelPanelViewModel(Client, info)));
            });
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ElgatoWaveSDK.Emulator.Utils;

namespace ElgatoWaveSDK.Emulator.ViewModels;

public interface IMainWindowViewModel
{
    ObservableCollection<IChannelPanelViewModel> Channels { get; }
    //ICommand ConnectCommand { get; set; }
}

public class MainWindowViewModel : IMainWindowViewModel
{
    //public ICommand ConnectCommand { get; set; }

    public ObservableCollection<IChannelPanelViewModel> Channels { get; } = new();

    public MainWindowViewModel()
    {
        //ConnectCommand = new CommandHandler(async (obj) =>
        //{
        //    try
        //    {
        //        await MainWindow.Client.ConnectAsync().ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(Application.Current.MainWindow!, "Failed to connect:\n" + ex.Message, "Failed to connect", MessageBoxButton.OK, MessageBoxImage.Error);
        //        return;
        //    }

        //    await LoadChannels().ConfigureAwait(false);
        //}, () => true);

        Task.Run(async () =>
        {
            await LoadChannels().ConfigureAwait(false);
        });
    }

    private async Task LoadChannels()
    {
        Application.Current.Dispatcher.Invoke(() => Channels.Clear());

        (await MainWindow.Client.GetAllChannelInfo().ConfigureAwait(false))?.ForEach(info =>
        {
            Application.Current.Dispatcher.Invoke(() => Channels.Add(new ChannelPanelViewModel(info)));
        });
    }
}

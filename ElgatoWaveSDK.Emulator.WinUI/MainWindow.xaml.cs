using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using ElgatoWaveSDK.Emulator.Controls;
using ElgatoWaveSDK.Emulator.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ElgatoWaveSDK.Emulator
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class MainWindow : Window
    {
        internal MainWindowViewModel ViewModel { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();

            //this.ExtendsContentIntoTitleBar = true;
            //this.SetTitleBar(new TitleBar());
        }

        private void ConnectButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await App.WaveClient.ConnectAsync().ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        
                    }

                }).ContinueWith(async task =>
                {
                    try
                    {
                        ViewModel.Channels = (await App.WaveClient.GetAllChannelInfo().ConfigureAwait(false))?.Select(info => new ChannelViewModel(info));
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
            catch (Exception ex)
            {

            }
        }
    }
}

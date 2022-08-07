using System.Windows;
using ElgatoWaveSDK.Emulator.ViewModels;

namespace ElgatoWaveSDK.Emulator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        DataContext = new MainWindowViewModel();
    }
}
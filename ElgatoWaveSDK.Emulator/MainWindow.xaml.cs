using System;
using System.Runtime.CompilerServices;
using System.Windows;
using ElgatoWaveSDK.Emulator.ViewModels;

namespace ElgatoWaveSDK.Emulator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    public static ElgatoWaveClient Client { get; set; } = new();

    public MainWindow()
    {
        InitializeComponent();

        try
        {
            Client.ConnectAsync().Wait();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to connect with following error:\n{ex.Message}\n\nExiting...", "Error To Connect",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        DataContext = new MainWindowViewModel();
    }
}
using System.Windows;
using System.Windows.Controls;
using ElgatoWaveSDK.Emulator.ViewModels;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveSDK.Emulator.UserControls;
/// <summary>
/// Interaction logic for OutputPanel.xaml
/// </summary>
public partial class OutputPanel : UserControl
{

    public static readonly DependencyProperty TypeProperty =
        DependencyProperty.Register("Type", typeof(MixType), typeof(OutputPanel), new UIPropertyMetadata(null));
    public MixType Type
    {
        get => (MixType)GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public OutputPanel()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            DataContext = new OutputPanelViewModel(Type);
        };
    }
}

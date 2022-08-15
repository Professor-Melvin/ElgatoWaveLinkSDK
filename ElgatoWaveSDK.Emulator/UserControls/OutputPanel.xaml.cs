using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        this.Loaded += (sender, obj) =>
        {
            DataContext = new OutputPanelViewModel(Type);
        };
    }
}

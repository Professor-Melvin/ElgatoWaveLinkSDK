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

namespace ElgatoWaveSDK.Emulator.UserControls;
/// <summary>
/// Interaction logic for MutableImage.xaml
/// </summary>
public partial class MutableImage : UserControl
{

    public ImageSource MainImageSource { get; set; }

    public ImageSource MutedImageSource { get; set; }

    public MutableImage()
    {
        InitializeComponent();
    }
}

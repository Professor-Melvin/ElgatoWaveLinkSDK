using System.Windows.Controls;
using System.Windows.Media;

namespace ElgatoWaveSDK.Emulator.UserControls;
/// <summary>
/// Interaction logic for MutableImage.xaml
/// </summary>
public partial class MutableImage : UserControl
{

    public ImageSource? MainImageSource
    {
        get; set;
    }

    public ImageSource? MutedImageSource
    {
        get; set;
    }

    public MutableImage()
    {
        InitializeComponent();
    }
}

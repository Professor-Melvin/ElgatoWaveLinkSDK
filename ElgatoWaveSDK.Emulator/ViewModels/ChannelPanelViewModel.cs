using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ElgatoWaveSDK.Emulator.Annotations;
using ElgatoWaveSDK.Emulator.Utils;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveSDK.Emulator.ViewModels;

public interface IChannelPanelViewModel : INotifyPropertyChanged
{
    ICommand MuteLocal { get; set; }

    ICommand MuteStream { get; set; }

    string Name
    {
        get;
    }

    string Colour
    {
        get;
    }

    int LocalVolume
    {
        get; set;
    }

    int StreamVolume
    {
        get; set;
    }

    bool IsLocalMuted
    {
        get; set;
    }

    bool IsStreamMuted
    {
        get; set;
    }
}

public class ChannelPanelViewModel : IChannelPanelViewModel
{
    private ChannelInfo Info { get; set; }
    private ElgatoWaveClient Client{ get; }

    public ICommand MuteLocal { get; set; }
    public ICommand MuteStream { get; set; }

    public ChannelPanelViewModel(ElgatoWaveClient client, ChannelInfo info)
    {
        Info = info;
        Client = client;

        client.InputMixerChanged += (sender, channelInfo) =>
        {
            if (channelInfo.MixId == Info.MixId)
            {
                Info = channelInfo;
                UpdateUi();
            }
        };

        MuteLocal = new CommandHandler((obj) =>
        {
            Info.IsLocalInMuted = !Info.IsLocalInMuted;
            UpdateObject(MixType.LocalMix);
        }, () => true);

        MuteStream = new CommandHandler((obj) =>
        {
            Info.IsStreamInMuted = !Info.IsStreamInMuted;
            UpdateObject(MixType.StreamMix);
        }, () => true);
    }

    private void UpdateObject(MixType slider)
    {
        Task.Run(async () =>
        {
            var newInfo = await Client.SetInputMixer(Info, slider).ConfigureAwait(true);
            if (newInfo != null)
            {
                Info = newInfo;
                Application.Current.Dispatcher.Invoke(UpdateUi);
            }
        });
    }

    private void UpdateUi()
    {
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(Colour));
        OnPropertyChanged(nameof(LocalVolume));
        OnPropertyChanged(nameof(StreamVolume));
        OnPropertyChanged(nameof(IsLocalMuted));
        OnPropertyChanged(nameof(IsStreamMuted));
    }

    public string Name => Info.MixerName ?? $"-ERROR-";

    public string Colour => Info.BgColor ?? "#FFFFFF";

    public int LocalVolume
    {
        get => Info.LocalVolumeIn ?? 0;
        set
        {
            Info.LocalVolumeIn = value;
            UpdateObject(MixType.LocalMix);
        }
    }

    public int StreamVolume
    {
        get => Info.StreamVolumeIn ?? 0;
        set
        {
            Info.StreamVolumeIn = value;
            UpdateObject(MixType.StreamMix);
        }
    }

    public bool IsLocalMuted
    {
        get => Info.IsLocalInMuted ?? false;
        set
        {
            Info.IsLocalInMuted = value;
            UpdateObject(MixType.LocalMix);
        }
    }

    public bool IsStreamMuted
    {
        get => Info.IsStreamInMuted ?? false;
        set
        {
            Info.IsStreamInMuted = value;
            UpdateObject(MixType.StreamMix);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
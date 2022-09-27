using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using ElgatoWaveSDK.Emulator.Annotations;
using ElgatoWaveSDK.Emulator.Utils;
using ElgatoWaveSDK.Models;

namespace ElgatoWaveSDK.Emulator.ViewModels;

internal interface IOutputPanelViewModel : INotifyPropertyChanged
{
    MixType Type
    {
        get;
    }

    string OutputName
    {
        get;
    }

    bool IsSelected
    {
        get; set;
    }

    ICommand Select
    {
        get; set;
    }
}

internal class OutputPanelViewModel : IOutputPanelViewModel
{
    public ICommand Select
    {
        get; set;
    }
    public MixType Type
    {
        get; private set;
    }

    public string OutputName => Type == MixType.LocalMix ? "MONITOR MIX" : "STREAM MIX";

    public bool IsSelected
    {
        get => SelectedType == Type;
        set
        {
            if (value)
            {
                SelectThisType();
            }
        }
    }

    private MixType? SelectedType
    {
        get; set;
    }

    public OutputPanelViewModel(MixType type)
    {
        Type = type;

        MainWindow.Client.MonitorSwitchOutputChanged += (sender, mixType) =>
        {
            SelectedType = mixType;
            UpdateUi();
        };

        Select = new CommandHandler((obj) => SelectThisType(), () => !IsSelected);

        SelectedType = MainWindow.Client.GetSwitchState().Result;
        UpdateUi();
    }

    private void UpdateUi()
    {
        OnPropertyChanged(nameof(IsSelected));
    }

    private void SelectThisType()
    {
        Task.Run(async () =>
        {
            SelectedType = await MainWindow.Client.SetMonitoringState(Type).ConfigureAwait(false);
            UpdateUi();
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

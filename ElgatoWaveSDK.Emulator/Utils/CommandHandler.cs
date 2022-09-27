using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ElgatoWaveSDK.Emulator.Utils;

public class CommandHandler : ICommand
{
    private readonly Func<object?, Task>? _asyncAction;
    private readonly Action<object?>? _action;
    private readonly Func<bool> _canExecute;

    public CommandHandler(Func<object?, Task> action, Func<bool> canExecute)
    {
        _asyncAction = action;
        _canExecute = canExecute;
    }
    public CommandHandler(Action<object?> action, Func<bool> canExecute)
    {
        _action = action;
        _canExecute = canExecute;
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute.Invoke();
    }

    public void Execute(object? parameter)
    {
        _asyncAction?.Invoke(parameter);
        _action?.Invoke(parameter);
    }

    public event EventHandler? CanExecuteChanged
    {
        add
        {
            if (value != null)
            {
                CommandManager.RequerySuggested += value;
            }
        }
        remove
        {
            if (value != null)
            {
                CommandManager.RequerySuggested -= value;
            }
        }
    }
}
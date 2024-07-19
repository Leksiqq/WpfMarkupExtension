using System;
using System.Windows.Input;

namespace Net.Leksi.WpfMarkup;

public class Unsort : ICommand
{
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }
    private readonly DataGridManager _manager;
    public Unsort(DataGridManager manager)
    {
        _manager = manager;
        NotifyInstanceCreated.InstanceCreated?.Invoke(this, NotifyInstanceCreated.s_instanceCreatedArgs);
    }
    public bool CanExecute(object? parameter)
    {
        return _manager.UnsortCanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        try
        {
            _manager.Unsort();
        }
        catch { }
    }
}

using System;
using System.Windows.Input;

namespace Net.Leksi.WpfMarkup;

public class Unsort(DataGridManager manager) : ICommand
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
    public bool CanExecute(object? parameter)
    {
        return manager.UnsortCanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        try
        {
            manager.Unsort();
        }
        catch { }
    }
}

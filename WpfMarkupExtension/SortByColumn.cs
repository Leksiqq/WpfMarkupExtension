using System;
using System.ComponentModel;
using System.Windows.Input;
namespace Net.Leksi.WpfMarkup;
public class SortByColumn : ICommand
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
    public SortByColumn(DataGridManager manager)
    {
        _manager = manager;
        NotifyInstanceCreated.InstanceCreated?.Invoke(this, NotifyInstanceCreated.s_instanceCreatedArgs);
    }
    public bool CanExecute(object? parameter)
    {
        return _manager.SortByColumnCanExecute(parameter);
    }
    public void Execute(object? parameter)
    {
        if (parameter is SortByColumnArgs args && args.FieldName != null)
        {
            ListSortDirection? newDirection;
            if (args.SortDirection is ListSortDirection direction)
            {
                if (direction is ListSortDirection.Ascending)
                {
                    newDirection = ListSortDirection.Descending;
                }
                else
                {
                    newDirection = null;
                }
            }
            else
            {
                newDirection = ListSortDirection.Ascending;
            }
            try
            {
                _manager.SortByColumnExecute(args.FieldName, newDirection);
            }
            catch { }
        }
    }
}

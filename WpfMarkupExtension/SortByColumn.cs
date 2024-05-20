using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Net.Leksi.WpfMarkup;

public class SortByColumn(DataGridManager manager) : ICommand
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
        return manager.SortByColumnCanExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        if (parameter is SortByColumnArgs args && args.FieldName is { })
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
                manager.SortByColumnExecute(args.FieldName, newDirection);
            }
            catch { }
        }
    }
}

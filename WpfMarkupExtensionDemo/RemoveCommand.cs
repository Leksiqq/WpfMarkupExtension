using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace WpfMarkupExtensionDemo;

public class RemoveCommand : ICommand
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

    private readonly ObservableCollection<DataHolder> _datas;

    public RemoveCommand(ObservableCollection<DataHolder> datas)
    {
        _datas = datas;
    }

    public bool CanExecute(object? parameter)
    {
        object?[] parameters = parameter is object[] ? (object[])parameter : new[] { parameter };
        return parameters.Length > 0 && parameters[0] is DataHolder row && !row.IsReadOnly && (parameters.Length == 1 || parameters[1] is bool yes && yes);
    }

    public void Execute(object? parameter)
    {
        if(CanExecute(parameter))
        {
            object?[] parameters = parameter is object[]? (object[])parameter : new[] { parameter };
            _datas.Remove((parameters[0] as DataHolder)!);
        }
    }
}

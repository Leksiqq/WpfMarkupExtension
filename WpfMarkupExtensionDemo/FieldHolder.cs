using System;
using System.ComponentModel;

namespace WpfMarkupExtensionDemo;

public class FieldHolder : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private Type _type = typeof(string);
    private object? _value = null;

    public Type Type
    {
        get => _type;
        set
        {
            if(_type != value)
            {
                _type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
            }
        }
    }
    public object? Value
    {
        get => _value;
        set
        {
            if(_value != value)
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }

}

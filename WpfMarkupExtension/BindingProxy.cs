using System;
using System.ComponentModel;
using System.Windows;

namespace Net.Leksi.WpfMarkup;

public class BindingProxy : Freezable, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private object? _value;

    public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(object),
            typeof(BindingProxy));

    public string? Name { get; set; }

    public object? Value
    {
        get => Type is { } ? Convert.ChangeType(_value, Type) : _value;
        set
        {
            if (_value is INotifyPropertyChanged notify)
            {
                notify.PropertyChanged -= Notify_PropertyChanged;
            }
            _value = value;
            if(_value is INotifyPropertyChanged notify1)
            {
                notify1.PropertyChanged += Notify_PropertyChanged;
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
    }

    public void Touch()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
    }

    private void Notify_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
    }

    public Type? Type { get; set; }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == ValueProperty)
        {
            Value = (object?)e.NewValue;
        }
        base.OnPropertyChanged(e);
    }

    protected override Freezable CreateInstanceCore()
    {
        throw new NotImplementedException();
    }
}

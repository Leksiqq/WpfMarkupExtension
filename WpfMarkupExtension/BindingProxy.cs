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
        get => _value;
        set 
        {
            _value = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }
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

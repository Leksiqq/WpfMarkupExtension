using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Net.Leksi.WpfMarkup;

public class BindingProxy : Freezable
{
    private object? _value;

    public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(object),
            typeof(BindingProxy));

    public object? Value
    {
        get => _value;
        set => _value = value;
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        if (e.Property == ValueProperty)
        {
            Console.WriteLine($"BindingProxy: {e.NewValue}");
            Value = (object?)e.NewValue;
        }
        base.OnPropertyChanged(e);
    }

    protected override Freezable CreateInstanceCore()
    {
        throw new NotImplementedException();
    }
}

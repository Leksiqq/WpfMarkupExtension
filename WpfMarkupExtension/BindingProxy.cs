using System;
using System.Windows;

namespace Net.Leksi.WpfMarkup;

public class BindingProxy : Freezable
{
    public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(object),
            typeof(BindingProxy));

    public object? Value { get; set; }

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

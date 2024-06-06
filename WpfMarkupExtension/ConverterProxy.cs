using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Net.Leksi.WpfMarkup;

public class ConverterProxy: Freezable, IValueConverter, IMultiValueConverter
{
    public static readonly DependencyProperty ConverterProperty = DependencyProperty.Register(
       nameof(Converter), typeof(Object),
       typeof(ConverterProxy)
    );
    public static readonly DependencyProperty MultiConverterProperty = DependencyProperty.Register(
       nameof(MultiConverter), typeof(Object),
       typeof(ConverterProxy)
    );
    public IValueConverter Converter
    {
        get => (IValueConverter)GetValue(ConverterProperty);
        set => SetValue(ConverterProperty, value);
    }
    public IMultiValueConverter MultiConverter
    {
        get => (IMultiValueConverter)GetValue(MultiConverterProperty);
        set => SetValue(MultiConverterProperty, value);
    }

    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Converter?.Convert(value, targetType, parameter, culture);
    }

    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return MultiConverter?.Convert(values, targetType, parameter, culture);
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Converter?.ConvertBack(value, targetType, parameter, culture);
    }

    public object[]? ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return MultiConverter?.ConvertBack(value, targetTypes, parameter, culture);
    }
    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}

using Net.Leksi.WpfMarkup;
using System;
using System.Globalization;
using System.Windows.Media;

namespace WpfMarkupExtensionDemo;

internal class BlueConverter : IUniversalConverter
{
    public object Convert(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        if ("Color".Equals(parameter))
        {
            return new SolidColorBrush(Colors.Blue);
        }
        if ("Type".Equals(parameter))
        {
            return GetType().FullName!;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

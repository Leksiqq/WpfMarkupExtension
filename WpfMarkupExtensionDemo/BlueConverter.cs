using Net.Leksi.WpfMarkup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

namespace WpfMarkupExtensionDemo;

internal class BlueConverter : IUniversalConverter
{
    public object Convert(object value, Type targetType, object? selector, Dictionary<string, object?> parameters, CultureInfo culture)
    {
        if ("Color".Equals(selector))
        {
            return new SolidColorBrush(Colors.Blue);
        }
        if ("Type".Equals(selector))
        {
            return GetType().FullName!;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object? selector, Dictionary<string, object?> parameters, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object ConvertMulti(object[] values, Type targetType, object? selector, Dictionary<string, object?> parameters, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object[] ConvertMultiBack(object value, Type[] targetTypes, object? selector, Dictionary<string, object?> parameters, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

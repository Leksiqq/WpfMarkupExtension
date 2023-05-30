using Net.Leksi.WpfMarkup;
using System;
using System.Globalization;
using System.Windows.Media;

namespace WpfMarkupExtensionDemo;

internal class RedConverter : IUniversalConverter
{
    public object? Convert(object?[]? values, Type targetType, object?[] parameters, CultureInfo? culture, bool multi)
    {
        switch (parameters[0])
        {
            case "Color":
                return new SolidColorBrush(Colors.Red);
            case "Type":
                return GetType().FullName!;
        }
        return null;
    }

    public object? ConvertBack(object? value, Type[] targetTypes, object?[] parameters, CultureInfo? culture, bool multi)
    {
        throw new NotImplementedException();
    }

}

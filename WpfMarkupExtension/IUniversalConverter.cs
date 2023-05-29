using System;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;

namespace Net.Leksi.WpfMarkup;

public interface IUniversalConverter : IValueConverter, IMultiValueConverter
{
    object? Convert(object?[]? values, Type targetType, object?[] parameters, CultureInfo? culture, bool multi);

    object? ConvertBack(object? value, Type[] targetTypes, object?[] parameters, CultureInfo? culture, bool multi);

    object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        object?[] parameters = SplitParameter(parameter);
        object?[]? values = new object?[] { value };
        return Convert(values, targetType, parameters, culture, false);
    }

    object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
    {
        object?[] parameters = SplitParameter(parameter);
        Type[] targetTypes = new Type[] { targetType };
        return ConvertBack(value, targetTypes, parameters, culture, false);
    }

    object? IMultiValueConverter.Convert(object?[] values, Type targetType, object? parameter, CultureInfo? culture)
    {
        object?[] parameters = SplitParameter(parameter);
        return Convert(values, targetType, parameters, culture, true);
    }

    object?[]? IMultiValueConverter.ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo? culture)
    {
        object?[] parameters = SplitParameter(parameter);
        return (object?[]?)ConvertBack(value, targetTypes, parameters, culture, true);
    }

    public static object?[] SplitParameter(object? parameter)
    {
        if (parameter is object?[] arr1)
        {
            return arr1;
        }
        if (parameter is object[] arr2)
        {
            return arr2;
        }
        if (parameter is Array array)
        {
            object?[] res = new object?[array.Length];
            array.CopyTo(res, 0);
            return res;
        }
        if(parameter is string str)
        {
            return str.Split('|').ToArray<object>();
        }
        return new object?[] { parameter };
    }


}

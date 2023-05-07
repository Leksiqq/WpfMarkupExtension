using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Net.Leksi.WpfMarkup;

public interface IUniversalConverter: IValueConverter, IMultiValueConverter
{
    object Convert(object value, Type targetType, object selector, Dictionary<string, object?> parameters, CultureInfo culture);

    object ConvertBack(object value, Type targetType, object selector, Dictionary<string, object?> parameters, CultureInfo culture);

    object ConvertMulti(object[] values, Type targetType, object selector, Dictionary<string, object?> parameters, CultureInfo culture);

    object[] ConvertMultiBack(object value, Type[] targetTypes, object selector, Dictionary<string, object?> parameters, CultureInfo culture);

    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Dictionary<string, object?> parameters = new();
        object selector = TakeParameters(parameter, parameters);
        return Convert(value, targetType, selector, parameters, culture);
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Dictionary<string, object?> parameters = new();
        object selector = TakeParameters(parameter, parameters);
        return ConvertBack(value, targetType, selector, parameters, culture);
    }

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Dictionary<string, object?> parameters = new();
        object selector = TakeParameters(parameter, parameters);
        return ConvertMulti(values, targetType, selector, parameters, culture);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        Dictionary<string, object?> parameters = new();
        object selector = TakeParameters(parameter, parameters);
        return ConvertMultiBack(value, targetTypes, selector, parameters, culture);
    }

    private object TakeParameters(object parameter, Dictionary<string, object?>? parameters)
    {
        if (parameter is null)
        {
            throw new XamlParseException($"{nameof(parameter)} must be set to select an action!");
        }
        object res = null!;
        if (parameter is Array array)
        {
            if (array.Length == 0)
            {
                throw new XamlParseException($"{nameof(parameter)} must be not empty to select an action!");
            }
            for(int i = 0; i < array.Length; ++i)
            {
                object? item = array.GetValue(i);
                string? name;
                object? value;
                if(item is string stringValue)
                {
                    int colonPos = stringValue?.IndexOf(':') ?? -1;
                    if (colonPos >= 0)
                    {
                        name = stringValue!.Substring(0, colonPos);
                        value = stringValue!.Substring(colonPos + 1);
                    }
                    else
                    {
                        name = i.ToString();
                        value = stringValue;
                    }
                }
                else if (item is BindingProxy bpValue)
                {
                    name = bpValue.Name;
                    if (name is null)
                    {
                        name = i.ToString();
                    }
                    value = bpValue.Value;
                }
                else
                {
                    name = i.ToString();
                    value = item?.ToString();
                }
                if (!parameters.TryAdd(name!, value))
                {
                    throw new XamlParseException($"{nameof(parameter)} must have unique names!");
                }
                if (res is null && value is { })
                {
                    res = value;
                }
            }
        }
        else if(parameter is BindingProxy)
        {
            res = TakeParameters(new object[] { parameter }, parameters);
        }
        else
        {
            string[] parts = parameter.ToString()!.Split('|');
            res = TakeParameters(parts, parameters);
        }

        return res!;
    }
}

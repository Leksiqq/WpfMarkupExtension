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
        Dictionary<string, object?> parameters = TakeParameters(parameter);
        object selector = parameters[string.Empty]!;
        return Convert(value, targetType, selector, parameters, culture);
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Dictionary<string, object?> parameters = TakeParameters(parameter);
        object selector = parameters[string.Empty]!;
        return ConvertBack(value, targetType, selector, parameters, culture);
    }

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Dictionary<string, object?> parameters = TakeParameters(parameter);
        object selector = parameters[string.Empty]!;
        return ConvertMulti(values, targetType, selector, parameters, culture);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        Dictionary<string, object?> parameters = TakeParameters(parameter);
        object selector = parameters[string.Empty]!;
        return ConvertMultiBack(value, targetTypes, selector, parameters, culture);
    }

    private Dictionary<string, object?> TakeParameters(object parameter, Dictionary<string, object?>? res = null)
    {
        if (parameter is null)
        {
            throw new XamlParseException($"{nameof(parameter)} must be set to select an action!");
        }

        if(res is null)
        {
            res = new();
        }

        if (parameter is Array array)
        {
            if (array.Length == 0)
            {
                throw new XamlParseException($"{nameof(parameter)} must be not empty to select an action!");
            }
            object? firstValue = null;
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
                        name = string.Empty;
                    }
                    value = bpValue.Value;
                }
                else
                {
                    name = i.ToString();
                    value = item?.ToString();
                }
                if (!res.TryAdd(name!, value))
                {
                    throw new XamlParseException($"{nameof(parameter)} must have unique names!");
                }
                if (firstValue is null && value is { })
                {
                    firstValue = value;
                }
            }
            if (!res.ContainsKey(string.Empty))
            {
                res.Add(string.Empty, firstValue);
            }
        }
        else if(parameter is BindingProxy)
        {
            TakeParameters(new object[] { parameter }, res);
        }
        else
        {
            string[] parts = parameter.ToString()!.Split('|');
            TakeParameters(parts, res);
        }

        if (res[string.Empty] is null)
        {
            throw new XamlParseException($"Selector must be not null to select an action!");
        }

        return res;
    }
}

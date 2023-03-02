using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfMarkupExtensionDemo;

public class DataConverter : MarkupExtension, IValueConverter, IMultiValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        object[] parameters = parameter is string s ? s.Split('|') : new object[] { parameter };
        if(parameters.Length > 1 && ("IsMouseEnter".Equals(parameters[1]) || "IsMouseDown".Equals(parameters[1])))
        {
            return value is { } && value.Equals(parameters[0]);
        }
        else if(parameters.Length > 0 && "Content".Equals(parameters[0]) && value is Button button)
        {
            return $"{Grid.GetRow(button)}.{Grid.GetColumn(button)}";
        }
        Console.WriteLine($"{value}, {parameter}");
        return false;
    }

    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        int selector = 0;
        if (
            ("Background".Equals(parameter) && (selector = 1) == selector)
            || ("BorderThickness".Equals(parameter) && (selector = 2) == selector)
            || ("BorderBrush".Equals(parameter) && (selector = 3) == selector)
            || ("Foreground".Equals(parameter) && (selector = 4) == selector)
        )
        {
            if(values.Length > 1 && values[0] is Grid grid && values[1] is string coords)
            {
                string[] parts = coords.Split('.');
                if(parts.Length > 1)
                {
                    int row = int.Parse(parts[0]);
                    int col = int.Parse(parts[1]);
                    if (grid.Children.Cast<UIElement>().Where(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col).FirstOrDefault() is Button button)
                    {
                        return selector switch { 1 => button.Background, 2 => button.BorderThickness, 3 => button.BorderBrush, 4 => button.Foreground, _ => null};
                    }
                }
            }
        }
        Console.WriteLine($"{string.Join(',', values)}, {parameter}");
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}

using Net.Leksi.WpfMarkup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfMarkupExtensionDemo;

public class DataConverter : MarkupExtension, IValueConverter, IMultiValueConverter
{
    private Type? _fieldType;

    public Type? FieldType
    {
        get => _fieldType;
        set 
        {
            Console.WriteLine(value);
            _fieldType = value; 
        }
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        object[] parameters = parameter is string s ? s.Split('|') : new object[] { parameter };
        if(parameters.Length > 1 && ("IsMouseEnter".Equals(parameters[1]) || "IsMouseDown".Equals(parameters[1])))
        {
            return value is { } && value.Equals(parameters[0]);
        }
        if(parameters.Contains("Content") && value is Button button)
        {
            return $"{Grid.GetRow(button)}.{Grid.GetColumn(button)}";
        }
        if (parameters.Contains("FieldTypeText"))
        {
            string res = (value as Type)!.Name;
            return res;
        }
        if (parameters.Contains("Activities"))
        {
            return Enum.GetNames(typeof(Activities)).ToArray();
        }
        if(parameters
            .Where(p => p is BindingProxy bp)
            .FirstOrDefault() is BindingProxy bp)
        {
            Console.WriteLine($"bp.Value: {bp.Value}");
            //return System.Convert.ChangeType(value, type);
        }
        Console.WriteLine($"Convert: {value}, {targetType}, [{string.Join(',', parameters.Select(p => $"{p.GetType()}:{p}"))}]");
        return false;
    }

    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        object[] parameters = parameter is string s ? s.Split('|') : new object[] { parameter };
        int selector = 0;
        if (
            (parameters.Contains("Background") && (selector = 1) == selector)
            || (parameters.Contains("BorderThickness") && (selector = 2) == selector)
            || (parameters.Contains("BorderBrush") && (selector = 3) == selector)
            || (parameters.Contains("Foreground") && (selector = 4) == selector)
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
        if (parameters.Contains("RemoveCommandCanExecute"))
        {
            return values.ToArray();
        }
        Console.WriteLine($"ConvertMulti: [{string.Join(',', values)}],  [{string.Join(',', parameters)}]");
        return values.FirstOrDefault();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        object[] parameters = parameter is string s ? s.Split('|') : new object[] { parameter };
        if (parameters.Contains("FieldTypeText"))
        {
            switch (value)
            {
                case "String":
                    return typeof(string);
                case "Int32":
                    return typeof(int);
                case "Double":
                    return typeof(double);
                case "Boolean":
                    return typeof(bool);
                case "DateOnly":
                    return typeof(DateOnly);
                case "Activities":
                    return typeof(Activities);
            }
            return typeof(string);
        }
        Console.WriteLine($"ConvertBack: {value}, [{string.Join(',', parameters)}]");
        return value;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        object[] parameters = parameter is string s ? s.Split('|') : new object[] { parameter };
        Console.WriteLine($"ConvertBackMulti: {value}, [{string.Join(',', parameters)}]");
        return new object[] { value };
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}

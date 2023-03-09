using Net.Leksi.WpfMarkup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfMarkupExtensionDemo;

public class DataConverter : IValueConverter, IMultiValueConverter
{
    internal BindingProxy? CurrentEditedItem { get; set; }

    public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }
        List<object> parameters = SplitParameter(parameter);
        if (parameters.Count > 1 && ("IsMouseEnter".Equals(parameters[1]) || "IsMouseDown".Equals(parameters[1])))
        {
            return value is { } && value.Equals(parameters[0]);
        }
        if (parameters.Contains("Content") && value is Button button)
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
        if (parameters.Contains("EditValue"))
        {
            if (CurrentEditedItem is BindingProxy bp && bp.Value is FieldHolder field)
            {
                if (field.Type == typeof(bool))
                {
                    return value switch { false => 0, true => 1, _ => -1 };
                }
                if (field.Type == typeof(Activities))
                {
                    return (int)value;
                }
                if(field.Type == typeof(DateOnly))
                {
                    return ((DateOnly)value).ToDateTime(new TimeOnly(0, 0));
                }
            }
            return value.ToString();
        }
        if (parameters.Contains("ReadValue"))
        {
            if (CurrentEditedItem is BindingProxy bp && bp.Value is FieldHolder field)
            {
            }
            return value.ToString();
        }
        if (targetType == typeof(string))
        {
            return value.ToString();
        }
        return false;
    }

    public object? Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        List<object> parameters = SplitParameter(parameter);
        int selector = 0;
        if (
            (parameters.Contains("Background") && (selector = 1) == selector)
            || (parameters.Contains("BorderThickness") && (selector = 2) == selector)
            || (parameters.Contains("BorderBrush") && (selector = 3) == selector)
            || (parameters.Contains("Foreground") && (selector = 4) == selector)
        )
        {
            if (values.Length > 1 && values[0] is Grid grid && values[1] is string coords)
            {
                string[] parts = coords.Split('.');
                if (parts.Length > 1)
                {
                    int row = int.Parse(parts[0]);
                    int col = int.Parse(parts[1]);
                    if (grid.Children.Cast<UIElement>().Where(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col).FirstOrDefault() is Button button)
                    {
                        return selector switch { 1 => button.Background, 2 => button.BorderThickness, 3 => button.BorderBrush, 4 => button.Foreground, _ => null };
                    }
                }
            }
        }
        if (parameters.Contains("RemoveCommandCanExecute"))
        {
            return values.ToArray();
        }
        return values.FirstOrDefault();
    }

    public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }
        List<object> parameters = SplitParameter(parameter);
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
        if (parameters.Contains("EditValue"))
        {
            if (CurrentEditedItem is BindingProxy bp && bp.Value is FieldHolder field)
            {
                if (field.Type == typeof(bool))
                {
                    return value switch { 0 => false, 1 => true, _ => null };
                }
                if (field.Type == typeof(Activities))
                {
                    if(int.TryParse(value.ToString(), out int pos) && pos >= 0 && pos < Enum.GetValues<Activities>().Length)
                    {
                        return Enum.GetValues<Activities>()[pos];
                    }
                    return null;
                }
                if (field.Type == typeof(Int32))
                {
                    if (Int32.TryParse(value.ToString(), out Int32 result))
                    {
                        return result;
                    }
                    return null;
                }
                if (field.Type == typeof(Double))
                {
                    if (Double.TryParse(value.ToString(), out Double result))
                    {
                        return result;
                    }
                    return null;
                }
                if (field.Type == typeof(DateOnly))
                {
                    if (value is DateTime datetime)
                    {
                        return DateOnly.FromDateTime(datetime);
                    }
                }
            }
            return value.ToString();
        }
        return value;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        List<object> parameters = SplitParameter(parameter);
        return new object[] { value };
    }

    private List<object> SplitParameter(object? parameter)
    {
        List<object> list = new();
        if (parameter is string s)
        {
            list.AddRange(s.Split('|'));
        }
        else if (parameter is { })
        {
            list.Add(parameter);
        }
        return list;
    }
}

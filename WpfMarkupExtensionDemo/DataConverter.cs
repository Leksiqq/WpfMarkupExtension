using Net.Leksi.WpfMarkup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfMarkupExtensionDemo;

public class DataConverter : IUniversalConverter
{
    internal BindingProxy? CurrentEditedItem { get; set; }

    public bool ReverseString { get; set; } = false;

    public object? Convert(object? value, Type targetType, object selector, Dictionary<string,object?> parameters, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }
        if ("IsMouseEnter".Equals(selector) || "IsMouseDown".Equals(selector))
        {
            return value is { } && value.Equals(parameters["1"]);
        }
        if ("Content".Equals(selector) && value is Button button)
        {
            return $"{Grid.GetRow(button)}.{Grid.GetColumn(button)}";
        }
        if ("FieldTypeText".Equals(selector))
        {
            string res = (value as Type)!.Name;
            return res;
        }
        if ("Activities".Equals(selector))
        {
            return Enum.GetNames(typeof(Activities)).ToArray();
        }
        if ("EditValue".Equals(selector))
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
                if (field.Type == typeof(DateOnly))
                {
                    return ((DateOnly)value).ToDateTime(new TimeOnly(0, 0));
                }
            }
            return MayBeReversed(value);
        }
        if ("ReadValue".Equals(selector))
        {
            if (CurrentEditedItem is BindingProxy bp && bp.Value is FieldHolder field)
            {
            }
            return MayBeReversed(value);
        }
        if ("T1Text".Equals(selector))
        {
            Console.WriteLine($"T1Text({nameof(DataConverter)}): {value}, {string.Join('|', parameters.Values)}");
        }
        if (targetType == typeof(string))
        {
            return MayBeReversed(value);
        }
        return value;
    }

    private object? MayBeReversed(object? value)
    {
        return ReverseString ? new String(value.ToString().Reverse().ToArray()) : value.ToString();
    }

    public object? ConvertMulti(object[] values, Type targetType, object selector, Dictionary<string, object?> parameters, CultureInfo culture)
    {
        int choice = 0;
        if (
            ("Background".Equals(selector) && (choice = 1) == choice)
            || ("BorderThickness".Equals(selector) && (choice = 2) == choice)
            || ("BorderBrush".Equals(selector) && (choice = 3) == choice)
            || ("Foreground".Equals(selector) && (choice = 4) == choice)
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
                        return choice switch { 1 => button.Background, 2 => button.BorderThickness, 3 => button.BorderBrush, 4 => button.Foreground, _ => null };
                    }
                }
            }
        }
        if ("RemoveCommandCanExecute".Equals(selector))
        {
            return values.ToArray();
        }
        return values.FirstOrDefault();
    }

    public object? ConvertBack(object? value, Type targetType, object selector, Dictionary<string, object?> parameters, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }
        if ("FieldTypeText".Equals(selector))
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
        if ("EditValue".Equals(selector))
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
            return MayBeReversed(value);
        }
        return value;
    }

    public object[] ConvertMultiBack(object value, Type[] targetTypes, object selector, Dictionary<string, object?> parameters, CultureInfo culture)
    {
        return new object[] { value };
    }

}

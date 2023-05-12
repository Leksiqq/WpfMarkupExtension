using Net.Leksi.WpfMarkup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfMarkupExtensionDemo;

public class DataConverter : IUniversalConverter
{
    internal BindingProxy? CurrentEditedItem { get; set; }

    public bool ReverseString { get; set; } = false;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }
        object?[] parameters = SplitParameter(parameter);
        if (parameters.Length > 1 && ("IsMouseEnter".Equals(parameters[0]) || "IsMouseDown".Equals(parameters[0])))
        {
            return value is { } && value.Equals(parameters[1]);
        }
        if (parameters.Length > 0 && "Content".Equals(parameters[0]) && value is Button button)
        {
            return $"{Grid.GetRow(button)}.{Grid.GetColumn(button)}";
        }
        if (parameters.Length > 0 && "FieldTypeText".Equals(parameters[0]))
        {
            string res = (value as Type)!.Name;
            return res;
        }
        if (parameters.Length > 0 && "Activities".Equals(parameters[0]))
        {
            return Enum.GetNames(typeof(Activities)).ToArray();
        }
        if (parameters.Length > 0 && "EditValue".Equals(parameters[0]))
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
        if (parameters.Length > 0 && "ReadValue".Equals(parameters[0]))
        {
            if (CurrentEditedItem is BindingProxy bp && bp.Value is FieldHolder field)
            {
            }
            return MayBeReversed(value);
        }
        if (parameters.Length > 0 && "T1Text".Equals(parameters[0]))
        {
            Console.WriteLine($"T1Text({nameof(DataConverter)}): {value}, {string.Join('|', parameters)}");
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

    public object? Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        object?[] parameters = SplitParameter(parameter);
        int choice = 0;
        if (
            parameters.Length > 0 && (
            (
                "Background".Equals(parameters[0]) && (choice = 1) == choice)
                || ("BorderThickness".Equals(parameters[0]) && (choice = 2) == choice)
                || ("BorderBrush".Equals(parameters[0]) && (choice = 3) == choice)
                || ("Foreground".Equals(parameters[0]) && (choice = 4) == choice)
            )
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
        if (parameters.Length > 0 && "RemoveCommandCanExecute".Equals(parameters[0]))
        {
            return values.ToArray();
        }
        return values.FirstOrDefault();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return null;
        }
        object?[] parameters = SplitParameter(parameter);
        if (parameters.Length > 0 && "FieldTypeText".Equals(parameters[0]))
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
        if (parameters.Length > 0 && "EditValue".Equals(parameters[0]))
        {
            if (CurrentEditedItem is BindingProxy bp && bp.Value is FieldHolder field)
            {
                if (field.Type == typeof(bool))
                {
                    return value switch { 0 => false, 1 => true, _ => null };
                }
                if (field.Type == typeof(Activities))
                {
                    if (int.TryParse(value.ToString(), out int pos) && pos >= 0 && pos < Enum.GetValues<Activities>().Length)
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

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
    {
        return new object[] { value };
    }

    private object?[] SplitParameter(object? parameter)
    {
        if (parameter is Array array)
        {
            object[] res = new object[array.Length];
            array.CopyTo(res, 0);
            return res;
        }
        if (parameter is string str)
        {
            return str.Split('|');
        }
        return new object?[] { parameter };
    }

}

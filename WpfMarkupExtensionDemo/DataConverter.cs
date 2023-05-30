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

    public object? Convert(object?[] values, Type targetType, object?[] parameters, CultureInfo? culture, bool multi)
    {
        if (values is null || values.Length == 0 || values[0] is null)
        {
            return null;
        }
        if (parameters.Length > 1 && ("IsMouseEnter".Equals(parameters[0]) || "IsMouseDown".Equals(parameters[0])))
        {
            return values[0]!.Equals(parameters[1]);
        }
        if (parameters.Length > 0 && "Content".Equals(parameters[0]) && values[0] is Button button)
        {
            return $"{Grid.GetRow(button)}.{Grid.GetColumn(button)}";
        }
        if (parameters.Length > 0 && "FieldTypeText".Equals(parameters[0]))
        {
            string res = (values[0] as Type)!.Name;
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
                    return values[0] switch { false => 0, true => 1, _ => -1 };
                }
                if (field.Type == typeof(Activities))
                {
                    return (int)values[0]!;
                }
                if (field.Type == typeof(DateOnly))
                {
                    return ((DateOnly)values[0]!).ToDateTime(new TimeOnly(0, 0));
                }
            }
            return MayBeReversed(values[0]);
        }
        if (parameters.Length > 0 && "ReadValue".Equals(parameters[0]))
        {
            if (CurrentEditedItem is BindingProxy bp && bp.Value is FieldHolder field)
            {
            }
            return MayBeReversed(values[0]);
        }
        if (parameters.Length > 0 && "T1Text".Equals(parameters[0]))
        {
            Console.WriteLine($"T1Text({nameof(DataConverter)}): {values[0]}, {string.Join('|', parameters)}");
        }
        if (targetType == typeof(string))
        {
            return MayBeReversed(values[0]);
        }
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
                    if (grid.Children.Cast<UIElement>().Where(e => Grid.GetRow(e) == row && Grid.GetColumn(e) == col).FirstOrDefault() is Button button1)
                    {
                        return choice switch { 1 => button1.Background, 2 => button1.BorderThickness, 3 => button1.BorderBrush, 4 => button1.Foreground, _ => null };
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

    private object? MayBeReversed(object? value)
    {
        return ReverseString ? new String(value.ToString().Reverse().ToArray()) : value.ToString();
    }

    public object? ConvertBack(object? value, Type[] targetTypes, object?[] parameters, CultureInfo? culture, bool multi)
    {
        if (value is null)
        {
            return null;
        }
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

}

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Net.Leksi.WpfMarkup;

public class SortByColumnConverter: Freezable, IValueConverter
{
    public static string SortPosition { get; private set; } = "sortPosition";
    public static string SortDirection { get; private set; } = "sortDirection";

    public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register(
       "FieldName", typeof(string),
       typeof(SortByColumnConverter)
    );
    public static readonly DependencyProperty DataGridManagerProperty = DependencyProperty.Register(
        "DataGridManager", typeof(DataGridManager),
        typeof(SortByColumnConverter)
    );
    public string? FieldName
    {
        get => (string)GetValue(FieldNameProperty);
        set => SetValue(FieldNameProperty, value);
    }
    public DataGridManager? DataGridManager
    {
        get => (DataGridManager)GetValue(DataGridManagerProperty);
        set => SetValue(DataGridManagerProperty, value);
    }
    public virtual object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (parameter.Equals(SortPosition))
        {
            if (DataGridManager?.GetSortDescriptionPosition(FieldName) is int pos)
            {
                return pos;
            }
            return -1;
        }
        if (parameter.Equals(SortDirection))
        {
            return DataGridManager?.GetSortDirection(FieldName);
        }
        throw new Exception($"{nameof(SortByColumnConverter)}.Convert: {value}, {FieldName}, {parameter}");
    }
    public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new Exception($"{nameof(SortByColumnConverter)}.ConvertBack: {value}, {FieldName}, {parameter}");
    }

    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}

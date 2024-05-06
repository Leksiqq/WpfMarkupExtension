using System.ComponentModel;
using System.Windows;

namespace Net.Leksi.WpfMarkup;

public class SortByColumnArgs : Freezable
{
    public static readonly DependencyProperty FieldNameProperty = DependencyProperty.Register(
       "FieldName", typeof(string),
       typeof(SortByColumnArgs)
    );
    public static readonly DependencyProperty ButtonProperty = DependencyProperty.Register(
       "SortDirection", typeof(ListSortDirection?),
       typeof(SortByColumnArgs)
    );
    public string? FieldName
    {
        get => (string)GetValue(FieldNameProperty);
        set => SetValue(FieldNameProperty, value);
    }
    public ListSortDirection? SortDirection
    {
        get => (ListSortDirection?)GetValue(ButtonProperty);
        set => SetValue(ButtonProperty, value);
    }
    protected override Freezable CreateInstanceCore()
    {
        return this;
    }
}

using Net.Leksi.WpfMarkup;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfMarkupExtensionDemo;

public class TableDataTemplateSelector: DataTemplateSelector
{
    public string FieldName { get; set; } = null!;
    public bool? IsEditable { get; set; } = null;

    public DataTemplate ReadEnum { get; set; } = null!;
    public DataTemplate EditEnum { get; set; } = null!;
    public DataTemplate ReadDate { get; set; } = null!;
    public DataTemplate EditDate { get; set; } = null!;
    public DataTemplate ReadValue { get; set; } = null!;
    public DataTemplate EditValue { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        DataHolder? row = item as DataHolder;
        if (row is { } && typeof(DataHolder).GetProperty(FieldName.ToString()!)?.GetValue(row) is FieldHolder field)
        {
            bool isEditable = IsEditable is bool @bool && @bool;
            if (field.Type.IsEnum)
            {
                if (isEditable && EditEnum is { })
                {
                    return EditEnum;
                }
                return ReadEnum;
            }
            if(field.Type == typeof(DateOnly))
            {
                if(isEditable && EditDate is { })
                {
                    return EditDate;
                }
                return ReadDate;
            }
            if (isEditable && EditValue is { })
            {
                return EditValue;
            }
            return ReadValue;
        }
        return base.SelectTemplate(item, container);

    }
}

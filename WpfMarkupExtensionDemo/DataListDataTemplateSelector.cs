using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfMarkupExtensionDemo
{
    internal class DataListDataTemplateSelector: DataTemplateSelector
    {
        public string FieldName { get; set; } = null!;
        public bool IsEditable { get; set; } = false;

        public DataTemplate ReadEnum { get; set; } = null!;
        public DataTemplate EditEnum { get; set; } = null!;
        public DataTemplate ReadDate { get; set; } = null!;
        public DataTemplate EditDate { get; set; } = null!;
        public DataTemplate ReadValue { get; set; } = null!;
        public DataTemplate EditValue { get; set; } = null!;

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataHolder? row = item as DataHolder;
            if(row is { } && typeof(DataHolder).GetProperty(FieldName)?.GetValue(row) is FieldHolder field)
            {
                if(field.Type.IsEnum)
                {
                    if (IsEditable && EditEnum is { })
                    {
                        return EditEnum;
                    }
                    return ReadEnum;
                }
                if(field.Type == typeof(DateOnly))
                {
                    if(IsEditable && EditDate is { })
                    {
                        return EditDate;
                    }
                    return ReadDate;
                }
                if (IsEditable && EditValue is { })
                {
                    return EditValue;
                }
                return ReadValue;
            }
            return base.SelectTemplate(item, container);

        }
    }
}

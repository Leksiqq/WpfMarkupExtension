using Net.Leksi.WpfMarkup;
using System.Windows;
using System.Windows.Controls;

namespace WpfMarkupExtensionDemo;

public class TableDataTemplateSelector: DataTemplateSelector
{
    public string FieldName { get; set; } = null!;
    public BindingProxy? IsEditable { get; set; } = null;

    public DataTemplate ReadValue { get; set; } = null!;
    public DataTemplate EditValue { get; set; } = null!;

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is DataHolder row && typeof(DataHolder).GetProperty(FieldName.ToString()!)?.GetValue(row) is FieldHolder field)
        {
            bool isEditable = IsEditable is BindingProxy proxy && proxy.Value is bool yes && yes && !row.IsReadOnly;
            if (isEditable && EditValue is { })
            {
                if(container is ContentPresenter cp)
                {
                    if(cp.FindResource("CurrentEditedItem") is BindingProxy bp)
                    {
                        bp.Value = field;
                    }
                }
                return EditValue;
            }
            if (ReadValue is { })
            {
                return ReadValue;
            }
        }
        return base.SelectTemplate(item, container);

    }
}

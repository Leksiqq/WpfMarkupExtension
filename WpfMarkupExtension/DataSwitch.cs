using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Net.Leksi.WpfMarkupExtension;

[MarkupExtensionReturnType(typeof(DataTrigger))]
[ContentProperty("Triggers")]
public class DataSwitch : MarkupExtension
{
    public List<DataTrigger> Triggers { get; init; } = new();

    public Binding Binding { get; set; }


    public DataSwitch()
    {
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        DataTrigger result = new();

        return result;
    }

}

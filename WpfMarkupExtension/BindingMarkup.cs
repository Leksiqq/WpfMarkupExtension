using System;
using System.Windows.Markup;

namespace Net.Leksi.WpfMarkup;

public class BindingMarkup : MarkupExtension
{
    public BindingProxy? BindingProxy { get; set; }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return BindingProxy?.Value;
    }
}

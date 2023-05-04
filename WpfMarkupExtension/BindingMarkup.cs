using System;
using System.Windows.Markup;
using System.Xaml;

namespace Net.Leksi.WpfMarkup;

public class BindingMarkup : MarkupExtension
{
    public BindingProxy? BindingProxy { get; set; }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return BindingProxy?.Value is { } ? BindingProxy?.Value : (serviceProvider?.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider)?.RootObject;
    }
}

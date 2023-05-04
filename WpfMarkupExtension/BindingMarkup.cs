using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xaml;

namespace Net.Leksi.WpfMarkup;

public class BindingMarkup : MarkupExtension
{
    public BindingProxy? BindingProxy { get; set; }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        Binding b = BindingOperations.GetBinding(BindingProxy, BindingProxy.ValueProperty);
        //b.Source = (serviceProvider?.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider)?.RootObject;
        Console.WriteLine($">>{b}, {BindingProxy?.Value}, {{{b.Source}}}");
        //return b.ProvideValue(serviceProvider);
        return BindingProxy?.Value is { } ? BindingProxy?.Value : (serviceProvider?.GetService(typeof(IRootObjectProvider)) as IRootObjectProvider)?.RootObject;
    }
}

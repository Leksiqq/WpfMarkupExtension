using System;
using System.Windows.Markup;

namespace Net.Leksi.WpfMarkup;

public class XamlServiceProviderCatcher : MarkupExtension
{
    public IServiceProvider? ServiceProvider { get; set; }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        return this;
    }
}

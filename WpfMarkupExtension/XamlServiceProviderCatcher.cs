using System;
using System.Windows;
using System.Windows.Markup;

namespace Net.Leksi.WpfMarkup;

public class XamlServiceProviderCatcher : MarkupExtension
{
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return new XamlServiceProviderHolder { ServiceProvider = serviceProvider };
    }
}

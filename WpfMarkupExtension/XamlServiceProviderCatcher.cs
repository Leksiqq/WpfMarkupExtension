using System;
using System.Windows.Markup;

namespace Net.Leksi.WpfMarkup;

public class XamlServiceProviderCatcher : MarkupExtension
{
    public XamlServiceProviderCatcher()
    {
        NotifyInstanceCreated.InstanceCreated?.Invoke(this, NotifyInstanceCreated.s_instanceCreatedArgs);
    }
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return serviceProvider;
    }
}

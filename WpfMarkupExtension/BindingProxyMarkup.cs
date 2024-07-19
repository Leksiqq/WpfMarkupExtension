using System;
using System.Windows.Markup;
namespace Net.Leksi.WpfMarkup;
public class BindingProxyMarkup : MarkupExtension
{
    public BindingProxy? BindingProxy { get; set; }
    public BindingProxyMarkup()
    {
        NotifyInstanceCreated.InstanceCreated?.Invoke(this, NotifyInstanceCreated.s_instanceCreatedArgs);
    }
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return BindingProxy?.Value;
    }
}

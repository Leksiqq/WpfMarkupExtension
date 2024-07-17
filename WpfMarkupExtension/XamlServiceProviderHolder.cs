using System;
using System.Windows;

namespace Net.Leksi.WpfMarkup;

public class XamlServiceProviderHolder : DependencyObject
{
    public static readonly DependencyProperty ServiceProviderProperty = DependencyProperty.Register(
        nameof(ServiceProvider), typeof(IServiceProvider), typeof(XamlServiceProviderHolder)
    );
    public IServiceProvider ServiceProvider
    {
        get => (IServiceProvider)GetValue(ServiceProviderProperty);
        set => SetValue(ServiceProviderProperty, value);
    }
}

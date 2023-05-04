using System;
using System.Windows.Markup;
using System.Xaml;

namespace Net.Leksi.WpfMarkup;

public class RootObject : MarkupExtension
{
    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        return ((IRootObjectProvider?)serviceProvider.GetService(typeof(IRootObjectProvider)))?.RootObject;
    }
}

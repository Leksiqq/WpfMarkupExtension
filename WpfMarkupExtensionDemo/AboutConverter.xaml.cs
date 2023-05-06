using System.Windows;
using System.Windows.Data;

namespace WpfMarkupExtensionDemo;

public partial class AboutConverter : Window
{
    public IValueConverter Converter { get; init; }

    public AboutConverter(IValueConverter converter)
    {
        Converter = converter;
        InitializeComponent();
    }
}

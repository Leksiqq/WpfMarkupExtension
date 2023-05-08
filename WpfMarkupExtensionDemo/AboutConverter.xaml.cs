using Net.Leksi.WpfMarkup;
using System.Windows;
using System.Windows.Data;

namespace WpfMarkupExtensionDemo;

public partial class AboutConverter : Window
{
    public IUniversalConverter Converter { get; init; }

    public AboutConverter(IUniversalConverter converter)
    {
        Converter = converter;
        InitializeComponent();
    }
}

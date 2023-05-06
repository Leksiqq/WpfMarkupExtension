using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace WpfMarkupExtensionDemo;

internal class BlueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if ("Color".Equals(parameter.ToString()))
        {
            return new SolidColorBrush(Colors.Blue);
        }
        if ("Type".Equals(parameter.ToString()))
        {
            return GetType().FullName;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

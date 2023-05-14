using System;
using System.Windows.Data;

namespace Net.Leksi.WpfMarkup;

public interface IUniversalConverter : IValueConverter, IMultiValueConverter
{
    public static object?[] SplitParameter(object? parameter)
    {
        if(parameter is Array array)
        {
            object[] res = new object[array.Length];
            array.CopyTo(res, 0);
            return res;
        }
        if(parameter is string str)
        {
            return str.Split('|');
        }
        return new object?[] { parameter };
    }


}

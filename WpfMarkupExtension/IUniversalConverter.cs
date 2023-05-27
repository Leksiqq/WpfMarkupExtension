using System;
using System.Linq;
using System.Windows.Data;

namespace Net.Leksi.WpfMarkup;

public interface IUniversalConverter : IValueConverter, IMultiValueConverter
{
    public static object?[] SplitParameter(object? parameter)
    {
        if (parameter is object?[] arr1)
        {
            return arr1;
        }
        if (parameter is object[] arr2)
        {
            return arr2;
        }
        if (parameter is Array array)
        {
            object?[] res = new object?[array.Length];
            array.CopyTo(res, 0);
            return res;
        }
        if(parameter is string str)
        {
            return str.Split('|').ToArray<object>();
        }
        return new object?[] { parameter };
    }


}

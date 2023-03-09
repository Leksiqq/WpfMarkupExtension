using System;
using System.ComponentModel;

namespace WpfMarkupExtensionDemo;

public class FieldHolder : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private Type _type = typeof(string);
    private object? _value = null;

    public Type Type
    {
        get => _type;
        set
        {
            if(_type != value)
            {
                _value = ConvertValue(_value, value);
                _type = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Type)));
            }
        }
    }

    public object? Value
    {
        get => _value;
        set
        {
            if(_value != value)
            {
                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }
    }

    private object? ConvertValue(object? value, Type to)
    {
        if (value == null || _type == to)
        {
            return null;
        }
        if(to == typeof(Activities))
        {
            if (_type == typeof(Int32))
            {
                return Enum.GetValues<Activities>()[(Int32)value];
            }
            if (_type == typeof(Double))
            {
                return Enum.GetValues<Activities>()[(int)Math.Round((double)value)];
            }
            if (_type == typeof(string))
            {
                return Enum.Parse<Activities>((string)value);
            }
            if (_type == typeof(bool))
            {
                return Enum.GetValues<Activities>()[(bool)value ? 1 : 0];
            }
        }
        else if(to == typeof(Int32)) 
        {
            if (_type == typeof(Double))
            {
                return (Int32)Math.Round((double)value);
            }
            if(_type == typeof(DateOnly))
            {
                return ((DateOnly)value).DayNumber;
            }
        }
        else if(to == typeof(DateOnly))
        {
            if(_type == typeof(string))
            {
                if(DateTime.TryParse(value.ToString(), out DateTime dateTime))
                {
                    return DateOnly.TryParse(dateTime.ToString("yyyy-MM-dd"), out DateOnly date) ? date : null;
                }
                return null;
            }
            if (_type == typeof(Int32))
            {
                return DateOnly.FromDayNumber((Int32)value);
            }
            if (_type == typeof(Double))
            {
                return DateOnly.FromDayNumber((Int32)Math.Round((double)value));
            }
        }
        else if(to == typeof(string))
        {
            if(_type == typeof(DateOnly))
            {
                return ((DateOnly)value).ToString();
            }
        }
        try
        {
            return Convert.ChangeType(value, to);
        }
        catch
        {
            return null;
        }
    }

}

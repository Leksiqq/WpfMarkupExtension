using System;
using System.ComponentModel;

namespace WpfMarkupExtensionDemo;

public class DataHolder: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _isReadOnly = false;
    private FieldHolder _item1 = new();
    private FieldHolder _item2 = new();
    private FieldHolder _item3 = new();
    private FieldHolder _item4 = new();
    private FieldHolder _item5 = new();

    public bool IsReadOnly
    {
        get => _isReadOnly;
        set
        {
            if(_isReadOnly != value)
            {
                _isReadOnly = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsReadOnly)));
            }
        }
    }
    public FieldHolder Item1 => _item1;
    public FieldHolder Item2 => _item2;
    public FieldHolder Item3 => _item3;
    public FieldHolder Item4 => _item4;
    public FieldHolder Item5 => _item5;

    public DataHolder()
    {
        _item1.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item1)));
        _item2.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item2)));
        _item3.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item3)));
        _item4.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item4)));
        _item5.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Item5)));
    }
}

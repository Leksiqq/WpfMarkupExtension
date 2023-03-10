using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Net.Leksi.WpfMarkup;

[MarkupExtensionReturnType(typeof(DataTrigger))]
[ContentProperty("Cases")]

public class DataSwitch: DataTrigger
{
    public ObservableCollection<DataTrigger> Cases { get; init; } = new();

    public DataSwitch()
    {
        Cases.CollectionChanged += Cases_CollectionChanged;
    }

    private void Cases_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is { })
        {
            foreach (DataTrigger item in e.NewItems)
            {
                item.Binding = this.Binding;
            }
        }
    }

}

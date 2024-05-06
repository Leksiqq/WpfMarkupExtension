using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace Net.Leksi.WpfMarkup;

public class DataGridManager: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public CollectionViewSource ViewSource { get; private init; } = new();
    public SortByColumn SortByColumnCommand { get; private init; }
    public Unsort UnsortCommand { get; private init; }
#pragma warning disable CA1822 // Пометьте члены как статические
    public int Notification => 0;
#pragma warning restore CA1822 // Пометьте члены как статические
    public DataGridManager()
    {
        SortByColumnCommand = new SortByColumn(this);
        UnsortCommand = new Unsort(this);
    }
    public void SortByColumnExecute(string fieldName, ListSortDirection? direction)
    {
        if (direction is ListSortDirection dir)
        {
            if (
                Enumerable.Range(0, ViewSource.SortDescriptions.Count)
                    .Where(i => ViewSource.SortDescriptions[i].PropertyName == fieldName)
                    .FirstOrDefault(-1) is int pos && pos >= 0
            )
            {
                ViewSource.SortDescriptions[pos] = new SortDescription
                {
                    PropertyName = fieldName,
                    Direction = dir,
                };
            }
            else
            {
                ViewSource.SortDescriptions.Add(new SortDescription
                {
                    PropertyName = fieldName,
                    Direction = dir,
                });
            }
        }
        else if (ViewSource.SortDescriptions.Where(d => d.PropertyName == fieldName).FirstOrDefault() is SortDescription sd)
        {
            ViewSource.SortDescriptions.Remove(sd);
        }
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notification)));

    }
    public void Unsort()
    {
        ViewSource.SortDescriptions.Clear();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notification)));
    }

    public int GetSortDescriptionPosition(string? fieldName)
    {
        if (
            fieldName is { }
            && Enumerable.Range(0, ViewSource.SortDescriptions.Count)
                .Where(i => ViewSource.SortDescriptions[i].PropertyName == fieldName)
                .FirstOrDefault(-1) is int pos
            && pos >= 0
        )
        {
            return pos;
        }
        return -1;
    }
    public ListSortDirection? GetSortDirection(string? fieldName)
    {
        if (
            fieldName is { }
            && ViewSource.SortDescriptions
                .Where(sd => sd.PropertyName == fieldName)
                .FirstOrDefault() is SortDescription sd 
            && sd.PropertyName == fieldName
        )
        {
            return sd.Direction;
        }
        return null;
    }

}

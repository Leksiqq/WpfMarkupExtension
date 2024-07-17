using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace Net.Leksi.WpfMarkup;

public class DataGridManager: INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    private DataGrid? _dataGrid = null;
    private bool _isInEditMode = false;
    public CollectionViewSource ViewSource { get; private init; } = new();
    public SortByColumn SortByColumnCommand { get; private init; }
    public Unsort UnsortCommand { get; private init; }
#pragma warning disable CA1822 // Пометьте члены как статические
    public int Notification => 0;
#pragma warning restore CA1822 // Пометьте члены как статические
    public bool AutoCommit { get; set; }
    public DataGrid? DataGrid { 
        get => _dataGrid; 
        set
        {
            if(_dataGrid != value)
            {
                if(_dataGrid != null)
                {
                    _dataGrid.CellEditEnding -= _dataGrid_CellEditEnding;
                    _dataGrid.BeginningEdit -= _dataGrid_BeginningEdit;
                }
                _dataGrid = value;
                if(_dataGrid != null)
                {
                    _dataGrid.CellEditEnding += _dataGrid_CellEditEnding;
                    _dataGrid.BeginningEdit += _dataGrid_BeginningEdit;
                }
            }
        }
    }
    public DataGridManager()
    {
        SortByColumnCommand = new SortByColumn(this);
        UnsortCommand = new Unsort(this);
    }
    public void SortByColumnExecute(string fieldName, ListSortDirection? direction)
    {
        if(_isInEditMode && AutoCommit)
        {
            DataGrid!.CommitEdit();
        }
        if (!_isInEditMode)
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
    }
    public void Unsort()
    {
        if (_isInEditMode && AutoCommit)
        {
            DataGrid!.CommitEdit();
        }
        if (!_isInEditMode)
        {
            ViewSource.SortDescriptions.Clear();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Notification)));
        }
    }
        public int GetSortDescriptionPosition(string? fieldName)
    {
        if (
            fieldName != null
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
            fieldName != null
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
    internal bool SortByColumnCanExecute(object? parameter)
    {
        return parameter is SortByColumnArgs args && args.FieldName != null && (!_isInEditMode || AutoCommit);
    }

    internal bool UnsortCanExecute(object? parameter)
    {
        return !_isInEditMode || AutoCommit;
    }
    private void _dataGrid_BeginningEdit(object? sender, DataGridBeginningEditEventArgs e)
    {
        _isInEditMode = true;
    }
    private void _dataGrid_CellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
    {
        if(ViewSource.View is ListCollectionView lcw)
        {
            switch (e.EditAction)
            {
                case DataGridEditAction.Commit:
                    if (lcw.CurrentAddItem != null)
                    {
                        lcw.CommitNew();
                    }
                    else
                    {
                        lcw.CommitEdit();
                    }
                    break;
                case DataGridEditAction.Cancel:
                    if (lcw.CurrentAddItem != null)
                    {
                        lcw.CancelNew();
                    }
                    else
                    {
                        lcw.CancelEdit();
                    }
                    break;
            }
        }
        _isInEditMode = false;
    }
}

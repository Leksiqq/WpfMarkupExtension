using MaterialDesignColors.Recommended;
using Net.Leksi.WpfMarkup;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfMarkupExtensionDemo
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool Col_1_On => (Cb1.IsChecked ?? false) && (Cb17.IsChecked ?? false);
        public bool Col_2_On => (Cb2.IsChecked ?? false) && (Cb18.IsChecked ?? false);
        public bool Col_3_On => (Cb3.IsChecked ?? false) && (Cb19.IsChecked ?? false);
        public bool Col_4_On => (Cb4.IsChecked ?? false) && (Cb20.IsChecked ?? false);
        public bool Col_5_On => (Cb5.IsChecked ?? false) && (Cb21.IsChecked ?? false);
        public bool Col_6_On => (Cb6.IsChecked ?? false) && (Cb22.IsChecked ?? false);

        public bool Row_1_On => (Cb8.IsChecked ?? false) && (Cb9.IsChecked ?? false);
        public bool Row_2_On => (Cb10.IsChecked ?? false) && (Cb11.IsChecked ?? false);
        public bool Row_3_On => (Cb12.IsChecked ?? false) && (Cb13.IsChecked ?? false);
        public bool Row_4_On => (Cb14.IsChecked ?? false) && (Cb15.IsChecked ?? false);

        public bool LH_1_On => (Cb2.IsChecked ?? false) && (Cb10.IsChecked ?? false);
        public bool LH_2_On => (Cb3.IsChecked ?? false) && (Cb12.IsChecked ?? false);
        public bool LH_3_On => (Cb4.IsChecked ?? false) && (Cb14.IsChecked ?? false);
        public bool LH_4_On => (Cb5.IsChecked ?? false) && (Cb16.IsChecked ?? false);
        public bool LH_5_On => (Cb6.IsChecked ?? false) && (Cb17.IsChecked ?? false);
        public bool LH_6_On => (Cb7.IsChecked ?? false) && (Cb18.IsChecked ?? false);
        public bool LH_7_On => (Cb9.IsChecked ?? false) && (Cb19.IsChecked ?? false);
        public bool LH_8_On => (Cb11.IsChecked ?? false) && (Cb20.IsChecked ?? false);
        public bool LH_9_On => (Cb13.IsChecked ?? false) && (Cb21.IsChecked ?? false);

        public bool HL_1_On => (Cb12.IsChecked ?? false) && (Cb18.IsChecked ?? false);
        public bool HL_2_On => (Cb10.IsChecked ?? false) && (Cb19.IsChecked ?? false);
        public bool HL_3_On => (Cb8.IsChecked ?? false) && (Cb20.IsChecked ?? false);
        public bool HL_4_On => (Cb0.IsChecked ?? false) && (Cb21.IsChecked ?? false);
        public bool HL_5_On => (Cb1.IsChecked ?? false) && (Cb22.IsChecked ?? false);
        public bool HL_6_On => (Cb2.IsChecked ?? false) && (Cb23.IsChecked ?? false);
        public bool HL_7_On => (Cb3.IsChecked ?? false) && (Cb15.IsChecked ?? false);
        public bool HL_8_On => (Cb4.IsChecked ?? false) && (Cb13.IsChecked ?? false);
        public bool HL_9_On => (Cb5.IsChecked ?? false) && (Cb11.IsChecked ?? false);

        public string CurrentMouseEnter { get; private set; } = string.Empty;
        public string CurrentMouseDown { get; private set; } = string.Empty;

        public RemoveCommand RemoveCommand { get; init; }
        public AddCommand AddCommand { get; init; }

        private bool _isDatasEditable = false;
        public bool IsDatasEditable 
        {
            get => _isDatasEditable;
            set
            {
                if(value != _isDatasEditable)
                {
                    _isDatasEditable = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDatasEditable)));
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ObservableCollection<DataHolder> Datas { get; init; } = new();
        public CollectionViewSource DatasViewSource { get; init; } = new();

        public DataConverter DataConverter { get; init; } = new();

        public bool ReverseString
        {
            get => DataConverter.ReverseString;
            set
            {
                DataConverter.ReverseString = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ReverseString)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DataConverter)));
            }
        }

        public MainWindow()
        {
            DatasViewSource.Source = Datas;
            RemoveCommand = new RemoveCommand(Datas);
            AddCommand = new AddCommand(Datas);
            InitializeComponent();
            (FindResource("DataConverter") as DataConverter)!.CurrentEditedItem = FindResource("CurrentEditedItem") as BindingProxy;
        }

        private void CheckBoxClick(object sender, RoutedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        private void ButtonMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                CurrentMouseEnter = $"{Grid.GetRow(button)}.{Grid.GetColumn(button)}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMouseEnter)));
            }
        }

        private void ButtonMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                CurrentMouseEnter = string.Empty;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMouseEnter)));
            }
        }

        private void ButtonMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                CurrentMouseDown = $"{Grid.GetRow(button)}.{Grid.GetColumn(button)}";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMouseDown)));
            }
        }

        private void ButtonMouseUp(object sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                CurrentMouseDown = string.Empty;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentMouseDown)));
            }
        }

        private void AboutConverter_Click(object sender, RoutedEventArgs e)
        {
            if(sender is Button button) 
            {
                switch(button.Name) 
                {
                    case "Red":
                        new AboutConverter(new RedConverter()).ShowDialog();
                        break;
                    case "Blue":
                        new AboutConverter(new BlueConverter()).ShowDialog();
                        break;
                }
            }
        }
    }
}

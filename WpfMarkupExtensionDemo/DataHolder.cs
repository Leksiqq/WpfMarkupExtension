namespace WpfMarkupExtensionDemo;

public class DataHolder
{
    public bool IsReadOnly { get; set; } = false;
    public FieldHolder Item1 { get; init; } = new();
    public FieldHolder Item2 { get; init; } = new();
    public FieldHolder Item3 { get; init; } = new();
    public FieldHolder Item4 { get; init; } = new();
    public FieldHolder Item5 { get; init; } = new();
}

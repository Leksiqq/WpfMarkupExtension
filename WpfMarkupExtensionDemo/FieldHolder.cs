using System;

namespace WpfMarkupExtensionDemo;

public class FieldHolder
{
    public Type Type { get; set; } = typeof(string);
    public object? Value { get; set; }
}

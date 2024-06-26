**Attention!** _This article, as well as this announcement, are automatically translated from Russian_.

The **Net.Leksi.WpfMarkupExtension** library is designed to extend WPF markup. It contains several classes that you may find useful when developing XAML. All classes are contained in the `Net.Leksi.WpfMarkup` namespace.

* `StyleCombiner` - allows you to apply multiple styles to an element without inheritance.
* `ParameterizedResource` - analogue of `StaticResourceExtension`, which allows using resources with parameters that can be replaced with different values in the markup.
* `XamlServiceProviderCatcher` - allows using `ParameterizedResource` in code.
* `BindingProxy` is a universal resource that can serve as a link to any object or act as a binding.
* `BindingProxyMarkup` - used when you need to place a binding value where a markup extension is required.
* `IUniversalConverter` - combines the `System.Windows.Data.IValueConverter` and `System.Windows.Data.IMultiValueConverter` interfaces for convenience.
* `DataSwitch` - used instead of a large number of `DataTrigger` that have the same binding but different trigger values. Reduces both the XAML text and the number of calls to the binding source.
* `BoolExpressionConverter` - incomplete implementation of the `IMultiValueConverter` interface, which implements the `object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)` method, which is passed an array of `bool` values `values ` and a string describing the Boolean expression above them, as `parameter`. Returns the result of evaluating an expression.
* `ConverterProxy` - an adapter that is a `MarkupExtension` for converters `IValueConverter` or `IMultiValueConverter`, which are not `MarkupExtension` and cannot become one, since they are already inherited from another type, but are required there, where `MarkupExtension` is expected.

More info: [https://github.com/Leksiqq/WpfMarkupExtension/wiki](https://github.com/Leksiqq/WpfMarkupExtension/wiki)
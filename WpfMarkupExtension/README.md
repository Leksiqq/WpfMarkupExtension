**Attention!** _This article, as well as this announcement, are automatically translated from Russian_.

The **Net.Leksi.WpfMarkupExtension** library is designed to extend WPF markup. It contains several classes that you may find useful when developing XAML. All classes are contained in the `Net.Leksi.WpfMarkup` namespace.

* `StyleCombiner` - allows you to apply multiple styles to an element without inheritance.
* `ParameterizedResource` - analogue of `StaticResourceExtension`, which allows using resources with parameters that can be replaced with different values in the markup.
* `BindingProxy` - an universal resource that can serve as a link to any object or act as a binding.
* `DataSwitch` - used instead of a large number of `DataTrigger` that have the same binding but different trigger values. Reduces both the XAML text and the number of calls to the binding source.

More info: [https://github.com/Leksiqq/WpfMarkupExtension/wiki](https://github.com/Leksiqq/WpfMarkupExtension/wiki)
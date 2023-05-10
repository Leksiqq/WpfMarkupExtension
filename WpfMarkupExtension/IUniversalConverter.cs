using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup.Primitives;
using System.Windows.Media;

namespace Net.Leksi.WpfMarkup;

public interface IUniversalConverter : IValueConverter, IMultiValueConverter
{
    private readonly static List<string> _route = new();
    private readonly static HashSet<object> _seenObjects = new();

    internal static bool IsConnecting { get; private set; } = false;
    internal static string Path => "/" + string.Join('/', _route.Select(p => Regex.Match(p, @"^[^{]+\{([^}]+)\}$")).Where(m => m.Success).Select(m => m.Groups[1].Captures[0].Value));

    object Convert(object value, Type targetType, UniversalConverterParameter? parameter, CultureInfo culture);

    object ConvertBack(object value, Type targetType, UniversalConverterParameter? parameter, CultureInfo culture);

    object ConvertMulti(object[] values, Type targetType, UniversalConverterParameter? parameter, CultureInfo culture);

    object[] ConvertMultiBack(object value, Type[] targetTypes, UniversalConverterParameter? parameter, CultureInfo culture);

    public void Connect(DependencyObject value)
    {
        try
        {
            IsConnecting = true;
            _route.Clear();
            _seenObjects.Clear();
            WalkMarkup(MarkupWriter.GetMarkupObjectFor(value));
        }
        finally
        {
            IsConnecting = false;
            _route.Clear();
            _seenObjects.Clear();
        }
    }

    private void WalkMarkup(MarkupObject mo)
    {
        if (_seenObjects.Add(mo.Instance))
        {
            if (mo.Instance is DependencyObject dependencyObject)
            {
                foreach (
                    PropertyDescriptor pd in TypeDescriptor.GetProperties(
                        dependencyObject, new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) }
                    )
                )
                {
                    bool isBinding = false;

                    try
                    {
                        if (pd.GetValue(dependencyObject) is object value)
                        {
                            if (pd.PropertyType == typeof(BindingBase))
                            {
                                isBinding = true;
                                if (value is Binding binding)
                                {
                                    OnBinding(binding);
                                }
                                else if (value is MultiBinding multiBinding)
                                {
                                    OnBinding(multiBinding);
                                }
                            }
                        }
                    }
                    catch (Exception) { }

                    if (!isBinding)
                    {
                        DependencyPropertyDescriptor dpd =
                            DependencyPropertyDescriptor.FromProperty(pd);

                        if (dpd is { })
                        {
                            if (BindingOperations.GetBinding(dependencyObject, dpd.DependencyProperty) is Binding binding1)
                            {
                                _route.Add(pd.Name);
                                OnBinding(binding1);
                                _route.RemoveAt(_route.Count - 1);
                            }
                            else if (BindingOperations.GetMultiBinding(dependencyObject, dpd.DependencyProperty) is MultiBinding multiBinding)
                            {
                                _route.Add(pd.Name);
                                OnBinding(multiBinding);
                                _route.RemoveAt(_route.Count - 1);
                            }
                        }
                    }
                }
                if (dependencyObject is Visual visual)
                {
                    int childrenCount = VisualTreeHelper.GetChildrenCount(visual);
                    if (childrenCount > 0)
                    {
                        for (int i = 0; i < childrenCount; ++i)
                        {
                            DependencyObject child = VisualTreeHelper.GetChild(visual, i);
                            _route.Add(child.GetType().ToString());
                            WalkMarkup(MarkupWriter.GetMarkupObjectFor(child));
                            _route.RemoveAt(_route.Count - 1);
                        }
                    }
                }
            }
            foreach (var prop in mo.Properties)
            {
                if (prop.Value is BindingBase)
                {
                    OnBinding((BindingBase)prop.Value);
                }
                else
                {
                    _route.Add(prop.Name);
                    if (prop.DependencyProperty is { })
                    {
                        if (BindingOperations.GetBinding((DependencyObject)mo.Instance, prop.DependencyProperty) is Binding binding)
                        {
                            OnBinding(binding);
                        }
                        else if (BindingOperations.GetMultiBinding((DependencyObject)mo.Instance, prop.DependencyProperty) is MultiBinding multiBinding)
                        {
                            OnBinding(multiBinding);
                        }
                    }
                    try
                    {
                        if (prop.IsComposite)
                        {
                            _route[_route.Count - 1] += "[]";
                            foreach (var item in prop.Items)
                            {
                                WalkMarkup(item);
                            }
                        }
                    }
                    catch (NullReferenceException) { }
                    _route.RemoveAt(_route.Count - 1);
                }
            }

        }
    }

    private void OnBinding(BindingBase bindingBase)
    {
        if (bindingBase is Binding binding)
        {
            if(!_route[_route.Count - 1].Contains("{"))
            {
                _route[_route.Count - 1] += $"{{{binding.Path.Path}}}";
            }
            Console.WriteLine($"path: {Path}");
        }
        else if(bindingBase is MultiBinding multiBinding)
        {
            foreach (Binding bindingItem in multiBinding.Bindings)
            {
                OnBinding(bindingItem);
            }
        }
    }

    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        UniversalConverterParameter? ucp;
        if (parameter is UniversalConverterParameter ucp1)
        {
            ucp = ucp1;
        }
        else
        {
            ucp = parameter is { } ? new UniversalConverterParameter { Parameter = parameter } : null;
        }
        return Convert(value, targetType, ucp, culture);
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        UniversalConverterParameter? ucp;
        if (parameter is UniversalConverterParameter ucp1)
        {
            ucp = ucp1;
        }
        else
        {
            ucp = parameter is { } ? new UniversalConverterParameter { Parameter = parameter } : null;
        }
        return ConvertBack(value, targetType, ucp, culture);
    }

    object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        UniversalConverterParameter? ucp;
        if (parameter is UniversalConverterParameter ucp1)
        {
            ucp = ucp1;
        }
        else
        {
            ucp = parameter is { } ? new UniversalConverterParameter { Parameter = parameter } : null;
        }
        return ConvertMulti(values, targetType, ucp, culture);
    }

    object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        UniversalConverterParameter? ucp;
        if (parameter is UniversalConverterParameter ucp1)
        {
            ucp = ucp1;
        }
        else
        {
            ucp = parameter is { } ? new UniversalConverterParameter { Parameter = parameter } : null;
        }
        return ConvertMultiBack(value, targetTypes, ucp, culture);
    }
}

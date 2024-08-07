﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Markup.Primitives;
using System.Windows.Media;
using System.Xaml;
using XamlParseException = System.Windows.Markup.XamlParseException;

namespace Net.Leksi.WpfMarkup;

[MarkupExtensionReturnType(typeof(object))]
[ContentProperty("Replaces")]
public class ParameterizedResourceExtension : MarkupExtension
{
    private const string s_indentionStep = "  ";
    private readonly Dictionary<string, object?> _replacements = new();
    private readonly Dictionary<string, object?> _defaults = new();
    private object? _replaces;
    private object? _defaultsString;
    private static readonly Stack<ParameterizedResourceExtension> s_callStacks = new();
    private string _indention = string.Empty;
    private string _prompt = string.Empty;
    private HashSet<object>? _seenObjects;
    private IServiceProvider _services = null!;
    private ParameterizedResourceExtension? _root = null;
    public static bool DefaultSrict { get; set; } = false;

    [Ambient]
    public object? Replaces
    {
        get => _replaces;
        set
        {
            if (!object.Equals(_replaces, value))
            {
                _replaces = value;
                _replacements.Clear();
                if (_replaces is string str)
                {
                    foreach (
                        string[] ent in str.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(entry => entry.Split(':', StringSplitOptions.TrimEntries))
                            .Where(entry => entry.Length == 2)
                    )
                    {
                        _replacements.TryAdd(ent[0], ent[1]);
                    }
                }
                else if (_replaces is object[] arr)
                {
                    foreach (object o in arr)
                    {
                        if(o is BindingProxy proxy)
                        {
                            if(proxy.Name != null)
                            {
                                _replacements.TryAdd(proxy.Name, proxy.Value);
                            }
                        }
                        else
                        {
                            string[]? ent = o?.ToString()!.Split(':', 2, StringSplitOptions.TrimEntries);
                            if (ent != null && ent.Length == 2)
                            {
                                _replacements.TryAdd(ent[0], ent[1]);
                            }
                        }
                    }
                }
            }
        }
    }

   [Ambient]
    public object? Defaults
    {
        get => _defaultsString;
        set
        {
            if (!object.Equals(_defaultsString, value))
            {
                _defaultsString = value;
                _defaults.Clear();
                if (_defaultsString is string str)
                {
                    foreach (
                        string[] ent in str.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Select(entry => entry.Split(':', StringSplitOptions.TrimEntries))
                            .Where(entry => entry.Length == 2)
                    )
                    {
                        _defaults.TryAdd(ent[0], ent[1]);
                    }
                }
                else if (_defaultsString is object[] arr)
                {
                    foreach (object o in arr)
                    {
                        if (o is BindingProxy proxy)
                        {
                            if (proxy.Name != null)
                            {
                                _replacements.TryAdd(proxy.Name, proxy.Value);
                            }
                        }
                        else
                        {
                            string[]? ent = o?.ToString()!.Split(':', 2, StringSplitOptions.TrimEntries);
                            if (ent != null && ent.Length == 2)
                            {
                                _replacements.TryAdd(ent[0], ent[1]);
                            }
                        }
                    }
                }
            }
        }
    }
    public Type? AsValueOfType { get; set; } = null;

    public string At { get; set; } = string.Empty;

    public int Verbose { get; set; } = 0;

    public bool Strict { get; set; } = DefaultSrict;

    public object? ResourceKey { get; set; }

    public object? DefaultDataContext { get; set; }

    public ParameterizedResourceExtension(object key): this()
    {
        ResourceKey = key;
    }

    public ParameterizedResourceExtension() 
    {
        NotifyInstanceCreated.InstanceCreated?.Invoke(this, NotifyInstanceCreated.s_instanceCreatedArgs);
    }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        if (ResourceKey is null)
        {
            return null;
        }

        if(s_callStacks.Count == 0)
        {
            _seenObjects = new(ReferenceEqualityComparer.Instance);
            if(((IRootObjectProvider?)serviceProvider.GetService(typeof(IRootObjectProvider)))?.RootObject
                    is object rootObject)
            {
                _seenObjects.Add(rootObject);
            }
        }

        _indention = string.Format($"{{0,{s_callStacks.Count}}}{s_callStacks.Count + 1})", "").Replace(" ", s_indentionStep);

        foreach (ParameterizedResourceExtension resource in s_callStacks)
        {
            if (Strict && !resource.Strict)
            {
                Strict = false;
            }
            if (Verbose < resource.Verbose)
            {
                Verbose = resource.Verbose;
            }
            At = resource.At;
        }
        _prompt = $"{_indention}[{ResourceKey}{(string.IsNullOrEmpty(At) ? string.Empty : $"@{At}")}]";


        if (Verbose > 0)
        {
            Console.WriteLine($"{_prompt} < ProvideValue >");
            foreach (string parameterName in _replacements.Keys)
            {
                Console.WriteLine($"{_prompt} < {parameterName}={_replacements[parameterName]} (from {nameof(Replaces)}) >");
            }
        }

        if (s_callStacks.Count > 0)
        {
            foreach (ParameterizedResourceExtension resource in s_callStacks)
            {
                if (Verbose == 0 && resource.Verbose > 0)
                {
                    Console.WriteLine($"{_prompt} < ProvideValue >");
                    foreach (string parameterName in _replacements.Keys)
                    {
                        Console.WriteLine($"{_prompt} < {parameterName}={_replacements[parameterName]} (from {nameof(Replaces)}) >");
                    }
                }
                foreach (string parameterName in resource._replacements.Keys)
                {
                    if (_replacements.TryAdd(parameterName, resource._replacements[parameterName]))
                    {
                        if (Verbose > 0)
                        {
                            Console.WriteLine($"{_prompt} < {parameterName}={resource._replacements[parameterName]} (from {resource.ResourceKey}) >");
                        }
                    }
                }
                _root = resource;
            }
        }
        else
        {
            _root = this;
        }
        _services = serviceProvider;


        s_callStacks.Push(this);

        try
        {
            bool properKey = true;

            if (ResourceKey.ToString()!.StartsWith('$'))
            {
                if (Verbose > 0)
                {
                    Console.Write($"{_indention}< ResourceKey: {ResourceKey}");
                }
                properKey = false;
                if (_replacements.TryGetValue(ResourceKey.ToString()!, out object? newKey))

                {
                    ResourceKey = newKey;
                    _prompt = $"{_indention}[{ResourceKey}{(string.IsNullOrEmpty(At) ? string.Empty : $"@{At}")}]";

                    properKey = true;
                    if (Verbose > 0)
                    {
                        Console.WriteLine($" -> {ResourceKey} (from {nameof(Replaces)}) >");
                    }
                }
                else if (_defaults.TryGetValue(ResourceKey.ToString()!, out object? defaultKey))
                {
                    ResourceKey = defaultKey;
                    _prompt = $"{_indention}[{ResourceKey}{(string.IsNullOrEmpty(At) ? string.Empty : $"@{At}")}]";

                    properKey = true;
                    if (Verbose > 0)
                    {
                        Console.WriteLine($" -> {ResourceKey} (from {nameof(Defaults)}) >");
                    }
                }
            }

            if (properKey)
            {
                try
                {
                    if (AsValueOfType is Type type)
                    {
                        return type.IsAssignableFrom(ResourceKey.GetType()) ? ResourceKey : Convert.ChangeType(ResourceKey, type);
                    }
                    //_value = new StaticResourceExtension(ResourceKey);

                    object? result = null;
                    Exception? exception = null;

                    foreach (ParameterizedResourceExtension resource in s_callStacks.ToArray())
                    {
                        try
                        {
                            result = ResourceKey != null ? new StaticResourceExtension(ResourceKey).ProvideValue(resource._services) : null;
                            exception = null;
                            break;
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }
                    }

                    if(exception != null)
                    {
                        throw new AggregateException(exception);
                    }

                    if(result != null)
                    {
                        List<string> route = new();
                        WalkMarkup(MarkupWriter.GetMarkupObjectFor(result), route);
                    }

                    if (Verbose > 0)
                    {
                        Console.WriteLine($"{_prompt} < Done >");
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ResourceKey} --- {ex}");
                    if (Strict)
                    {
                        throw;
                    }
                }
            }
            else if (Strict)
            {
                throw new XamlParseException($"ResourceKey parameter is not provided: {ResourceKey.ToString()}!");
            }
            else if (Verbose > 0)
            {
                Console.WriteLine($" - is not provided! >");
            }

            return null;
        }
        finally
        {
            s_callStacks.Pop();
        }
    }

    private void WalkMarkup(MarkupObject mo, List<string> route)
    {
        if (!_root!._seenObjects!.Add(mo.Instance))
        {
            return;
        }
        if (mo.Instance is DependencyObject dependencyObject)
        {
            foreach (
                PropertyDescriptor pd in TypeDescriptor.GetProperties(
                    dependencyObject, [new PropertyFilterAttribute(PropertyFilterOptions.All)]
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
                                OnBinding(binding, route);
                            }
                            else if (value is MultiBinding multiBinding)
                            {
                                OnBinding(multiBinding, route);
                            }
                        }
                    }
                }
                catch (Exception) { }

                if (!isBinding)
                {
                    DependencyPropertyDescriptor dpd =
                        DependencyPropertyDescriptor.FromProperty(pd);

                    if (dpd != null)
                    {
                        if (BindingOperations.GetBinding(dependencyObject, dpd.DependencyProperty) is Binding binding1)
                        {
                            route.Add(pd.Name);
                            OnBinding(binding1, route);
                            route.RemoveAt(route.Count - 1);
                        }
                        else if (BindingOperations.GetMultiBinding(dependencyObject, dpd.DependencyProperty) is MultiBinding multiBinding)
                        {
                            route.Add(pd.Name);
                            OnBinding(multiBinding, route);
                            route.RemoveAt(route.Count - 1);
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
                        route.Add(child.GetType().ToString());
                        WalkMarkup(MarkupWriter.GetMarkupObjectFor(child), route);
                        route.RemoveAt(route.Count - 1);
                    }
                }
            }
        }
        foreach (var prop in mo.Properties)
        {
            if (prop.Value is BindingBase)
            {
                OnBinding((BindingBase)prop.Value, route);
            }
            else
            {
                route.Add(prop.Name);
                if (Verbose > 1)
                {
                    Console.WriteLine($"{_prompt} {prop.PropertyType} {string.Join('/', route)}");
                }
                if (prop.DependencyProperty != null)
                {
                    if (BindingOperations.GetBinding((DependencyObject)mo.Instance, prop.DependencyProperty) is Binding binding)
                    {
                        OnBinding(binding, route);
                    }
                    else if (BindingOperations.GetMultiBinding((DependencyObject)mo.Instance, prop.DependencyProperty) is MultiBinding multiBinding)
                    {
                        OnBinding(multiBinding, route);
                    }
                }
                try
                {
                    if (prop.IsComposite)
                    {
                        route[route.Count - 1] += "[]";
                        foreach (var item in prop.Items)
                        {
                            WalkMarkup(item, route);
                        }
                    }
                }
                catch (NullReferenceException) { }
                route.RemoveAt(route.Count - 1);
            }
        }
    }

    private void OnBinding(BindingBase bindingBase, List<string> route)
    {
        if (!_root!._seenObjects!.Add(bindingBase))
        {
            return;
        }
        if (bindingBase is Binding binding)
        {
            if (binding.ElementName is string elementName && elementName.StartsWith('$'))
            {
                if (Verbose > 0)
                {
                    Console.Write($"{_prompt} {string.Join('/', route)} < ElementName: {binding.ElementName}");
                }
                if (_replacements.TryGetValue(elementName, out object? newElementName))
                {
                    binding.ElementName = newElementName?.ToString();
                    if (Verbose > 0)
                    {
                        Console.WriteLine($" -> {binding.ElementName} (from {nameof(Replaces)}) >");
                    }
                }
                else if (_defaults.TryGetValue(elementName, out object? defaultElementName))
                {
                    binding.ElementName = defaultElementName?.ToString();
                    if (Verbose > 0)
                    {
                        Console.WriteLine($" -> {binding.ElementName} (from {nameof(Defaults)}) >");
                    }
                }
                else if (Strict)
                {
                    throw new XamlParseException($"ElementName parameter is not provided: {elementName} at {ResourceKey}");
                }
                else if (Verbose > 0)
                {
                    Console.WriteLine($" - is not provided! >");
                }
            }
            if (
                binding.ConverterParameter is string converterParameter && converterParameter.Contains('$')
                || binding.ConverterParameter is Array 
            )
            {
                object?[] parameters = IUniversalConverter.SplitParameter(binding.ConverterParameter);
                if (Verbose > 0)
                {
                    Console.Write($"{_prompt} {string.Join('/', route)} < ConverterParameter: [{string.Join(',', parameters)}]");
                }
                bool changed = false;
                bool failed = false;
                for(int i = 0; i < parameters.Length; ++i)
                {
                    if (parameters[i] is string str && str.StartsWith('$'))
                    {
                        object? newConverterParameter = null;
                        if(
                            _replacements.TryGetValue(str, out newConverterParameter)
                            || _defaults.TryGetValue(str, out newConverterParameter)
                        )
                        {
                            changed = true;
                            parameters[i] = newConverterParameter;
                        }
                        else
                        {
                            failed = true;
                            if (Strict)
                            {
                                throw new XamlParseException($"ConverterParameter parameter is not provided: {parameters[i]} at {ResourceKey}");
                            }
                            else if (Verbose > 0)
                            {
                                Console.WriteLine($" - is not provided! >");
                            }
                        }
                    }
                }
                if (changed)
                {
                    if(parameters.Length == 1)
                    {
                        binding.ConverterParameter = parameters[0];
                    }
                    else
                    {
                        binding.ConverterParameter = parameters;
                    }
                }
                if (!failed && Verbose > 0)
                {
                    Console.WriteLine($" -> [{string.Join(',', parameters)}] >");
                }
            }
            if (binding.Source is string source && source.Contains('$'))
            {
                if (Verbose > 0)
                {
                    Console.Write($"{_prompt} {string.Join('/', route)} < Source: {binding.Source}");
                }
                string newSource = source;
                foreach (string key in _replacements.Keys.OrderByDescending(k => k))
                {
                    newSource = newSource.Replace(key, _replacements[key]?.ToString());
                }
                foreach (string key in _defaults.Keys.OrderByDescending(k => k))
                {
                    newSource = newSource.Replace(key, _defaults[key]?.ToString());
                }
                binding.Source = newSource;
                if (newSource.Contains('$'))
                {
                    if (Strict)
                    {
                        throw new XamlParseException($"Source parameter is not provided: {newSource} at {ResourceKey}");
                    }
                    else if (Verbose > 0)
                    {
                        Console.WriteLine($" - is not provided! >");
                    }
                }
                else if (Verbose > 0)
                {
                    Console.WriteLine($" -> {binding.Source} >");
                }
            }
            if (binding.Path != null && binding.Path.Path != null && binding.Path.Path.Contains('$'))
            {
                if (Verbose > 0)
                {
                    Console.Write($"{_prompt} {string.Join('/', route)} < Path: {binding.Path.Path}");
                }
                string newPath = binding.Path.Path;
                foreach (string key in _replacements.Keys.OrderByDescending(k => k))
                {
                    newPath = newPath.Replace(key, _replacements[key]?.ToString());
                }
                foreach (string key in _defaults.Keys.OrderByDescending(k => k))
                {
                    newPath = newPath.Replace(key, _defaults[key]?.ToString());
                }
                binding.Path.Path = newPath;
                if (newPath.Contains('$'))
                {
                    if (Strict)
                    {
                        throw new XamlParseException($"Path parameter is not provided: {binding.Path.Path} at {ResourceKey}");
                    }
                    else if (Verbose > 0)
                    {
                        Console.WriteLine($" - is not provided! >");
                    }
                }
                else if (Verbose > 0)
                {
                    Console.WriteLine($" -> {binding.Path.Path} >");
                }
            }
            if (binding.XPath is string xPath && xPath.StartsWith('$'))
            {
                if (Verbose > 0)
                {
                    Console.Write($"{_prompt} {string.Join('/', route)} < XPath: {binding.XPath}");
                }
                string newXPath = xPath;
                foreach (string key in _replacements.Keys.OrderByDescending(k => k))
                {
                    newXPath = newXPath.Replace(key, _replacements[key]?.ToString());
                }
                foreach (string key in _defaults.Keys.OrderByDescending(k => k))
                {
                    newXPath = newXPath.Replace(key, _defaults[key]?.ToString());
                }
                binding.XPath = newXPath;
                if (newXPath.Contains('$'))
                {
                    if (Strict)
                    {
                        throw new XamlParseException($"XPath parameter is not provided: {xPath} at {ResourceKey}");
                    }
                    else if (Verbose > 0)
                    {
                        Console.WriteLine($" - is not provided! >");
                    }
                }
                else if (Verbose > 0)
                {
                    Console.WriteLine($" -> {binding.XPath} >");
                }
            }
            if(binding.ConverterParameter != null)
            {
                WalkMarkup(MarkupWriter.GetMarkupObjectFor(binding.ConverterParameter), route);
            }
        }
        else if (bindingBase is MultiBinding multiBinding)
        {
            if (
                multiBinding.ConverterParameter is string converterParameter && converterParameter.Contains('$')
                || multiBinding.ConverterParameter is Array
            )
            {
                object?[] parameters = IUniversalConverter.SplitParameter(multiBinding.ConverterParameter);
                if (Verbose > 0)
                {
                    Console.Write($"{_prompt} {string.Join('/', route)} < ConverterParameter: [{string.Join(',', parameters)}]");
                }
                bool changed = false;
                bool failed = false;
                for (int i = 0; i < parameters.Length; ++i)
                {
                    if (parameters[i] is string str && str.StartsWith('$'))
                    {
                        object? newConverterParameter = null;
                        if (
                            _replacements.TryGetValue(str, out newConverterParameter)
                            || _defaults.TryGetValue(str, out newConverterParameter)
                        )
                        {
                            changed = true;
                            parameters[i] = newConverterParameter;
                        }
                        else
                        {
                            failed = true;
                            if (Strict)
                            {
                                throw new XamlParseException($"ConverterParameter parameter is not provided: {parameters[i]} at {ResourceKey}");
                            }
                            else if (Verbose > 0)
                            {
                                Console.WriteLine($" - is not provided! >");
                            }
                        }
                    }
                }
                if (changed)
                {
                    if (parameters.Length == 1)
                    {
                        multiBinding.ConverterParameter = parameters[0];
                    }
                    else
                    {
                        multiBinding.ConverterParameter = parameters;
                    }
                }
                if (!failed && Verbose > 0)
                {
                    Console.WriteLine($" -> [{string.Join(',', parameters)}] >");
                }
            }
            if (multiBinding.ConverterParameter != null)
            {
                WalkMarkup(MarkupWriter.GetMarkupObjectFor(multiBinding.ConverterParameter), route);
            }
            foreach (Binding bindingItem in multiBinding.Bindings)
            {
                OnBinding(bindingItem, route);
            }
        }
    }
}

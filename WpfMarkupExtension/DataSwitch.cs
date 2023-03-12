using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Net.Leksi.WpfMarkup;

[MarkupExtensionReturnType(typeof(DataTrigger))]
[ContentProperty("Triggers")]

public class DataSwitch : MarkupExtension
{
    private BindingBase? _binding = null;

    public BindingProxy BindingProxy { get; init; } = new();

    public List<DataTrigger> Triggers { get; init; } = new();

    public BindingBase? Binding
    {
        get => _binding;
        set
        {
            Console.WriteLine($"here");
            _binding = value;
            BindingOperations.SetBinding(BindingProxy, BindingProxy.ValueProperty, value); 
        }
    }

    public DataSwitch()
    {
       
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        IProvideValueTarget target = serviceProvider.GetRequiredService<IProvideValueTarget>();

        DataTrigger result = new();

        if(target.TargetObject is TriggerCollection triggers)
        {
            foreach (DataTrigger trigger in Triggers)
            {
                Console.WriteLine($"     {trigger}");
                Binding binding = new("Value") { Source = BindingProxy, NotifyOnSourceUpdated = true };
                trigger.Binding = binding;
                triggers.Add(trigger);
            }
            Console.WriteLine($"     {triggers.Count}");
        }

        return result;
    }

}

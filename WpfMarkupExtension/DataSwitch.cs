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
    public List<DataTrigger> Triggers { get; init; } = new();

    public BindingBase? Binding { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        IProvideValueTarget target = serviceProvider.GetRequiredService<IProvideValueTarget>();

        DataTrigger result = new();

        if (target.TargetObject is TriggerCollection triggers)
        {
            foreach (DataTrigger trigger in Triggers)
            {
                if(trigger.Value is Array values)
                {
                    foreach(var value in values)
                    {
                        DataTrigger trigger1 = new DataTrigger();
                        trigger1.Value = value;
                        trigger1.Binding = Binding;
                        foreach(SetterBase? setter in trigger.Setters)
                        {
                            trigger1.Setters.Add(setter);
                        }
                        triggers.Add(trigger1);
                    }
                }
                else
                {
                    trigger.Binding = Binding;
                    triggers.Add(trigger);
                }
            }
        }

        return result;
    }

}

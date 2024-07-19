using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
namespace Net.Leksi.WpfMarkup;
[MarkupExtensionReturnType(typeof(Style))]
[ContentProperty("Styles")]
[DictionaryKeyProperty("TargetType")]
public class StyleCombiner : MarkupExtension
{
    public List<Style> Styles { get; init; } = new();
    public Type? TargetType { get; set; } = null;
    public StyleCombiner()
    {
        NotifyInstanceCreated.InstanceCreated?.Invoke(this, NotifyInstanceCreated.s_instanceCreatedArgs);
    }
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Styles.Count == 1)
        {
            return Styles[0];
        }

        Style result = new();

        if (Styles.Count > 0)
        {
            Stack<Style> stack = new();

            foreach (Style style in Styles)
            {
                stack.Push(style);
                while (stack.Peek().BasedOn is Style basedOn)
                {
                    stack.Push(basedOn);
                }
                while (stack.TryPop(out Style? current))
                {
                    foreach (TriggerBase trigger in current.Triggers)
                    {
                        result.Triggers.Add(trigger);
                    }
                    foreach (SetterBase setter in current.Setters)
                    {
                        result.Setters.Add(setter);
                    }
                }
            }
        }
        return result;
    }
}

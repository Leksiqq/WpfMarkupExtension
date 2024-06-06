using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Net.Leksi.WpfMarkup;

public class BoolExpressionConverter : IMultiValueConverter
{
    private const char s_parameterPrefix = '@';
    private StringBuilder? _sb;
    public string PostfixRecord => _sb?.ToString().TrimEnd() ?? string.Empty;
    public bool Verbose {  get; set; } = false;
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if(parameter is string expression)
        {
            return Evaluate(values.Select(v => v is bool b && b).ToArray(), expression);  
        }
        if(values.Length == 1)
        {
            return values[0] is bool b && b;
        }
        return false;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    private bool Evaluate(bool[] values, string expression)
    {
        Stack<bool> operands = [];
        Stack<char> operations = [];
        int pos = -1;
        bool pushedOperand = false;
        if (Verbose)
        {
            _sb ??= new StringBuilder();
        }
        _sb?.Clear();

        foreach (char ch in expression)
        {
            if(ch >= '0' && ch <= '9')
            {
                if (pos < 0)
                {
                    throw new Exception();
                }
                pos *= 10;
                pos += ch - '0';
            }
            else
            {
                PushOperand();
                switch (ch)
                {
                    case '!' or '(':
                        operations.Push(ch);
                        pushedOperand = false;
                        break;
                    case '|':
                        while (operations.TryPeek(out char next) && (next == '&' || next == ch))
                        {
                            Apply(operands, next);
                            operations.Pop();
                        }
                        PushOperation(ch);
                        break;
                    case '&':
                        while (operations.TryPeek(out char next) && next == ch)
                        {
                            Apply(operands, next);
                            operations.Pop();
                        }
                        PushOperation(ch);
                        break;
                    case ')':
                        while (operations.TryPeek(out char next) && next != '(')
                        {
                            Apply(operands, next);
                            operations.Pop();
                        }
                        if (!operations.TryPop(out char par) || par != '(')
                        {
                            throw new Exception();
                        }
                        while (operations.TryPeek(out char next) && next == '!')
                        {
                            Apply(operands, next);
                            operations.Pop();
                        }
                        break;
                    case s_parameterPrefix:
                        pos = 0;
                        break;
                    case ' ' or '\t' or '\n' or '\r':
                        break;
                    default:
                        throw new Exception();
                }
            }
        }
        PushOperand();
        while (operations.TryPop(out char par))
        {
            if(par == '(')
            {
                throw new Exception();
            }
            Apply(operands, par);
        }
        if (operands.Count != 1)
        {
            throw new Exception();
        }
        return operands.Pop();

        void PushOperation(char ch)
        {
            operations.Push(ch);
            pushedOperand = false;
        };
        void PushOperand()
        {
            if (pos >= 0)
            {
                if (pushedOperand || pos >= values.Length)
                {
                    throw new Exception();
                }
                _sb?.Append($"{s_parameterPrefix}{pos} ");
                operands.Push(values[pos]);
                pushedOperand = true;
                pos = -1;
                while (operations.TryPeek(out char next) && next == '!')
                {
                    Apply(operands, next);
                    operations.Pop();
                }
            }
        }
    }

    private void Apply(Stack<bool> operands, char next)
    {
        _sb?.Append($"{next} ");
        bool arg1;
        bool arg2;
        switch (next)
        {
            case '!':
                arg1 = BoolExpressionConverter.PopOrThrow(operands);
                operands.Push(!arg1);
                break;
            case '|':
                arg2 = BoolExpressionConverter.PopOrThrow(operands);
                arg1 = BoolExpressionConverter.PopOrThrow(operands);
                operands.Push(arg2 || arg1);
                break;
            case '&':
                arg2 = BoolExpressionConverter.PopOrThrow(operands);
                arg1 = BoolExpressionConverter.PopOrThrow(operands);
                operands.Push(arg2 && arg1);
                break;
        }
    }

    private static bool PopOrThrow(Stack<bool> operands)
    {
        if(operands.TryPop(out bool b))
        {
            return b;
        }
        throw new Exception();
    }
}

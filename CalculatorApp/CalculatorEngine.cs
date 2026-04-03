using System.Globalization;
using System.Text;

namespace CalculatorApp;

/// <summary>
/// Contains expression editing, parsing and scientific evaluation logic independent of the UI.
/// </summary>
public sealed class CalculatorEngine
{
    private readonly StringBuilder _expression = new();
    private double _memory;

    public bool IsDegreeMode { get; set; } = true;

    public bool HasMemory => Math.Abs(_memory) > 1e-12;

    public string ExpressionText => _expression.Length == 0 ? "0" : _expression.ToString();

    public string DisplayText => _expression.Length == 0 ? "0" : _expression.ToString();

    public void InputDigit(string digit)
    {
        AppendWithImplicitMultiply(digit, startsValue: true);
    }

    public void InputDecimalPoint()
    {
        if (EndsWithNumberTokenContainingDecimal())
        {
            return;
        }

        if (_expression.Length == 0 || EndsWithOperatorOrOpenParen())
        {
            AppendWithImplicitMultiply("0.", startsValue: true);
            return;
        }

        _expression.Append('.');
    }

    public void InputOperator(string op)
    {
        if (op == "!")
        {
            if (_expression.Length > 0 && !EndsWithOperatorOrOpenParen())
            {
                _expression.Append('!');
            }

            return;
        }

        if (_expression.Length == 0)
        {
            if (op == "-")
            {
                _expression.Append('-');
            }

            return;
        }

        if (EndsWithBinaryOperator())
        {
            _expression[^1] = op[0];
            return;
        }

        _expression.Append(op);
    }

    public void InputParenthesis(string parenthesis)
    {
        if (parenthesis == "(")
        {
            AppendWithImplicitMultiply("(", startsValue: true);
        }
        else if (parenthesis == ")" && CanCloseParenthesis())
        {
            _expression.Append(')');
        }
    }

    public void InputFunction(string function)
    {
        var mapped = function switch
        {
            "√" => "sqrt(",
            "|x|" => "abs(",
            "1/x" => "inv(",
            _ => function + "("
        };

        AppendWithImplicitMultiply(mapped, startsValue: true);
    }

    public void InputPower(string power)
    {
        switch (power)
        {
            case "x²":
                _expression.Append("^2");
                break;
            case "x³":
                _expression.Append("^3");
                break;
            case "xʸ":
                _expression.Append('^');
                break;
        }
    }

    public void InputConstant(string constant)
    {
        var token = constant switch
        {
            "π" => "pi",
            "e" => "e",
            _ => constant
        };

        AppendWithImplicitMultiply(token, startsValue: true);
    }

    public void ToggleSign()
    {
        if (_expression.Length == 0)
        {
            _expression.Append('-');
            return;
        }

        var start = FindCurrentValueStart();
        if (start < 0)
        {
            _expression.Insert(0, '-');
            return;
        }

        if (start > 0 && _expression[start - 1] == '-')
        {
            _expression.Remove(start - 1, 1);
        }
        else
        {
            _expression.Insert(start, "-");
        }
    }

    public string Evaluate()
    {
        if (_expression.Length == 0)
        {
            return "0";
        }

        var result = EvaluateExpression(_expression.ToString());
        var formatted = result.ToString("G15", CultureInfo.InvariantCulture);
        _expression.Clear();
        _expression.Append(formatted);
        return formatted;
    }

    public void ClearAll() => _expression.Clear();

    public void ClearEntry()
    {
        var start = FindCurrentValueStart();
        if (start >= 0)
        {
            _expression.Remove(start, _expression.Length - start);
        }
        else
        {
            _expression.Clear();
        }
    }

    public void Backspace()
    {
        if (_expression.Length > 0)
        {
            _expression.Remove(_expression.Length - 1, 1);
        }
    }

    public void MemoryClear() => _memory = 0;

    public void MemoryRecall()
    {
        AppendWithImplicitMultiply(_memory.ToString("G15", CultureInfo.InvariantCulture), startsValue: true);
    }

    public void MemoryAdd() => _memory += EvaluateExpression(_expression.Length == 0 ? "0" : _expression.ToString());

    public void MemorySubtract() => _memory -= EvaluateExpression(_expression.Length == 0 ? "0" : _expression.ToString());

    private void AppendWithImplicitMultiply(string token, bool startsValue)
    {
        if (startsValue && _expression.Length > 0 && EndsWithValue())
        {
            _expression.Append('*');
        }

        _expression.Append(token);
    }

    private bool EndsWithValue()
    {
        var last = _expression[^1];
        return char.IsDigit(last) || last == ')' || last == 'i' || last == 'e' || last == '!';
    }

    private bool EndsWithOperatorOrOpenParen()
    {
        if (_expression.Length == 0)
        {
            return true;
        }

        var last = _expression[^1];
        return last is '+' or '-' or '*' or '/' or '^' or '(';
    }

    private bool EndsWithBinaryOperator()
    {
        if (_expression.Length == 0)
        {
            return false;
        }

        return _expression[^1] is '+' or '-' or '*' or '/' or '^';
    }

    private bool EndsWithNumberTokenContainingDecimal()
    {
        for (var i = _expression.Length - 1; i >= 0; i--)
        {
            var c = _expression[i];
            if (c == '.')
            {
                return true;
            }

            if (!char.IsDigit(c))
            {
                break;
            }
        }

        return false;
    }

    private bool CanCloseParenthesis()
    {
        if (_expression.Length == 0 || EndsWithOperatorOrOpenParen())
        {
            return false;
        }

        var balance = 0;
        foreach (var c in _expression.ToString())
        {
            if (c == '(') balance++;
            if (c == ')') balance--;
        }

        return balance > 0;
    }

    private int FindCurrentValueStart()
    {
        for (var i = _expression.Length - 1; i >= 0; i--)
        {
            var c = _expression[i];
            if (c is '+' or '-' or '*' or '/' or '^' or '(')
            {
                return i + 1;
            }
        }

        return 0;
    }

    private double EvaluateExpression(string expression)
    {
        var tokens = Tokenize(expression);
        var rpn = ToRpn(tokens);
        return EvaluateRpn(rpn);
    }

    private static List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        var i = 0;

        while (i < expression.Length)
        {
            var c = expression[i];
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            if (char.IsDigit(c) || c == '.')
            {
                var start = i;
                while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                {
                    i++;
                }

                tokens.Add(expression[start..i]);
                continue;
            }

            if (char.IsLetter(c))
            {
                var start = i;
                while (i < expression.Length && char.IsLetter(expression[i]))
                {
                    i++;
                }

                tokens.Add(expression[start..i].ToLowerInvariant());
                continue;
            }

            if ("+-*/^()!".Contains(c))
            {
                tokens.Add(c.ToString());
                i++;
                continue;
            }

            throw new InvalidOperationException($"Invalid token '{c}'.");
        }

        return tokens;
    }

    /// <summary>
    /// Converts infix expression tokens to postfix using the Shunting Yard algorithm.
    /// </summary>
    private static List<string> ToRpn(List<string> tokens)
    {
        var output = new List<string>();
        var operators = new Stack<string>();
        string? previous = null;

        foreach (var token in tokens)
        {
            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out _)
                || token is "pi" or "e")
            {
                output.Add(token);
            }
            else if (IsFunction(token))
            {
                operators.Push(token);
            }
            else if (token == "(")
            {
                operators.Push(token);
            }
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }

                if (operators.Count == 0)
                {
                    throw new InvalidOperationException("Mismatched parentheses.");
                }

                operators.Pop();
                if (operators.Count > 0 && IsFunction(operators.Peek()))
                {
                    output.Add(operators.Pop());
                }
            }
            else
            {
                var current = token;
                if (current == "-" && (previous is null || previous is "(" || IsOperator(previous)))
                {
                    current = "neg";
                }

                while (operators.Count > 0 && IsOperator(operators.Peek()) &&
                       ((IsLeftAssociative(current) && Precedence(current) <= Precedence(operators.Peek())) ||
                        (!IsLeftAssociative(current) && Precedence(current) < Precedence(operators.Peek()))))
                {
                    output.Add(operators.Pop());
                }

                operators.Push(current);
            }

            previous = token;
        }

        while (operators.Count > 0)
        {
            var op = operators.Pop();
            if (op is "(" or ")")
            {
                throw new InvalidOperationException("Mismatched parentheses.");
            }

            output.Add(op);
        }

        return output;
    }

    private double EvaluateRpn(List<string> rpn)
    {
        var stack = new Stack<double>();

        foreach (var token in rpn)
        {
            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            {
                stack.Push(number);
                continue;
            }

            if (token == "pi")
            {
                stack.Push(Math.PI);
                continue;
            }

            if (token == "e")
            {
                stack.Push(Math.E);
                continue;
            }

            if (IsFunction(token) || token == "neg")
            {
                if (stack.Count < 1)
                {
                    throw new InvalidOperationException("Invalid expression.");
                }

                var value = stack.Pop();
                stack.Push(ApplyFunction(token, value));
                continue;
            }

            if (stack.Count < 2)
            {
                throw new InvalidOperationException("Invalid expression.");
            }

            var right = stack.Pop();
            var left = stack.Pop();
            stack.Push(ApplyOperator(token, left, right));
        }

        if (stack.Count != 1)
        {
            throw new InvalidOperationException("Invalid expression.");
        }

        return stack.Pop();
    }

    private static double ApplyOperator(string op, double left, double right)
    {
        return op switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" when Math.Abs(right) < 1e-12 => throw new DivideByZeroException("Cannot divide by zero."),
            "/" => left / right,
            "^" => Math.Pow(left, right),
            _ => throw new InvalidOperationException($"Unsupported operator '{op}'.")
        };
    }

    private double ApplyFunction(string function, double value)
    {
        var radians = IsDegreeMode ? value * (Math.PI / 180.0) : value;
        return function switch
        {
            "sin" => Math.Sin(radians),
            "cos" => Math.Cos(radians),
            "tan" => Math.Tan(radians),
            "asin" => InverseTrig(Math.Asin(value)),
            "acos" => InverseTrig(Math.Acos(value)),
            "atan" => InverseTrig(Math.Atan(value)),
            "log" => Math.Log10(value),
            "ln" => Math.Log(value),
            "exp" => Math.Exp(value),
            "sqrt" => Math.Sqrt(value),
            "cbrt" => Math.Cbrt(value),
            "abs" => Math.Abs(value),
            "inv" when Math.Abs(value) < 1e-12 => throw new DivideByZeroException("Cannot divide by zero."),
            "inv" => 1 / value,
            "!" => Factorial(value),
            "neg" => -value,
            _ => throw new InvalidOperationException($"Unsupported function '{function}'.")
        };
    }

    private double InverseTrig(double radians)
    {
        return IsDegreeMode ? radians * (180.0 / Math.PI) : radians;
    }

    private static double Factorial(double value)
    {
        if (value < 0 || Math.Abs(value - Math.Round(value)) > 1e-10)
        {
            throw new InvalidOperationException("Factorial is defined only for non-negative integers.");
        }

        double result = 1;
        for (var i = 2; i <= (int)value; i++)
        {
            result *= i;
        }

        return result;
    }

    private static bool IsFunction(string token)
    {
        return token is "sin" or "cos" or "tan" or "asin" or "acos" or "atan" or "log" or "ln" or "sqrt" or "cbrt" or "abs" or "exp" or "inv";
    }

    private static bool IsOperator(string token) => token is "+" or "-" or "*" or "/" or "^" or "!" or "neg";

    private static int Precedence(string op) => op switch
    {
        "!" => 5,
        "^" => 4,
        "neg" => 3,
        "*" or "/" => 2,
        "+" or "-" => 1,
        _ => 0
    };

    private static bool IsLeftAssociative(string op) => op is not "^" and not "neg";
}

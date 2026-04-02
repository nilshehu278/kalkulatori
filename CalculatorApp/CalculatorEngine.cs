using System.Globalization;

namespace CalculatorApp;

/// <summary>
/// Encapsulates calculator state and arithmetic logic independently from the form.
/// </summary>
public sealed class CalculatorEngine
{
    private readonly List<string> _expressionTokens = [];
    private string _currentEntry = "0";
    private bool _startNewEntry;

    public string DisplayText => _currentEntry;

    public void InputDigit(string digit)
    {
        if (_startNewEntry)
        {
            _currentEntry = digit;
            _startNewEntry = false;
            return;
        }

        if (_currentEntry == "0")
        {
            _currentEntry = digit;
            return;
        }

        _currentEntry += digit;
    }

    public void InputDecimalPoint()
    {
        if (_startNewEntry)
        {
            _currentEntry = "0";
            _startNewEntry = false;
        }

        if (!_currentEntry.Contains('.'))
        {
            _currentEntry += ".";
        }
    }

    public void SetOperator(string op)
    {
        if (!_startNewEntry)
        {
            _expressionTokens.Add(_currentEntry);
            _startNewEntry = true;
        }

        if (_expressionTokens.Count > 0 && IsOperator(_expressionTokens[^1]))
        {
            _expressionTokens[^1] = op;
            return;
        }

        _expressionTokens.Add(op);
    }

    public void ClearAll()
    {
        _expressionTokens.Clear();
        _currentEntry = "0";
        _startNewEntry = false;
    }

    public void ClearEntry()
    {
        _currentEntry = "0";
        _startNewEntry = false;
    }

    public void Backspace()
    {
        if (_startNewEntry)
        {
            return;
        }

        if (_currentEntry.Length <= 1)
        {
            _currentEntry = "0";
            return;
        }

        _currentEntry = _currentEntry[..^1];
    }

    public string Evaluate()
    {
        if (!_startNewEntry)
        {
            _expressionTokens.Add(_currentEntry);
        }

        if (_expressionTokens.Count == 0)
        {
            return _currentEntry;
        }

        if (IsOperator(_expressionTokens[^1]))
        {
            _expressionTokens.RemoveAt(_expressionTokens.Count - 1);
        }

        var result = EvaluateTokens(_expressionTokens);
        _expressionTokens.Clear();
        _currentEntry = result.ToString(CultureInfo.InvariantCulture);
        _startNewEntry = true;
        return _currentEntry;
    }

    private static bool IsOperator(string token) => token is "+" or "-" or "*" or "/";

    private static int Precedence(string op) => op is "*" or "/" ? 2 : 1;

    private static double EvaluateTokens(List<string> tokens)
    {
        var values = new Stack<double>();
        var operators = new Stack<string>();

        foreach (var token in tokens)
        {
            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            {
                values.Push(number);
                continue;
            }

            while (operators.Count > 0 && Precedence(operators.Peek()) >= Precedence(token))
            {
                ApplyTopOperator(values, operators);
            }

            operators.Push(token);
        }

        while (operators.Count > 0)
        {
            ApplyTopOperator(values, operators);
        }

        return values.Pop();
    }

    private static void ApplyTopOperator(Stack<double> values, Stack<string> operators)
    {
        var op = operators.Pop();
        var right = values.Pop();
        var left = values.Pop();

        var result = op switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" when right == 0 => throw new DivideByZeroException(),
            "/" => left / right,
            _ => throw new InvalidOperationException($"Unsupported operator: {op}")
        };

        values.Push(result);
    }
}

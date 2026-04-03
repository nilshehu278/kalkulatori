using System.Globalization;

namespace CalculatorApp;

public partial class Form1 : Form
{
    private readonly CalculatorEngine _engine = new();

    public Form1()
    {
        InitializeComponent();
        KeyPreview = true;
        UpdateDisplay();
    }

    private void DigitButton_Click(object sender, EventArgs e)
    {
        if (sender is Button { Tag: string digit })
        {
            _engine.InputDigit(digit);
            UpdateDisplay();
        }
    }

    private void OperatorButton_Click(object sender, EventArgs e)
    {
        if (sender is Button { Tag: string op })
        {
            _engine.SetOperator(op);
            UpdateDisplay();
        }
    }

    private void DecimalButton_Click(object sender, EventArgs e)
    {
        _engine.InputDecimalPoint();
        UpdateDisplay();
    }

    private void EqualsButton_Click(object sender, EventArgs e)
    {
        TryEvaluate();
    }

    private void ClearButton_Click(object sender, EventArgs e)
    {
        _engine.ClearAll();
        UpdateDisplay();
    }

    private void ClearEntryButton_Click(object sender, EventArgs e)
    {
        _engine.ClearEntry();
        UpdateDisplay();
    }

    private void BackspaceButton_Click(object sender, EventArgs e)
    {
        _engine.Backspace();
        UpdateDisplay();
    }

    private void Form1_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode is >= Keys.D0 and <= Keys.D9 && !e.Shift)
        {
            _engine.InputDigit(((int)(e.KeyCode - Keys.D0)).ToString(CultureInfo.InvariantCulture));
            UpdateDisplay();
            e.Handled = true;
            return;
        }

        if (e.KeyCode is >= Keys.NumPad0 and <= Keys.NumPad9)
        {
            _engine.InputDigit(((int)(e.KeyCode - Keys.NumPad0)).ToString(CultureInfo.InvariantCulture));
            UpdateDisplay();
            e.Handled = true;
            return;
        }

        switch (e.KeyCode)
        {
            case Keys.Add:
            case Keys.Oemplus when e.Shift:
                _engine.SetOperator("+");
                break;
            case Keys.Subtract:
            case Keys.OemMinus:
                _engine.SetOperator("-");
                break;
            case Keys.Multiply:
                _engine.SetOperator("*");
                break;
            case Keys.Divide:
            case Keys.OemQuestion:
                _engine.SetOperator("/");
                break;
            case Keys.Decimal:
            case Keys.OemPeriod:
                _engine.InputDecimalPoint();
                break;
            case Keys.Enter:
                TryEvaluate();
                e.Handled = true;
                return;
            case Keys.Back:
                _engine.Backspace();
                break;
            case Keys.Delete:
                _engine.ClearEntry();
                break;
            case Keys.Escape:
                _engine.ClearAll();
                break;
            default:
                return;
        }

        UpdateDisplay();
        e.Handled = true;
    }

    private void TryEvaluate()
    {
        try
        {
            _engine.Evaluate();
            UpdateDisplay();
        }
        catch (DivideByZeroException)
        {
            MessageBox.Show("Cannot divide by zero.", "Calculation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _engine.ClearAll();
            UpdateDisplay();
        }
    }

    private void UpdateDisplay()
    {
        txtDisplay.Text = _engine.DisplayText;
    }
}

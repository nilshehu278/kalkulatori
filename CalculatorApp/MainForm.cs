using System.Globalization;

namespace CalculatorApp;

public partial class MainForm : Form
{
    private readonly CalculatorEngine _engine = new();

    public MainForm()
    {
        InitializeComponent();
        KeyPreview = true;
        UpdateDisplay();
    }

    private void CalculatorButton_Click(object? sender, EventArgs e)
    {
        if (sender is not Button { Tag: string action })
        {
            return;
        }

        try
        {
            switch (action)
            {
                case "digit":
                    _engine.InputDigit(((Button)sender).Text);
                    break;
                case "decimal":
                    _engine.InputDecimalPoint();
                    break;
                case "operator":
                    _engine.InputOperator(((Button)sender).Text);
                    break;
                case "equals":
                    _engine.Evaluate();
                    break;
                case "clear":
                    _engine.ClearAll();
                    break;
                case "clear-entry":
                    _engine.ClearEntry();
                    break;
                case "backspace":
                    _engine.Backspace();
                    break;
                case "paren":
                    _engine.InputParenthesis(((Button)sender).Text);
                    break;
                case "function":
                    _engine.InputFunction(((Button)sender).Text);
                    break;
                case "power":
                    _engine.InputPower(((Button)sender).Text);
                    break;
                case "constant":
                    _engine.InputConstant(((Button)sender).Text);
                    break;
                case "sign":
                    _engine.ToggleSign();
                    break;
                case "memory-clear":
                    _engine.MemoryClear();
                    break;
                case "memory-recall":
                    _engine.MemoryRecall();
                    break;
                case "memory-add":
                    _engine.MemoryAdd();
                    break;
                case "memory-subtract":
                    _engine.MemorySubtract();
                    break;
                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Calculation error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _engine.ClearEntry();
        }

        UpdateDisplay();
    }

    private void DegreeModeToggle_CheckedChanged(object? sender, EventArgs e)
    {
        _engine.IsDegreeMode = chkDegreeMode.Checked;
        chkDegreeMode.Text = chkDegreeMode.Checked ? "DEG" : "RAD";
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode is >= Keys.D0 and <= Keys.D9 && !e.Shift)
        {
            _engine.InputDigit(((int)(e.KeyCode - Keys.D0)).ToString(CultureInfo.InvariantCulture));
        }
        else if (e.KeyCode is >= Keys.NumPad0 and <= Keys.NumPad9)
        {
            _engine.InputDigit(((int)(e.KeyCode - Keys.NumPad0)).ToString(CultureInfo.InvariantCulture));
        }
        else
        {
            switch (e.KeyCode)
            {
                case Keys.Add:
                case Keys.Oemplus when e.Shift:
                    _engine.InputOperator("+");
                    break;
                case Keys.Subtract:
                case Keys.OemMinus:
                    _engine.InputOperator("-");
                    break;
                case Keys.Multiply:
                    _engine.InputOperator("*");
                    break;
                case Keys.Divide:
                case Keys.OemQuestion:
                    _engine.InputOperator("/");
                    break;
                case Keys.Decimal:
                case Keys.OemPeriod:
                    _engine.InputDecimalPoint();
                    break;
                case Keys.Enter:
                    _engine.Evaluate();
                    break;
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
        }

        UpdateDisplay();
        e.Handled = true;
    }

    private void UpdateDisplay()
    {
        txtExpression.Text = _engine.ExpressionText;
        txtDisplay.Text = _engine.DisplayText;
        lblMemory.Text = _engine.HasMemory ? "M" : string.Empty;
    }
}

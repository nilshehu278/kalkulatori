namespace CalculatorApp;

partial class MainForm
{
    private System.ComponentModel.IContainer? components = null;
    private TextBox txtDisplay = null!;
    private TextBox txtExpression = null!;
    private Label lblMemory = null!;
    private CheckBox chkDegreeMode = null!;
    private TableLayoutPanel tableLayoutPanel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        txtDisplay = new TextBox();
        txtExpression = new TextBox();
        lblMemory = new Label();
        chkDegreeMode = new CheckBox();
        tableLayoutPanel = new TableLayoutPanel();
        SuspendLayout();

        txtExpression.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtExpression.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        txtExpression.Location = new Point(12, 12);
        txtExpression.ReadOnly = true;
        txtExpression.Size = new Size(646, 27);
        txtExpression.TextAlign = HorizontalAlignment.Right;

        txtDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtDisplay.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point);
        txtDisplay.Location = new Point(12, 45);
        txtDisplay.ReadOnly = true;
        txtDisplay.Size = new Size(646, 50);
        txtDisplay.Text = "0";
        txtDisplay.TextAlign = HorizontalAlignment.Right;

        lblMemory.AutoSize = true;
        lblMemory.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        lblMemory.Location = new Point(12, 98);
        lblMemory.Size = new Size(0, 19);

        chkDegreeMode.Appearance = Appearance.Button;
        chkDegreeMode.AutoSize = false;
        chkDegreeMode.Checked = true;
        chkDegreeMode.CheckState = CheckState.Checked;
        chkDegreeMode.Location = new Point(598, 98);
        chkDegreeMode.Size = new Size(60, 25);
        chkDegreeMode.Text = "DEG";
        chkDegreeMode.TextAlign = ContentAlignment.MiddleCenter;
        chkDegreeMode.CheckedChanged += DegreeModeToggle_CheckedChanged;

        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.ColumnCount = 6;
        for (var i = 0; i < 6; i++)
        {
            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.6667F));
        }

        tableLayoutPanel.RowCount = 8;
        for (var i = 0; i < 8; i++)
        {
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 12.5F));
        }

        tableLayoutPanel.Location = new Point(12, 130);
        tableLayoutPanel.Size = new Size(646, 420);

        AddButton("MC", "memory-clear", 0, 0);
        AddButton("MR", "memory-recall", 1, 0);
        AddButton("M+", "memory-add", 2, 0);
        AddButton("M-", "memory-subtract", 3, 0);
        AddButton("CE", "clear-entry", 4, 0);
        AddButton("C", "clear", 5, 0);

        AddButton("sin", "function", 0, 1);
        AddButton("cos", "function", 1, 1);
        AddButton("tan", "function", 2, 1);
        AddButton("asin", "function", 3, 1);
        AddButton("acos", "function", 4, 1);
        AddButton("atan", "function", 5, 1);

        AddButton("log", "function", 0, 2);
        AddButton("ln", "function", 1, 2);
        AddButton("exp", "function", 2, 2);
        AddButton("√", "function", 3, 2);
        AddButton("cbrt", "function", 4, 2);
        AddButton("|x|", "function", 5, 2);

        AddButton("(", "paren", 0, 3);
        AddButton(")", "paren", 1, 3);
        AddButton("x²", "power", 2, 3);
        AddButton("x³", "power", 3, 3);
        AddButton("xʸ", "power", 4, 3);
        AddButton("!", "operator", 5, 3);

        AddButton("π", "constant", 0, 4);
        AddButton("e", "constant", 1, 4);
        AddButton("1/x", "function", 2, 4);
        AddButton("±", "sign", 3, 4);
        AddButton("⌫", "backspace", 4, 4);
        AddButton("/", "operator", 5, 4);

        AddButton("7", "digit", 0, 5);
        AddButton("8", "digit", 1, 5);
        AddButton("9", "digit", 2, 5);
        AddButton("*", "operator", 3, 5);
        AddButton("4", "digit", 4, 5);
        AddButton("5", "digit", 5, 5);

        AddButton("6", "digit", 0, 6);
        AddButton("-", "operator", 1, 6);
        AddButton("1", "digit", 2, 6);
        AddButton("2", "digit", 3, 6);
        AddButton("3", "digit", 4, 6);
        AddButton("+", "operator", 5, 6);

        AddButton("0", "digit", 0, 7);
        AddButton(".", "decimal", 1, 7);
        AddButton("=", "equals", 2, 7);
        tableLayoutPanel.SetColumnSpan(tableLayoutPanel.Controls[^1], 4);

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(670, 562);
        Controls.Add(tableLayoutPanel);
        Controls.Add(chkDegreeMode);
        Controls.Add(lblMemory);
        Controls.Add(txtDisplay);
        Controls.Add(txtExpression);
        MinimumSize = new Size(686, 601);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Scientific Calculator";
        KeyDown += MainForm_KeyDown;
        ResumeLayout(false);
        PerformLayout();
    }

    private void AddButton(string text, string action, int column, int row)
    {
        var button = new Button
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point),
            Margin = new Padding(3),
            Text = text,
            Tag = action,
            UseVisualStyleBackColor = true
        };

        button.Click += CalculatorButton_Click;
        tableLayoutPanel.Controls.Add(button, column, row);
    }
}

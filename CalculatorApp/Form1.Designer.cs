namespace CalculatorApp;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;
    private TextBox txtDisplay = null!;
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
        tableLayoutPanel = new TableLayoutPanel();
        SuspendLayout();
        // 
        // txtDisplay
        // 
        txtDisplay.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtDisplay.Font = new Font("Segoe UI", 20F, FontStyle.Regular, GraphicsUnit.Point);
        txtDisplay.Location = new Point(12, 12);
        txtDisplay.Name = "txtDisplay";
        txtDisplay.ReadOnly = true;
        txtDisplay.Size = new Size(340, 43);
        txtDisplay.TabIndex = 0;
        txtDisplay.Text = "0";
        txtDisplay.TextAlign = HorizontalAlignment.Right;
        // 
        // tableLayoutPanel
        // 
        tableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tableLayoutPanel.ColumnCount = 4;
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tableLayoutPanel.Location = new Point(12, 68);
        tableLayoutPanel.Name = "tableLayoutPanel";
        tableLayoutPanel.RowCount = 5;
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
        tableLayoutPanel.Size = new Size(340, 320);
        tableLayoutPanel.TabIndex = 1;

        AddButton("CE", 0, 0, ClearEntryButton_Click);
        AddButton("C", 1, 0, ClearButton_Click);
        AddButton("⌫", 2, 0, BackspaceButton_Click);
        AddOperatorButton("/", 3, 0);

        AddDigitButton("7", 0, 1);
        AddDigitButton("8", 1, 1);
        AddDigitButton("9", 2, 1);
        AddOperatorButton("*", 3, 1);

        AddDigitButton("4", 0, 2);
        AddDigitButton("5", 1, 2);
        AddDigitButton("6", 2, 2);
        AddOperatorButton("-", 3, 2);

        AddDigitButton("1", 0, 3);
        AddDigitButton("2", 1, 3);
        AddDigitButton("3", 2, 3);
        AddOperatorButton("+", 3, 3);

        AddDigitButton("0", 0, 4);
        tableLayoutPanel.SetColumnSpan(tableLayoutPanel.Controls[^1], 2);
        AddButton(".", 2, 4, DecimalButton_Click);
        AddButton("=", 3, 4, EqualsButton_Click);

        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(364, 401);
        Controls.Add(tableLayoutPanel);
        Controls.Add(txtDisplay);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimumSize = new Size(380, 440);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Calculator";
        KeyDown += Form1_KeyDown;
        ResumeLayout(false);
        PerformLayout();
    }

    private void AddDigitButton(string text, int column, int row)
    {
        var button = CreateButton(text);
        button.Tag = text;
        button.Click += DigitButton_Click;
        tableLayoutPanel.Controls.Add(button, column, row);
    }

    private void AddOperatorButton(string op, int column, int row)
    {
        var button = CreateButton(op);
        button.Tag = op;
        button.Click += OperatorButton_Click;
        tableLayoutPanel.Controls.Add(button, column, row);
    }

    private void AddButton(string text, int column, int row, EventHandler onClick)
    {
        var button = CreateButton(text);
        button.Click += onClick;
        tableLayoutPanel.Controls.Add(button, column, row);
    }

    private static Button CreateButton(string text)
    {
        return new Button
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 14F, FontStyle.Regular, GraphicsUnit.Point),
            Margin = new Padding(4),
            Name = "btn" + text,
            Text = text,
            UseVisualStyleBackColor = true
        };
    }
}

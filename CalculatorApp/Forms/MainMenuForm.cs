using System.Drawing;
using System.Windows.Forms;
using CalculatorApp.UI;

namespace CalculatorApp.Forms
{
    public class MainMenuForm : Form
    {
        public MainMenuForm()
        {
            InitializeUi();
        }

        private void InitializeUi()
        {
            Text = "Apedin Market - Main Menu";
            Width = 560;
            Height = 420;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.Background;
            Font = new Font("Segoe UI", 10F);

            var title = new Label
            {
                Text = "Apedin Market",
                AutoSize = false,
                Width = 500,
                Height = 50,
                Location = new Point(20, 20),
                Font = new Font("Segoe UI", 20F, FontStyle.Regular),
                ForeColor = Theme.TextDark,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var subtitle = new Label
            {
                Text = "Simple retail desktop application (offline)",
                AutoSize = false,
                Width = 500,
                Height = 25,
                Location = new Point(20, 75),
                ForeColor = Color.DimGray,
                TextAlign = ContentAlignment.MiddleCenter
            };

            var posButton = CreateMenuButton("POS / Sales", 120, (s, e) => new PosForm().ShowDialog(this));
            var productsButton = CreateMenuButton("Product Management", 180, (s, e) => new ProductManagementForm().ShowDialog(this));
            var stockButton = CreateMenuButton("Stock View", 240, (s, e) => new StockViewForm().ShowDialog(this));
            var historyButton = CreateMenuButton("Sales History", 300, (s, e) => new SalesHistoryForm().ShowDialog(this));

            Controls.Add(title);
            Controls.Add(subtitle);
            Controls.Add(posButton);
            Controls.Add(productsButton);
            Controls.Add(stockButton);
            Controls.Add(historyButton);
        }

        private RoundedButton CreateMenuButton(string text, int top, System.EventHandler click)
        {
            var button = new RoundedButton
            {
                Text = text,
                Width = 260,
                Height = 42,
                Left = 140,
                Top = top
            };
            button.Click += click;
            return button;
        }
    }
}

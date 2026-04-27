using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CalculatorApp.Data;
using CalculatorApp.UI;

namespace CalculatorApp.Forms
{
    public class StockViewForm : Form
    {
        private readonly ProductRepository _repo = new ProductRepository();
        private readonly DataGridView _grid = new DataGridView();
        private readonly NumericUpDown _adjustQuantity = new NumericUpDown();

        public StockViewForm()
        {
            InitializeUi();
            LoadStock();
        }

        private void InitializeUi()
        {
            Text = "Stock Management";
            Width = 900;
            Height = 560;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Background;
            Font = new Font("Segoe UI", 9.5F);

            _grid.SetBounds(20, 20, 840, 420);
            _grid.ReadOnly = true;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoGenerateColumns = true;
            _grid.BackgroundColor = Color.White;

            _adjustQuantity.SetBounds(20, 455, 120, 30);
            _adjustQuantity.Maximum = 100000;

            var increase = new RoundedButton { Text = "Increase", Left = 155, Top = 455, Width = 100, Height = 30 };
            var decrease = new RoundedButton { Text = "Decrease", Left = 265, Top = 455, Width = 100, Height = 30, BackColor = Color.FromArgb(214, 79, 79) };
            var refresh = new RoundedButton { Text = "Refresh", Left = 375, Top = 455, Width = 100, Height = 30 };

            increase.Click += (s, e) => AdjustStock(+1);
            decrease.Click += (s, e) => AdjustStock(-1);
            refresh.Click += (s, e) => LoadStock();

            Controls.Add(_grid);
            Controls.Add(_adjustQuantity);
            Controls.Add(increase);
            Controls.Add(decrease);
            Controls.Add(refresh);
        }

        private void LoadStock()
        {
            var data = _repo.GetAll();
            _grid.DataSource = data.Select(p => new
            {
                p.ProductID,
                p.ProductName,
                p.Barcode,
                p.StockQuantity,
                p.MinimumStockLevel,
                LowStock = p.StockQuantity <= p.MinimumStockLevel ? "YES" : "NO"
            }).ToList();
        }

        private void AdjustStock(int direction)
        {
            if (_grid.CurrentRow == null) return;
            var productId = Convert.ToInt64(_grid.CurrentRow.Cells["ProductID"].Value);
            var all = _repo.GetAll();
            var product = all.FirstOrDefault(p => p.ProductID == productId);
            if (product == null) return;

            var delta = (int)_adjustQuantity.Value;
            if (delta < 0) return;

            var newStock = product.StockQuantity + direction * delta;
            if (newStock < 0)
            {
                MessageBox.Show("Stock cannot be negative.");
                return;
            }

            product.StockQuantity = newStock;
            _repo.Update(product);
            LoadStock();
        }
    }
}

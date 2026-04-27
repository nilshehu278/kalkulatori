using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CalculatorApp.Data;
using CalculatorApp.UI;

namespace CalculatorApp.Forms
{
    public class SalesHistoryForm : Form
    {
        private readonly SalesRepository _repo = new SalesRepository();
        private readonly DataGridView _salesGrid = new DataGridView();
        private readonly DataGridView _itemsGrid = new DataGridView();

        public SalesHistoryForm()
        {
            InitializeUi();
            LoadSales();
        }

        private void InitializeUi()
        {
            Text = "Sales History";
            Width = 980;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Background;
            Font = new Font("Segoe UI", 9.5F);

            _salesGrid.SetBounds(20, 20, 920, 280);
            _salesGrid.ReadOnly = true;
            _salesGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _salesGrid.AutoGenerateColumns = true;
            _salesGrid.BackgroundColor = Color.White;
            _salesGrid.CellClick += (s, e) => LoadSaleItems();

            _itemsGrid.SetBounds(20, 320, 920, 240);
            _itemsGrid.ReadOnly = true;
            _itemsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _itemsGrid.AutoGenerateColumns = true;
            _itemsGrid.BackgroundColor = Color.White;

            Controls.Add(_salesGrid);
            Controls.Add(_itemsGrid);
        }

        private void LoadSales()
        {
            _salesGrid.DataSource = _repo.GetSalesHistory().Select(s => new
            {
                s.SaleID,
                DateTime = s.SaleDate.ToString("yyyy-MM-dd HH:mm:ss"),
                s.TotalAmount,
                s.PaidAmount,
                s.ChangeAmount
            }).ToList();
        }

        private void LoadSaleItems()
        {
            if (_salesGrid.CurrentRow == null) return;
            var saleId = Convert.ToInt64(_salesGrid.CurrentRow.Cells["SaleID"].Value);
            _itemsGrid.DataSource = _repo.GetSaleItems(saleId).Select(i => new
            {
                i.ProductName,
                i.Quantity,
                i.UnitPrice,
                i.Subtotal
            }).ToList();
        }
    }
}

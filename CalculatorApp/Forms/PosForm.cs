using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CalculatorApp.Data;
using CalculatorApp.Models;
using CalculatorApp.UI;

namespace CalculatorApp.Forms
{
    public class PosForm : Form
    {
        private readonly ProductRepository _productRepo = new ProductRepository();
        private readonly SalesRepository _salesRepo = new SalesRepository();
        private readonly FlowLayoutPanel _productsPanel = new FlowLayoutPanel();
        private readonly DataGridView _cartGrid = new DataGridView();
        private readonly TextBox _barcodeInput = new TextBox();
        private readonly TextBox _paidInput = new TextBox();
        private readonly Label _totalLabel = new Label();
        private readonly Label _changeLabel = new Label();
        private readonly List<SaleItem> _cart = new List<SaleItem>();

        public PosForm()
        {
            InitializeUi();
            LoadProductCards();
        }

        private void InitializeUi()
        {
            Text = "POS / Sales";
            Width = 1180;
            Height = 680;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Background;
            Font = new Font("Segoe UI", 9.5F);

            var barcodeLabel = new Label { Text = "Barcode Scan:", Left = 20, Top = 20, Width = 100 };
            _barcodeInput.SetBounds(125, 16, 220, 30);
            _barcodeInput.KeyDown += BarcodeInputOnKeyDown;
            var scanButton = new RoundedButton { Text = "Add by Barcode", Left = 355, Top = 16, Width = 130, Height = 30 };
            scanButton.Click += (s, e) => AddByBarcode();

            _productsPanel.SetBounds(20, 60, 520, 560);
            _productsPanel.AutoScroll = true;
            _productsPanel.WrapContents = true;
            _productsPanel.BackColor = Color.White;

            _cartGrid.SetBounds(560, 60, 590, 410);
            _cartGrid.ReadOnly = true;
            _cartGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _cartGrid.AutoGenerateColumns = true;
            _cartGrid.BackgroundColor = Color.White;

            _totalLabel.SetBounds(560, 480, 260, 28);
            _totalLabel.Font = new Font("Segoe UI", 12F);
            _changeLabel.SetBounds(560, 548, 260, 28);
            _changeLabel.Font = new Font("Segoe UI", 12F);

            var paidLabel = new Label { Text = "Paid Amount:", Left = 560, Top = 515, Width = 90 };
            _paidInput.SetBounds(660, 512, 140, 30);
            _paidInput.TextChanged += (s, e) => RecalculateChange();

            var removeButton = new RoundedButton { Text = "Remove Item", Left = 840, Top = 480, Width = 120, Height = 32, BackColor = Color.FromArgb(214, 79, 79) };
            var clearButton = new RoundedButton { Text = "Clear Cart", Left = 970, Top = 480, Width = 120, Height = 32, BackColor = Color.FromArgb(214, 79, 79) };
            var checkoutButton = new RoundedButton { Text = "Complete Sale", Left = 840, Top = 522, Width = 250, Height = 40 };

            removeButton.Click += (s, e) => RemoveSelectedItem();
            clearButton.Click += (s, e) => { _cart.Clear(); RefreshCart(); };
            checkoutButton.Click += (s, e) => CompleteSale();

            Controls.Add(barcodeLabel);
            Controls.Add(_barcodeInput);
            Controls.Add(scanButton);
            Controls.Add(_productsPanel);
            Controls.Add(_cartGrid);
            Controls.Add(_totalLabel);
            Controls.Add(paidLabel);
            Controls.Add(_paidInput);
            Controls.Add(_changeLabel);
            Controls.Add(removeButton);
            Controls.Add(clearButton);
            Controls.Add(checkoutButton);

            RefreshCart();
        }

        private void LoadProductCards()
        {
            _productsPanel.Controls.Clear();
            foreach (var product in _productRepo.GetAll())
            {
                var button = new RoundedButton
                {
                    Width = 150,
                    Height = 90,
                    Margin = new Padding(10),
                    Text = product.ProductName + Environment.NewLine + "$" + product.Price.ToString("0.00") + Environment.NewLine + "Stock: " + product.StockQuantity,
                    Tag = product,
                    BackColor = product.StockQuantity > 0 ? Theme.MainGreen : Color.Gray
                };

                button.Enabled = product.StockQuantity > 0;
                button.Click += (s, e) => AddToCart((Product)((Control)s).Tag);
                _productsPanel.Controls.Add(button);
            }
        }

        private void BarcodeInputOnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AddByBarcode();
            }
        }

        private void AddByBarcode()
        {
            var barcode = _barcodeInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(barcode)) return;

            var product = _productRepo.GetByBarcode(barcode);
            if (product == null)
            {
                MessageBox.Show("Barcode not found.");
                return;
            }

            AddToCart(product);
            _barcodeInput.Clear();
        }

        private void AddToCart(Product product)
        {
            if (product.StockQuantity <= 0)
            {
                MessageBox.Show("Product out of stock.");
                return;
            }

            var existing = _cart.FirstOrDefault(i => i.ProductID == product.ProductID);
            if (existing == null)
            {
                _cart.Add(new SaleItem
                {
                    ProductID = product.ProductID,
                    ProductName = product.ProductName,
                    Quantity = 1,
                    UnitPrice = product.Price,
                    Subtotal = product.Price
                });
            }
            else
            {
                if (existing.Quantity >= product.StockQuantity)
                {
                    MessageBox.Show("Cannot exceed current stock.");
                    return;
                }

                existing.Quantity += 1;
                existing.Subtotal = existing.Quantity * existing.UnitPrice;
            }

            RefreshCart();
        }

        private void RefreshCart()
        {
            _cartGrid.DataSource = null;
            _cartGrid.DataSource = _cart.Select(x => new { x.ProductID, x.ProductName, x.Quantity, x.UnitPrice, x.Subtotal }).ToList();
            _totalLabel.Text = "Total: $" + _cart.Sum(i => i.Subtotal).ToString("0.00");
            RecalculateChange();
        }

        private void RecalculateChange()
        {
            decimal paid;
            decimal.TryParse(_paidInput.Text, out paid);
            var total = _cart.Sum(i => i.Subtotal);
            var change = paid - total;
            _changeLabel.Text = "Change: $" + (change < 0 ? 0 : change).ToString("0.00");
        }

        private void RemoveSelectedItem()
        {
            if (_cartGrid.CurrentRow == null) return;
            var productId = Convert.ToInt64(_cartGrid.CurrentRow.Cells["ProductID"].Value);
            var item = _cart.FirstOrDefault(c => c.ProductID == productId);
            if (item != null)
            {
                _cart.Remove(item);
                RefreshCart();
            }
        }

        private void CompleteSale()
        {
            try
            {
                var total = _cart.Sum(i => i.Subtotal);
                if (total <= 0) throw new InvalidOperationException("Cart is empty.");

                decimal paid;
                if (!decimal.TryParse(_paidInput.Text, out paid)) throw new InvalidOperationException("Enter paid amount.");
                if (paid < total) throw new InvalidOperationException("Payment is less than total.");

                var change = paid - total;
                var saleId = _salesRepo.CompleteSale(_cart, total, paid, change);

                MessageBox.Show("Sale completed.\nReceipt\nSale ID: " + saleId + "\nTotal: $" + total.ToString("0.00") + "\nPaid: $" + paid.ToString("0.00") + "\nChange: $" + change.ToString("0.00"));

                _cart.Clear();
                _paidInput.Clear();
                RefreshCart();
                LoadProductCards();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sale Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}

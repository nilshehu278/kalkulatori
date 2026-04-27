using System;
using System.Drawing;
using System.Windows.Forms;
using CalculatorApp.Data;
using CalculatorApp.Models;
using CalculatorApp.UI;

namespace CalculatorApp.Forms
{
    public class ProductManagementForm : Form
    {
        private readonly ProductRepository _repo = new ProductRepository();
        private readonly DataGridView _grid = new DataGridView();
        private readonly TextBox _search = new TextBox();
        private readonly TextBox _name = new TextBox();
        private readonly TextBox _category = new TextBox();
        private readonly TextBox _barcode = new TextBox();
        private readonly NumericUpDown _price = new NumericUpDown();
        private readonly NumericUpDown _stock = new NumericUpDown();
        private readonly NumericUpDown _minStock = new NumericUpDown();

        public ProductManagementForm()
        {
            InitializeUi();
            LoadProducts();
        }

        private void InitializeUi()
        {
            Text = "Product Management";
            Width = 980;
            Height = 620;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Theme.Background;
            Font = new Font("Segoe UI", 9.5F);

            var searchLabel = new Label { Text = "Search:", Left = 20, Top = 20, Width = 55 };
            _search.SetBounds(80, 16, 260, 30);
            var searchButton = new RoundedButton { Text = "Find", Left = 350, Top = 16, Width = 90, Height = 30 };
            searchButton.Click += (s, e) => LoadProducts(_search.Text);

            _grid.SetBounds(20, 60, 620, 500);
            _grid.ReadOnly = true;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.MultiSelect = false;
            _grid.AutoGenerateColumns = true;
            _grid.BackgroundColor = Color.White;
            _grid.CellClick += GridOnCellClick;

            var panel = new Panel { Left = 660, Top = 60, Width = 290, Height = 500, BackColor = Color.White };
            AddField(panel, "Name", _name, 20);
            AddField(panel, "Category", _category, 85);
            AddField(panel, "Barcode", _barcode, 150);
            AddField(panel, "Price", _price, 215);
            AddField(panel, "Stock", _stock, 280);
            AddField(panel, "Min Stock", _minStock, 345);

            _price.DecimalPlaces = 2;
            _price.Maximum = 1000000;
            _stock.Maximum = 1000000;
            _minStock.Maximum = 1000000;

            var addButton = new RoundedButton { Text = "Add", Left = 15, Top = 430, Width = 80, Height = 36 };
            var updateButton = new RoundedButton { Text = "Update", Left = 105, Top = 430, Width = 80, Height = 36 };
            var deleteButton = new RoundedButton { Text = "Delete", Left = 195, Top = 430, Width = 80, Height = 36, BackColor = Color.FromArgb(214, 79, 79) };

            addButton.Click += (s, e) => SaveProduct(false);
            updateButton.Click += (s, e) => SaveProduct(true);
            deleteButton.Click += DeleteSelected;

            panel.Controls.Add(addButton);
            panel.Controls.Add(updateButton);
            panel.Controls.Add(deleteButton);

            Controls.Add(searchLabel);
            Controls.Add(_search);
            Controls.Add(searchButton);
            Controls.Add(_grid);
            Controls.Add(panel);
        }

        private void AddField(Control parent, string label, Control input, int top)
        {
            parent.Controls.Add(new Label { Text = label, Left = 15, Top = top, Width = 250 });
            input.Left = 15;
            input.Top = top + 24;
            input.Width = 260;
            parent.Controls.Add(input);
        }

        private void LoadProducts(string search = null)
        {
            _grid.DataSource = _repo.GetAll(search);
        }

        private void GridOnCellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_grid.CurrentRow == null) return;
            var row = _grid.CurrentRow;
            _name.Text = Convert.ToString(row.Cells["ProductName"].Value);
            _category.Text = Convert.ToString(row.Cells["Category"].Value);
            _barcode.Text = Convert.ToString(row.Cells["Barcode"].Value);
            _price.Value = Convert.ToDecimal(row.Cells["Price"].Value);
            _stock.Value = Convert.ToDecimal(row.Cells["StockQuantity"].Value);
            _minStock.Value = Convert.ToDecimal(row.Cells["MinimumStockLevel"].Value);
        }

        private void SaveProduct(bool isUpdate)
        {
            try
            {
                var product = new Product
                {
                    ProductName = _name.Text,
                    Category = _category.Text,
                    Barcode = _barcode.Text,
                    Price = _price.Value,
                    StockQuantity = (int)_stock.Value,
                    MinimumStockLevel = (int)_minStock.Value
                };

                if (isUpdate)
                {
                    if (_grid.CurrentRow == null) throw new InvalidOperationException("Select a product to update.");
                    product.ProductID = Convert.ToInt64(_grid.CurrentRow.Cells["ProductID"].Value);
                    _repo.Update(product);
                }
                else
                {
                    _repo.Add(product);
                }

                LoadProducts(_search.Text);
                MessageBox.Show("Saved successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Validation/Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteSelected(object sender, EventArgs e)
        {
            if (_grid.CurrentRow == null) return;
            var id = Convert.ToInt64(_grid.CurrentRow.Cells["ProductID"].Value);
            _repo.Delete(id);
            LoadProducts(_search.Text);
        }
    }
}

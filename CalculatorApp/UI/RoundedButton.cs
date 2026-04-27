using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CalculatorApp.UI
{
    public class RoundedButton : Button
    {
        public int CornerRadius { get; set; } = 10;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            BackColor = Theme.MainGreen;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            Cursor = Cursors.Hand;
        }

        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            using (var path = new GraphicsPath())
            {
                path.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
                path.AddArc(Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
                path.AddArc(Width - CornerRadius, Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                path.AddArc(0, Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                path.CloseFigure();
                Region = new Region(path);
            }
        }
    }
}

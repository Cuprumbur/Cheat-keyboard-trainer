using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheatingKeyboardSimulator
{
    public class FormScreenShot : Form
    {

        private Rectangle SelectedRectangle;
        public Bitmap GetBitmap { get; private set; }

        public FormScreenShot()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);

            new Button { Text = "Close me", Parent = this , BackColor=Color.Orchid, Dock=DockStyle.Top}.Click += (o, e) => Application.Exit();
            this.FormBorderStyle = FormBorderStyle.None;
            TopMost = true;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Maximized;

            BackgroundImage = Shoot();
        }

        private Bitmap Shoot()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (var gr = Graphics.FromImage(bmp))
                gr.CopyFromScreen(0, 0, 0, 0, new Size(bmp.Width, bmp.Height));
            return bmp;
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            SelectedRectangle.Location = e.Location;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (SelectedRectangle.Width > 0 && SelectedRectangle.Height > 0)
            {
                using (var bmp = new Bitmap(SelectedRectangle.Width, SelectedRectangle.Height))
                {
                    var gr = Graphics.FromImage(bmp);
                    gr.DrawImage(BackgroundImage, -SelectedRectangle.Left, -SelectedRectangle.Top);

                    // Сохранение копии
                    GetBitmap = (Bitmap)bmp.Clone();
                    DialogResult = DialogResult.OK;
                }
                SelectedRectangle.Size = Size.Empty;
                Invalidate();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            var newSize = new Size(e.X - SelectedRectangle.Left, e.Y - SelectedRectangle.Top);

            if (MouseButtons == MouseButtons.Left)
                if (newSize.Width > 5 && newSize.Height > 5)
                {
                    SelectedRectangle.Size = newSize;
                    Invalidate();
                }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var r = new Region(ClientRectangle);
            r.Exclude(SelectedRectangle);
            using (var brush = new SolidBrush(Color.FromArgb(20, 0, 0, 0)))
                e.Graphics.FillRegion(brush, r);
        }


    }
}

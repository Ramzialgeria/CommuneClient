using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CommuneClient
{
    public partial class IpServerForm : Form
    {
        private readonly Color BgMain = Color.FromArgb(226, 232, 240);
        private readonly Color PanelBg = Color.FromArgb(255, 255, 255);
        private readonly Color BorderColor = Color.FromArgb(203, 213, 225);

        private readonly Color Blue = Color.FromArgb(0, 123, 255);
        private readonly Color BlueDark = Color.FromArgb(0, 82, 204);

        private readonly Color TextMain = Color.FromArgb(30, 41, 59);
        private readonly Color TextMuted = Color.FromArgb(71, 85, 105);

        public const int WM_NCLBUTTONDOWN = 0x00A1;
        public const int HT_CAPTION = 0x0002;

        [DllImport("user32.dll")]
        public static extern int SendMessage(
            IntPtr hWnd,
            int Msg,
            int wParam,
            int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public string IpAdress { get; private set; }

        public string Prefill
        {
            get { return txtIpAddress.Text; }
            set { txtIpAddress.Text = value ?? ""; }
        }

        public IpServerForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(600, 400);
            BackColor = BgMain;
            DoubleBuffered = true;

            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;

            BuildUI();
        }

        private void BuildUI()
        {
            buttonClose.BringToFront();

            card.Paint += PaintCard;

            txtIpAddress.Focus();
        }

        private void PaintCard(object sender, PaintEventArgs e)
        {
            Panel card = sender as Panel;

            if (card == null)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = RoundedRectangle(
                new Rectangle(
                    0,
                    0,
                    card.Width - 1,
                    card.Height - 1),
                20))
            {
                using (SolidBrush brush = new SolidBrush(PanelBg))
                {
                    e.Graphics.FillPath(brush, path);
                }

                using (Pen pen = new Pen(BorderColor, 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private GraphicsPath RoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            int diameter = radius * 2;

            path.AddArc(
                rect.X,
                rect.Y,
                diameter,
                diameter,
                180,
                90);

            path.AddArc(
                rect.Right - diameter,
                rect.Y,
                diameter,
                diameter,
                270,
                90);

            path.AddArc(
                rect.Right - diameter,
                rect.Bottom - diameter,
                diameter,
                diameter,
                0,
                90);

            path.AddArc(
                rect.X,
                rect.Bottom - diameter,
                diameter,
                diameter,
                90,
                90);

            path.CloseFigure();

            return path;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            Rectangle rect = ClientRectangle;

            using (LinearGradientBrush brush =
                new LinearGradientBrush(
                    rect,
                    Color.FromArgb(248, 250, 252),
                    Color.FromArgb(226, 232, 240),
                    LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            using (Pen pen =
                new Pen(Color.FromArgb(218, 225, 233)))
            {
                const int gridSize = 40;

                for (int x = 0; x < Width; x += gridSize)
                {
                    e.Graphics.DrawLine(
                        pen,
                        x,
                        0,
                        x,
                        Height);
                }

                for (int y = 0; y < Height; y += gridSize)
                {
                    e.Graphics.DrawLine(
                        pen,
                        0,
                        y,
                        Width,
                        y);
                }
            }
        }


        private void buttonClose_Click(object sender, EventArgs e)
        {
            IpAdress = "";
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private async void saveButton_Click_1(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            string ip = ServerConfig.Normalize(txtIpAddress.Text);

            if (ip == null)
            {
                MessageBox.Show(
                    "الرجاء إدخال عنوان IP أو اسم الخادم بشكل صحيح.\nمثال: 192.168.1.10",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtIpAddress.Focus();
                return;
            }

            // Check that the CommuneServer really answers at this address
            btn.Enabled = false;
            string oldText = btn.Text;
            btn.Text = "...";

            bool reachable = await ServerConfig.PingAsync(ip);

            btn.Text = oldText;
            btn.Enabled = true;

            if (!reachable)
            {
                var answer = MessageBox.Show(
                    "تعذر الاتصال بالخادم على هذا العنوان.\nهل تريد حفظه على أي حال؟",
                    "تنبيه",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2,
                    MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign);

                if (answer != DialogResult.Yes)
                {
                    txtIpAddress.Focus();
                    return;
                }
            }

            IpAdress = ip;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void IpServerForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left &&
                WindowState != FormWindowState.Maximized)
            {
                ReleaseCapture();

                SendMessage(
                    Handle,
                    WM_NCLBUTTONDOWN,
                    HT_CAPTION,
                    0);
            }
        }
    }
}
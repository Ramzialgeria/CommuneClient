using CommuneClient.Api;
using CommuneClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommuneClient
{
    public partial class LoginForm : Form
    {
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

        private readonly Color BgMain = Color.FromArgb(226, 232, 240);
        private readonly Color PanelBg = Color.FromArgb(255, 255, 255);
        private readonly Color BorderColor = Color.FromArgb(203, 213, 225);

        private readonly Color Green = Color.FromArgb(0, 166, 81);
        private readonly Color GreenDark = Color.FromArgb(0, 120, 58);

        private readonly Color Blue = Color.FromArgb(0, 123, 255);
        private readonly Color BlueDark = Color.FromArgb(0, 82, 204);

        private readonly Color TextMain = Color.FromArgb(30, 41, 59);
        private readonly Color TextMuted = Color.FromArgb(71, 85, 105);

        Button loginButton;
        TextBox txtUsername;
        TextBox txtPassword;
        Services _services;

        string CommuneServerUrl;
        Sizes _sizes;

        public LoginForm(string serverUrl)
        {
            InitializeComponent();

            CommuneServerUrl = serverUrl;

            int width = Screen.PrimaryScreen.Bounds.Width;
            if (width <= 800)
            {
                _sizes = new Sizes
                {
                    width = 800,
                    height = 600,
                    widthLogin = 320,
                    heightLogin = 420,
                    fontSize = 14,
                };
            }
            else
            {
                _sizes = new Sizes
                {
                    width = 1200,
                    height = 750,
                    widthLogin = 420,
                    heightLogin = 520,
                    fontSize = 16,
                };
            }

            FormBorderStyle = FormBorderStyle.None;
            Size = new Size(_sizes.width, _sizes.height);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(10, 13, 18);
            DoubleBuffered = true;

            BuildLoginUI();
            CreateCloseButton();
        }

        private void EnableFormMove(Control control)
        {
            control.MouseDown += FormMove_MouseDown;

            foreach (Control child in control.Controls)
            {
                EnableFormMove(child);
            }
        }

        private void FormMove_MouseDown(
    object sender,
    MouseEventArgs e)
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

        private void BuildLoginUI()
        {
            var main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(80)
            };

            main.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            main.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 50));

            Controls.Add(main);

            var header = CreateHeaderPanel();
            var login = CreateLoginPanel();

            // RTL means header visually appears on right
            main.Controls.Add(login, 0, 0);
            main.Controls.Add(header, 1, 0);
            EnableFormMove(header);
        }

        private Panel CreateHeaderPanel()
        {
            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                BackColor = Color.Transparent
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 75));

            panel.Controls.Add(layout);

            var logo = new PictureBox
            {
                Size = new Size(130, 130),
                SizeMode = PictureBoxSizeMode.Zoom,
                Anchor = AnchorStyles.None,
                Image = Image.FromFile(
                    Path.Combine(Application.StartupPath, "img", "logo.png"))
            };

            layout.Controls.Add(logo, 0, 1);

            var title = new Label
            {
                Text = "الجمهورية الجزائرية الديمقراطية الشعبية\n"+"السلطة الوطنية المستقلة للانتخابات",
                ForeColor = TextMain,
                Font = new Font("Cairo", _sizes.fontSize, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleLeft
            };

            layout.Controls.Add(title, 0, 2);

            var portal = new Label
            {
                Text = "تطبيقة انتخابات أعضاء المجالس الشعبية\n البلدية والولائية ليوم 28 نوفمبر 2026",
                ForeColor = TextMuted,
                Font = new Font("Cairo", _sizes.fontSize - 2, FontStyle.Bold),
                AutoSize = true,
                Anchor = AnchorStyles.None,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(0, 20, 0, 0)
            };

            layout.Controls.Add(portal, 0, 3);
            
            return panel;
        }

        private Panel CreateLoginPanel()
        {
            var card = new Panel
            {
                Width = _sizes.widthLogin,
                Height = _sizes.heightLogin,
                Anchor = AnchorStyles.None,
                BackColor = PanelBg,
                Padding = new Padding(40)
            };

            card.Paint += LoginCard_Paint;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                BackColor = Color.Transparent
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 110));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

            card.Controls.Add(layout);

            // Icon
            var icon = new Panel
            {
                Size = new Size(100, 100),
                Anchor = AnchorStyles.None
            };

            icon.Paint += LoginIcon_Paint;

            layout.Controls.Add(icon, 0, 0);

            // Title
            var title = new Label
            {
                Text = "تسجيل الدخول",
                Dock = DockStyle.Fill,
                ForeColor = TextMain,
                Font = new Font("Cairo", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            layout.Controls.Add(title, 0, 1);

            // Username
            var username = CreateInput(
                "اسم المستخدم",
                out txtUsername);

            layout.Controls.Add(username, 0, 2);

            // Password
            var password = CreateInput(
                "كلمة المرور",
                out txtPassword);

            txtPassword.UseSystemPasswordChar = true;

            layout.Controls.Add(password, 0, 3);

            // Login button
            loginButton = new Button
            {
                Text = "دخول ←",
                Dock = DockStyle.Fill,
                FlatStyle = FlatStyle.Flat,
                BackColor = BlueDark,
                ForeColor = Color.White,
                Font = new Font("Cairo", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            loginButton.FlatAppearance.BorderColor = Blue;
            loginButton.FlatAppearance.BorderSize = 1;

            loginButton.Click += LoginButton_Click;

            layout.Controls.Add(loginButton, 0, 4);

            return card;
        }

        private Panel CreateInput(
    string labelText,
    out TextBox textBox)
        {
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            var label = new Label
            {
                Text = labelText,
                Dock = DockStyle.Top,
                Height = 28,
                ForeColor = TextMuted,
                Font = new Font("Cairo", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            textBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 38,
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = TextMain,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Cairo", 11),
                RightToLeft = RightToLeft.Yes
            };

            container.Controls.Add(textBox);
            container.Controls.Add(label);

            return container;
        }

        private Button CreateCloseButton()
        {
            Button button = new Button
            {
                Text = "×",
                Size = new Size(45, 45),
                Location = new Point(20, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = TextMuted,
                Font = new Font("Segoe UI", 20, FontStyle.Regular),
                Cursor = Cursors.Hand,
                TabStop = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(226, 232, 240);
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(203, 213, 225);

            button.Click += delegate
            {
                Close();
            };

            Controls.Add(button);
            button.BringToFront();

            return button;
        }

        private void LoginCard_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = RoundedRectangle(
                new Rectangle(0, 0, panel.Width - 1, panel.Height - 1),
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

        private GraphicsPath RoundedRectangle(
    Rectangle rect,
    int radius)
        {
            var path = new GraphicsPath();

            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y,
                diameter, diameter, 180, 90);

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

        private void LoginIcon_Paint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Circle background
            using (LinearGradientBrush brush = new LinearGradientBrush(
                panel.ClientRectangle,
                Color.FromArgb(24, 32, 42),
                Color.FromArgb(14, 19, 26),
                LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillEllipse(
                    brush,
                    2,
                    2,
                    panel.Width - 4,
                    panel.Height - 4);
            }

            using (Pen border = new Pen(BorderColor, 1))
            {
                e.Graphics.DrawEllipse(
                    border,
                    2,
                    2,
                    panel.Width - 4,
                    panel.Height - 4);
            }

            using (Pen iconPen = new Pen(Blue, 4))
            {
                // Head
                e.Graphics.DrawEllipse(
                    iconPen,
                    38,
                    18,
                    24,
                    24);

                // Body
                e.Graphics.DrawArc(
                    iconPen,
                    22,
                    48,
                    56,
                    45,
                    180,
                    180);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            Rectangle rect = ClientRectangle;

            using (LinearGradientBrush brush = new LinearGradientBrush(
                rect,
                Color.FromArgb(248, 250, 252),
                Color.FromArgb(226, 232, 240),
                LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, rect);
            }

            // Light grid
            using (Pen pen = new Pen(Color.FromArgb(218, 225, 233)))
            {
                const int gridSize = 40;

                for (int x = 0; x < Width; x += gridSize)
                {
                    e.Graphics.DrawLine(
                        pen,
                        x, 0,
                        x, Height);
                }

                for (int y = 0; y < Height; y += gridSize)
                {
                    e.Graphics.DrawLine(
                        pen,
                        0, y,
                        Width, y);
                }
            }
        }

        private async void LoginButton_Click( object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show(
                    "الرجاء إدخال اسم المستخدم",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show(
                    "الرجاء إدخال كلمة المرور",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPassword.Focus();
                return;
            }

            try
            {
                loginButton.Enabled = false;
                _services = new Services(CommuneServerUrl);
                LoginResponse result =
                    await _services.LoginAsync(
                        username,
                        password);

                if (result == null ||
                    string.IsNullOrWhiteSpace(result.accessToken))
                {
                    MessageBox.Show(
                        "تعذر تسجيل الدخول.",
                        "خطأ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                // Login successful
                string token = result.accessToken;
                string userName = result.userName;
                string role = result.role;
                string codeStructure = result.codeStructure;


                // Open your next form here
                MainForm mainForm = new MainForm(_services,
                                                result.userName,
                                                result.role,
                                                result.codeStructure);
                Hide();
                mainForm.ShowDialog();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                loginButton.Enabled = true;
            }
        }
    }

    public class Sizes
    {
        public int width { get; set; }
        public int height { get; set; }
        public int widthLogin { get; set; }
        public int heightLogin { get; set; }
        public int fontSize { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommuneClient.Api;
using CommuneClient.DTOs.Models;
using CommuneClient.Models;
using CommuneClient.Models.DTOs;

namespace CommuneClient
{
    public partial class MainForm : Form
    {
        private readonly Services _services;
        private readonly string _userName;
        private readonly string _role;
        private readonly string _codeStructure;
        private string _activationType;
        private string _activationCode;

        private readonly Color BgMain =
            Color.FromArgb(226, 232, 240);

        private readonly Color PanelBg =
            Color.White;

        private readonly Color BorderColor =
            Color.FromArgb(203, 213, 225);

        private readonly Color TextMain =
            Color.FromArgb(30, 41, 59);

        private readonly Color TextMuted =
            Color.FromArgb(71, 85, 105);

        private readonly Color Green =
            Color.FromArgb(0, 166, 81);

        private readonly Color GreenDark =
            Color.FromArgb(0, 120, 58);

        private readonly Color Blue =
            Color.FromArgb(0, 176, 255);

        private readonly Color BlueDark =
            Color.FromArgb(0, 82, 204);

      
        private Panel contentPanel;
        public MainForm(
            Services services,
            string userName,
            string role,
            string codeStructure)
        {
            _services = services;
            _userName = userName;
            _role = role;
            _codeStructure = codeStructure;

            InitializeForm();
            BuildUI();
        }

        private void InitializeForm()
        {
            FormBorderStyle = FormBorderStyle.None;

            WindowState = FormWindowState.Maximized;

            StartPosition = FormStartPosition.CenterScreen;

            BackColor = BgMain;

            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;

            DoubleBuffered = true;
        }

        private void BuildUI()
        {
            CreateMinimizeButton();
            CreateCloseButton();

            TableLayoutPanel mainLayout =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 4,
                    BackColor = Color.Transparent,
                    Padding = new Padding(50, 30, 50, 40)
                };

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 115));

            mainLayout.RowStyles.Add(
    new RowStyle(SizeType.Absolute, 90));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 35));

            Controls.Add(mainLayout);

            // Header
            Panel header = CreateHeader();

            mainLayout.Controls.Add(
                header,
                0,
                0);

            // Portal title
            Label portalTitle =
                new Label
                {
                    Text = "تطبيقة انتخابات أعضاء المجالس الشعبية\n البلدية والولائية ليوم 28 نوفمبر 2026",
                    Dock = DockStyle.Fill,
                    ForeColor = TextMain,
                    Font = new Font(
                        "Cairo",
                        20,
                        FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter
                };

            mainLayout.Controls.Add(
                portalTitle,
                0,
                1);

            // Main content
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            mainLayout.Controls.Add(
                contentPanel,
                0,
                2);

            //ShowPortal();
            LoadActivationAndShowPortal();

            // User
            Label userLabel =
                new Label
                {
                    Text = "المستخدم: " + _userName,
                    Dock = DockStyle.Fill,
                    ForeColor = TextMuted,
                    Font = new Font(
                        "Cairo",
                        9),
                    TextAlign =
                        ContentAlignment.MiddleCenter
                };

            mainLayout.Controls.Add(
                userLabel,
                0,
                3);

           
        }

        private void StyleWindowButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = Color.Transparent;
            button.ForeColor = TextMuted;
            button.Cursor = Cursors.Hand;
            button.TabStop = false;

            button.MouseEnter += delegate
            {
                button.BackColor = Color.FromArgb(203, 213, 225);
                button.ForeColor = TextMain;
            };

            button.MouseLeave += delegate
            {
                button.BackColor = Color.Transparent;
                button.ForeColor = TextMuted;
            };
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

            button.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(226, 232, 240);

            button.FlatAppearance.MouseDownBackColor =
                Color.FromArgb(203, 213, 225);

            StyleWindowButton(button);

            button.Click += delegate
            {
                Close();
            };

            Controls.Add(button);
            button.BringToFront();

            return button;
        }

        private Button CreateMinimizeButton()
        {
            Button button = new Button
            {
                Text = "—",
                Size = new Size(45, 45),

                // Close button is at 20, so put minimize next to it
                Location = new Point(70, 20),

                Anchor =
                    AnchorStyles.Top |
                    AnchorStyles.Right,

                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = TextMuted,

                Font = new Font(
                    "Segoe UI",
                    16,
                    FontStyle.Regular),

                Cursor = Cursors.Hand,
                TabStop = false
            };

            button.FlatAppearance.BorderSize = 0;

            button.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(226, 232, 240);

            button.FlatAppearance.MouseDownBackColor =
                Color.FromArgb(203, 213, 225);

            StyleWindowButton(button);

            button.Click += delegate
            {
                WindowState = FormWindowState.Minimized;
            };

            Controls.Add(button);
            button.BringToFront();

            return button;
        }

        private void ShowPortal()
        {
            contentPanel.SuspendLayout();

            try
            {
                contentPanel.Controls.Clear();

                if (_activationType == "APW")
                {
                    BuildApwCard();
                }
                else if (_activationType == "APC")
                {
                    BuildApcCard();
                }
            }
            finally
            {
                contentPanel.ResumeLayout();
            }
        }

        private async Task LoadActivationAsync()
        {
            ActivationDto activation =
                await _services.GetActivationAsync();

            _activationType =
                activation.ActivationType;

            _activationCode =
                activation.CodeCommune;
        }

        private async void LoadActivationAndShowPortal()
        {
            try
            {
                await LoadActivationAsync();

                ShowPortal();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Close();
            }
        }

        private Panel CreateHeader()
        {
            Panel header = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // Left logo
            PictureBox leftLogo = CreateHeaderLogo();

            // Right logo
            PictureBox rightLogo = CreateHeaderLogo();

            // Center title
            Label title = new Label
            {
                Text =
                    "الجمهورية الجزائرية الديمقراطية الشعبية\n" +
                    "السلطة الوطنية المستقلة للانتخابات",

                ForeColor = TextMain,

                Font = new Font(
                    "Cairo",
                    15,
                    FontStyle.Bold),

                TextAlign = ContentAlignment.MiddleCenter,

                BackColor = Color.Transparent
            };

            header.Controls.Add(leftLogo);
            header.Controls.Add(title);
            header.Controls.Add(rightLogo);

            Action centerHeader = delegate
            {
                int logoSize = 90;
                int spacing = 30;

                // Center title in the entire header
                title.Size = new Size(
                    650,
                    header.ClientSize.Height);

                title.Location = new Point(
                    (header.ClientSize.Width - title.Width) / 2,
                    0);

                // Left logo
                leftLogo.Size = new Size(
                    logoSize,
                    logoSize);

                leftLogo.Location = new Point(
                    title.Left - logoSize - spacing,
                    (header.ClientSize.Height - logoSize) / 2);

                // Right logo
                rightLogo.Size = new Size(
                    logoSize,
                    logoSize);

                rightLogo.Location = new Point(
                    title.Right + spacing,
                    (header.ClientSize.Height - logoSize) / 2);
            };

            header.Resize += delegate
            {
                centerHeader();
            };

            centerHeader();
            return header;
        }

        private PictureBox CreateHeaderLogo()
        {
            PictureBox logo = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            string logoPath = Path.Combine(
                Application.StartupPath,
                "img",
                "logo.png");

            if (File.Exists(logoPath))
            {
                logo.Image = Image.FromFile(logoPath);
            }

            return logo;
        }

        private void BuildApcCard()
        {
            Panel apcCard =
                CreatePortalCard(
                    "APC",
                    "المجلس الشعبي البلدي",
                    Green,
                    GreenDark,
                    true);

            int cardWidth = 390;
            int cardHeight = 330;

            apcCard.Size =
                new Size(
                    cardWidth,
                    cardHeight);

            apcCard.Location =
                new Point(
                    (contentPanel.ClientSize.Width -
                        cardWidth) / 2,

                    (contentPanel.ClientSize.Height -
                        cardHeight) / 2);

            contentPanel.Controls.Add(apcCard);

            contentPanel.Resize += delegate
            {
                apcCard.Location =
                    new Point(
                        (contentPanel.ClientSize.Width -
                            cardWidth) / 2,

                        (contentPanel.ClientSize.Height -
                            cardHeight) / 2);
            };
        }

        private void BuildApwCard()
        {
            Panel apwCard =
                CreatePortalCard(
                    "APW",
                    "المجلس الشعبي الولائي",
                    Blue,
                    BlueDark,
                    false);

            int cardWidth = 390;
            int cardHeight = 330;

            apwCard.Size =
                new Size(
                    cardWidth,
                    cardHeight);

            apwCard.Location =
                new Point(
                    (contentPanel.ClientSize.Width -
                        cardWidth) / 2,

                    (contentPanel.ClientSize.Height -
                        cardHeight) / 2);

            contentPanel.Controls.Add(apwCard);

            contentPanel.Resize += delegate
            {
                apwCard.Location =
                    new Point(
                        (contentPanel.ClientSize.Width -
                            cardWidth) / 2,

                        (contentPanel.ClientSize.Height -
                            cardHeight) / 2);
            };
        }

        private void BuildCards()
        {
            contentPanel.SuspendLayout();

            try
            {
                contentPanel.Controls.Clear();

                Panel apcCard =
                    CreatePortalCard(
                        "APC",
                        "المجلس الشعبي البلدي",
                        Green,
                        GreenDark,
                        true);

                Panel apwCard =
                    CreatePortalCard(
                        "APW",
                        "المجلس الشعبي الولائي",
                        Blue,
                        BlueDark,
                        false);

                int cardWidth = 390;
                int cardHeight = 330;
                int spacing = 60;

                apcCard.Size =
                    new Size(cardWidth, cardHeight);

                apwCard.Size =
                    new Size(cardWidth, cardHeight);

                int totalWidth =
                    (cardWidth * 2) + spacing;

                int startX =
                    (contentPanel.ClientSize.Width - totalWidth) / 2;

                int startY =
                    (contentPanel.ClientSize.Height - cardHeight) / 2;

                if (startX < 0)
                    startX = 0;

                if (startY < 0)
                    startY = 0;

                apcCard.Location =
                    new Point(startX, startY);

                apwCard.Location =
                    new Point(
                        startX + cardWidth + spacing,
                        startY);

                contentPanel.Controls.Add(apcCard);
                contentPanel.Controls.Add(apwCard);
            }
            finally
            {
                contentPanel.ResumeLayout();
            }
        }

        private Panel CreatePortalCard(
    string title,
    string subtitle,
    Color color,
    Color darkColor,
    bool isApc)
        {
            Panel card =
                new Panel
                {
                    BackColor = PanelBg,
                    Cursor = Cursors.Hand
                };

            card.Paint += delegate (
                object sender,
                PaintEventArgs e)
            {
                PaintPortalCard(
                    sender,
                    e,
                    color);
            };

            // Icon
            Panel icon =
                new Panel
                {
                    Size = new Size(110, 110),
                    Location = new Point(
                        (390 - 110) / 2,
                        30),
                    BackColor = Color.Transparent
                };

            icon.Paint += delegate (
                object sender,
                PaintEventArgs e)
            {
                if (isApc)
                {
                    DrawApcIcon(
                        e.Graphics,
                        icon.ClientRectangle,
                        color);
                }
                else
                {
                    DrawApwIcon(
                        e.Graphics,
                        icon.ClientRectangle,
                        color);
                }
            };

            card.Controls.Add(icon);

            // APC / APW
            Label titleLabel =
                new Label
                {
                    Text = title,
                    Location = new Point(
                        0,
                        145),
                    Width = 390,
                    Height = 45,
                    ForeColor = color,
                    Font = new Font(
                        "Segoe UI",
                        24,
                        FontStyle.Bold),
                    TextAlign =
                        ContentAlignment.MiddleCenter
                };

            card.Controls.Add(titleLabel);

            // Subtitle
            Label subtitleLabel =
                new Label
                {
                    Text = subtitle,
                    Location = new Point(
                        0,
                        195),
                    Width = 390,
                    Height = 40,
                    ForeColor = TextMuted,
                    Font = new Font(
                        "Cairo",
                        11,
                        FontStyle.Regular),
                    TextAlign =
                        ContentAlignment.MiddleCenter
                };

            card.Controls.Add(subtitleLabel);

            // Button
            Button button =
                new Button
                {
                    Text = "اختر  ←",
                    Location = new Point(
                        105,
                        250),
                    Size = new Size(
                        180,
                        45),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = darkColor,
                    ForeColor = Color.White,
                    Font = new Font(
                        "Cairo",
                        10,
                        FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

            button.FlatAppearance.BorderSize = 0;

            card.Controls.Add(button);

            // Make whole card clickable
            EventHandler clickHandler =
                delegate
                {
                    if (isApc)
                    {
                        OpenApc();
                    }
                    else
                    {
                        OpenApw();
                    }
                };

            card.Click += clickHandler;
            titleLabel.Click += clickHandler;
            subtitleLabel.Click += clickHandler;
            icon.Click += clickHandler;

            button.Click += clickHandler;

            return card;
        }

        private void PaintPortalCard(
     object sender,
     PaintEventArgs e,
     Color accentColor)
        {
            Panel card = sender as Panel;

            if (card == null)
                return;

            e.Graphics.SmoothingMode =
                SmoothingMode.AntiAlias;

            Rectangle rect =
                new Rectangle(
                    0,
                    0,
                    card.Width - 1,
                    card.Height - 1);

            // Background
            using (SolidBrush brush =
                new SolidBrush(PanelBg))
            {
                e.Graphics.FillRectangle(
                    brush,
                    rect);
            }

            // Border
            using (Pen pen =
                new Pen(BorderColor, 1))
            {
                e.Graphics.DrawRectangle(
                    pen,
                    rect);
            }

            // Accent line at the top
            using (Pen pen =
                new Pen(accentColor, 4))
            {
                e.Graphics.DrawLine(
                    pen,
                    0,
                    2,
                    card.Width - 1,
                    2);
            }
        }

        private void DrawApcIcon(
    Graphics g,
    Rectangle rect,
    Color color)
        {
            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            float cx =
                rect.Width / 2f;

            float cy =
                rect.Height / 2f;

            PointF[] hexagon =
            {
                new PointF(cx, 10),
                new PointF(rect.Width - 15, 30),
                new PointF(rect.Width - 15, 75),
                new PointF(cx, 100),
                new PointF(15, 75),
                new PointF(15, 30)
            };

            using (Pen pen =
                new Pen(color, 3))
            {
                g.DrawPolygon(
                    pen,
                    hexagon);
            }

            using (SolidBrush brush =
                new SolidBrush(color))
            {
                g.FillEllipse(
                    brush,
                    cx - 14,
                    cy - 14,
                    28,
                    28);
            }

            using (Pen pen =
                new Pen(Color.White, 3))
            {
                pen.StartCap =
                    LineCap.Round;

                pen.EndCap =
                    LineCap.Round;

                g.DrawLine(
                    pen,
                    cx,
                    20,
                    cx,
                    40);

                g.DrawLine(
                    pen,
                    cx - 15,
                    42,
                    cx - 5,
                    48);

                g.DrawLine(
                    pen,
                    cx + 15,
                    42,
                    cx + 5,
                    48);
            }
        }

        private void DrawApwIcon(
    Graphics g,
    Rectangle rect,
    Color color)
        {
            g.SmoothingMode =
                SmoothingMode.AntiAlias;

            DrawHex(
                g,
                new PointF(
                    rect.Width / 2f,
                    25),
                16,
                color,
                true);

            DrawHex(
                g,
                new PointF(
                    rect.Width / 2f,
                    65),
                16,
                color,
                true);

            DrawHex(
                g,
                new PointF(
                    28,
                    45),
                16,
                color,
                false);

            DrawHex(
                g,
                new PointF(
                    rect.Width - 28,
                    45),
                16,
                color,
                false);
        }

        private void DrawHex(
            Graphics g,
            PointF center,
            float radius,
            Color color,
            bool filled)
        {
            PointF[] points =
                new PointF[6];

            for (int i = 0; i < 6; i++)
            {
                double angle =
                    Math.PI / 3 * i -
                    Math.PI / 6;

                points[i] =
                    new PointF(
                        center.X +
                        radius *
                        (float)Math.Cos(angle),

                        center.Y +
                        radius *
                        (float)Math.Sin(angle));
            }

            if (filled)
            {
                using (SolidBrush brush =
                    new SolidBrush(color))
                {
                    g.FillPolygon(
                        brush,
                        points);
                }
            }
            else
            {
                using (Pen pen =
                    new Pen(color, 2))
                {
                    g.DrawPolygon(
                        pen,
                        points);
                }
            }
        }

        private async void OpenApc()
        {
            await ShowApcCentres();
        }
        private async Task ShowApcCentres()
        {
            contentPanel.Controls.Clear();

            Panel page = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            contentPanel.Controls.Add(page);

            Button backButton = new Button
            {
                Text = "رجوع",
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = TextMain,
                Font = new Font("Cairo", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            backButton.FlatAppearance.BorderColor = BorderColor;
            backButton.FlatAppearance.BorderSize = 1;

            backButton.MouseEnter += delegate
            {
                backButton.BackColor =
                    Color.FromArgb(226, 232, 240);
            };

            backButton.MouseLeave += delegate
            {
                backButton.BackColor = Color.White;
            };

            backButton.Click += delegate
            {
                ShowPortal();
            };

            page.Controls.Add(backButton);

            Label title = new Label
            {
                Text = "قائمة مراكز البلديات",
                Size = new Size(700, 50),
                ForeColor = GreenDark,
                Font = new Font("Cairo", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            page.Controls.Add(title);

            DataGridView grid = CreateDataGrid();
            EnableGridHover(grid);
            grid.Size = new Size(980, 450);

            grid.Columns.Add("CodeCentre", "رمز المركز");
            grid.Columns.Add("NomCentre", "اسم المركز");
            grid.Columns.Add("Id", "Id");
            grid.Columns.Add("CodeCommune", "CodeCommune");
            grid.Columns["Id"].Visible = false;
            grid.Columns["CodeCommune"].Visible = false;

            // Button column
            DataGridViewButtonColumn actionColumn =
                new DataGridViewButtonColumn
                {
                    Name = "Action",
                    HeaderText = "العملية",
                    Text = "اختيار",
                    UseColumnTextForButtonValue = true,
                    Width = 120,
                    FlatStyle = FlatStyle.Flat
                };
            actionColumn.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(0, 82, 204),
                    ForeColor = Color.White,
                    Font = new Font(
                        "Cairo",
                        9,
                        FontStyle.Bold)
                };
            grid.CellContentClick += async delegate (
    object sender,
    DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0)
                    return;

                string codeCommune =
                    grid.Rows[e.RowIndex]
                        .Cells["CodeCommune"]
                        .Value
                        .ToString();

                string codeCentre =
                    grid.Rows[e.RowIndex]
                        .Cells["CodeCentre"]
                        .Value
                        .ToString();

                await ShowApwBureaux(
    codeCommune,
    codeCentre,
    ShowApcCentres);
            };
            EnableGridHover(grid);

            grid.Columns.Add(actionColumn);
            page.Controls.Add(grid);

            Action centerControls = delegate
            {
                // Center title
                title.Location = new Point(
                    (page.ClientSize.Width - title.Width) / 2,
                    10);

                // Center grid
                grid.Location = new Point(
                    (page.ClientSize.Width - grid.Width) / 2,
                    70);

                // Put back button near the left side of the centered grid
                backButton.Location = new Point(
                    grid.Left,
                    15);
            };

            page.Resize += delegate
            {
                centerControls();
            };

            centerControls();

            try
            {
                Label loadingLabel = new Label
                {
                    Text = "جاري تحميل المراكز...",
                    Dock = DockStyle.Fill,
                    ForeColor = TextMuted,
                    Font = new Font("Cairo", 11),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                page.Controls.Add(loadingLabel);
                loadingLabel.BringToFront();

                List<CentreDto> centres = await _services.GetAvailableCentresAsync();

                page.Controls.Remove(loadingLabel);
                loadingLabel.Dispose();

                foreach (CentreDto centre in centres)
                {
                    grid.Rows.Add(
                        centre.Code,
                        centre.Lib,
                        centre.Id,
                        centre.CodeCommune);
                }

            

                
        }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                ShowPortal();
                string s = ex.ToString();
            }
        }

        private void OpenApw()
        {
            ShowApwCommunes();
        }

        private async Task ShowApwCommunes()
        {
            contentPanel.Controls.Clear();

            Panel page =
                new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = Color.Transparent
                };

            contentPanel.Controls.Add(page);

            // Back button
            Button backButton =
                new Button
                {
                    Text = "رجوع",
                    Size = new Size(100, 40),
                    Location = new Point(10, 10),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White,
                    ForeColor = TextMain,
                    Font = new Font(
                        "Cairo",
                        9,
                        FontStyle.Bold),
                    Cursor = Cursors.Hand
                };

            backButton.FlatAppearance.BorderColor =
                BorderColor;

            backButton.FlatAppearance.BorderSize = 1;

            backButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(226, 232, 240);

            backButton.MouseEnter += delegate
            {
                backButton.BackColor =
                    Color.FromArgb(226, 232, 240);
            };

            backButton.MouseLeave += delegate
            {
                backButton.BackColor = Color.White;
            };

            backButton.Click += delegate
            {
                ShowPortal();
            };

            page.Controls.Add(backButton);

            // Title
            Label title =
                new Label
                {
                    Text = "قائمة البلديات",
                    Location = new Point(140, 5),
                    Width = 700,
                    Height = 50,
                    ForeColor = BlueDark,
                    Font = new Font(
                        "Cairo",
                        18,
                        FontStyle.Bold),
                    TextAlign =
                        ContentAlignment.MiddleCenter
                };

            page.Controls.Add(title);

            // Grid
            DataGridView grid =
                CreateDataGrid();
            EnableGridHover(grid);
            grid.Location =
                new Point(30, 70);

            grid.Size =
                new Size(980, 450);

            grid.Columns.Add(
                "CodeCommune",
                "رمز البلدية");

            grid.Columns.Add(
                "NomCommune",
                "اسم البلدية");

            page.Controls.Add(grid);

            Action centerControls = delegate
            {
                // Center title
                title.Location = new Point(
                    (page.ClientSize.Width - title.Width) / 2,
                    10);

                // Center grid
                grid.Location = new Point(
                    (page.ClientSize.Width - grid.Width) / 2,
                    70);

                // Put back button near the left side of the centered grid
                backButton.Location = new Point(
                    grid.Left,
                    15);
            };

            page.Resize += delegate
            {
                centerControls();
            };

            centerControls();

            try
            {
                Label loadingLabel =
                    new Label
                    {
                        Text = "جاري تحميل البلديات...",
                        Dock = DockStyle.Fill,
                        ForeColor = TextMuted,
                        Font = new Font(
                            "Cairo",
                            11),
                        TextAlign =
                            ContentAlignment.MiddleCenter
                    };

                contentPanel.Controls.Add(loadingLabel);

                List<CommuneDto> communes =  await _services.GetAvailableCommunesAsync();

                contentPanel.Controls.Remove(loadingLabel);

                foreach (CommuneDto commune in communes)
                {
                    grid.Rows.Add(
                        commune.CodeCommune,
                        commune.NomCommune);
                }

                grid.CellDoubleClick += async delegate (
    object sender,
    DataGridViewCellEventArgs e)
                {
                    if (e.RowIndex < 0)
                        return;

                    string codeCommune =
                        grid.Rows[e.RowIndex]
                            .Cells["CodeCommune"]
                            .Value
                            .ToString();

                    await ShowApwCentres(codeCommune);
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                ShowPortal();
            }
        }

        private async Task ShowApwCentres(string codeCommune)
        {
            contentPanel.Controls.Clear();

            Panel page = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            contentPanel.Controls.Add(page);

            // Back button
            Button backButton = new Button
            {
                Text = "رجوع",
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = TextMain,
                Font = new Font("Cairo", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            backButton.FlatAppearance.BorderColor = BorderColor;
            backButton.FlatAppearance.BorderSize = 1;

            backButton.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(226, 232, 240);

            backButton.MouseEnter += delegate
            {
                backButton.BackColor =
                    Color.FromArgb(226, 232, 240);
            };

            backButton.MouseLeave += delegate
            {
                backButton.BackColor = Color.White;
            };

            // Return to communes
            backButton.Click += async delegate
            {
                await ShowApwCommunes();
            };

            page.Controls.Add(backButton);

            // Title
            Label title = new Label
            {
                Text = "مراكز الاقتراع",
                Size = new Size(700, 50),
                ForeColor = BlueDark,
                Font = new Font("Cairo", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            page.Controls.Add(title);

            // Grid
            DataGridView grid = CreateDataGrid();
            EnableGridHover(grid);

            grid.Size = new Size(980, 450);

            grid.Columns.Add("Id", "Id");

            grid.Columns["Id"].Visible = false;

            grid.Columns.Add(
                "Code",
                "رمز المركز");

            grid.Columns.Add(
                "Lib",
                "اسم المركز");

            // Button column
            DataGridViewButtonColumn actionColumn =
                new DataGridViewButtonColumn
                {
                    Name = "Action",
                    HeaderText = "العملية",
                    Text = "اختيار",
                    UseColumnTextForButtonValue = true,
                    Width = 120,
                    FlatStyle = FlatStyle.Flat
                };

            actionColumn.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = Color.FromArgb(0, 82, 204),
                    ForeColor = Color.White,
                    Font = new Font(
                        "Cairo",
                        9,
                        FontStyle.Bold)
                };

            grid.Columns.Add(actionColumn);

            page.Controls.Add(grid);

            // Center controls
            Action centerControls = delegate
            {
                title.Location = new Point(
                    (page.ClientSize.Width - title.Width) / 2,
                    10);

                grid.Location = new Point(
                    (page.ClientSize.Width - grid.Width) / 2,
                    70);

                backButton.Location = new Point(
                    grid.Left,
                    15);
            };

            page.Resize += delegate
            {
                centerControls();
            };

            centerControls();

            try
            {
                Label loadingLabel = new Label
                {
                    Text = "جاري تحميل المراكز...",
                    Dock = DockStyle.Fill,
                    ForeColor = TextMuted,
                    Font = new Font("Cairo", 11),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                page.Controls.Add(loadingLabel);
                loadingLabel.BringToFront();

                List<CentreDto> centres =
                    await _services.GetAvailableCentresAsync(codeCommune);

                page.Controls.Remove(loadingLabel);
                loadingLabel.Dispose();

                foreach (CentreDto centre in centres)
                {
                    int rowIndex = grid.Rows.Add(
                        centre.Code,
                        centre.Lib);

                    // Keep the ID/code hidden from the user
                    grid.Rows[rowIndex].Cells["Code"].Tag =
                        centre.Code;
                }

                // Button click instead of double-click
                grid.CellContentClick += async delegate (
                    object sender,
                    DataGridViewCellEventArgs e)
                {
                    if (e.RowIndex < 0)
                        return;

                    if (e.ColumnIndex !=
                        grid.Columns["Action"].Index)
                        return;

                    string codeCentre =
                        grid.Rows[e.RowIndex]
                            .Cells["Code"]
                            .Tag?
                            .ToString();

                    if (string.IsNullOrEmpty(codeCentre))
                        return;

                    await ShowApwBureaux(
    codeCommune,
    codeCentre,
    async () => await ShowApwCentres(codeCommune));
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                await ShowApwCommunes();
            }
        }

        private async Task ShowApwBureaux(
    string codeCommune,
    string codeCentre,
    Func<Task> backAction)
        {
            
            contentPanel.Controls.Clear();

            Panel page = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            contentPanel.Controls.Add(page);

            //Panel panel = CreateContentPanel();
            // Back button
            Button backButton = new Button
            {
                Text = "رجوع",
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = TextMain,
                Font = new Font("Cairo", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            backButton.FlatAppearance.BorderColor = BorderColor;
            backButton.FlatAppearance.BorderSize = 1;

            backButton.MouseEnter += delegate
            {
                backButton.BackColor =
                    Color.FromArgb(226, 232, 240);
            };

            backButton.MouseLeave += delegate
            {
                backButton.BackColor = Color.White;
            };

            backButton.Click += async delegate
            {
                await backAction();
            };

            page.Controls.Add(backButton);

            Label title = new Label
            {
                Text = "مكاتب التصويت",
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Cairo", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.MiddleCenter
            };

            DataGridView grid = CreateDataGrid();
            grid.Dock = DockStyle.Fill;

            grid.Columns.Add("Id", "Id");

            grid.Columns["Id"].Visible = false;

            grid.Columns.Add(
                "Code",
                "رمز المكتب");

            grid.Columns.Add(
                "Type",
                "النوع");

            grid.Columns.Add(
                "NbInscrit",
                "عدد المسجلين");

            DataGridViewButtonColumn actionColumn =
    new DataGridViewButtonColumn
    {
        Name = "Action",
        HeaderText = "العملية",
        Text = "اختيار",
        UseColumnTextForButtonValue = true,
        Width = 120,
        FlatStyle = FlatStyle.Flat
    };

            actionColumn.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    BackColor = BlueDark,
                    ForeColor = Color.White,
                    Font = new Font(
                        "Cairo",
                        9,
                        FontStyle.Bold)
                };

            grid.Columns.Add(actionColumn);

            grid.CellContentClick += async delegate (
     object sender,
     DataGridViewCellEventArgs e)
            {
                if (e.RowIndex < 0)
                    return;

                if (e.ColumnIndex !=
                    grid.Columns["Action"].Index)
                    return;

                string codeBureau =
                    grid.Rows[e.RowIndex]
                        .Cells["Code"]
                        .Value?
                        .ToString();

                if (string.IsNullOrWhiteSpace(codeBureau))
                    return;

                Func<Task> returnToBureaux = async () =>
                            await ShowApwBureaux(
                                codeCommune,
                                codeCentre,
                                backAction);

                await ShowListes(
                        codeCommune,
                        codeCentre,
                        returnToBureaux);
            };

            EnableGridHover(grid);

            page.Controls.Add(grid);
            page.Controls.Add(title);

            contentPanel.Controls.Add(page);

            try
            {
                List<BureauDto> bureaux =
                    await _services.GetAvailableBureauxAsync(
                        codeCentre,
                        codeCommune);

                foreach (BureauDto bureau in bureaux)
                {
                    grid.Rows.Add(
                        bureau.Id,
                        bureau.Code,
                        bureau.Type,
                        bureau.NbInscrit);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private Panel CreateContentPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(50, 30, 50, 30)
            };

            return panel;
        }

        private async Task ShowListes(
    string codeCommune,
    string codeCentre,
    Func<Task> backToBureaux)
        {
            contentPanel.Controls.Clear();

            Panel page = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            contentPanel.Controls.Add(page);

            // Back button
            Button backButton = new Button
            {
                Text = "رجوع",
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = TextMain,
                Font = new Font("Cairo", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            backButton.FlatAppearance.BorderColor = BorderColor;
            backButton.FlatAppearance.BorderSize = 1;

            backButton.MouseEnter += delegate
            {
                backButton.BackColor =
                    Color.FromArgb(226, 232, 240);
            };

            backButton.MouseLeave += delegate
            {
                backButton.BackColor = Color.White;
            };

            backButton.Click += async delegate
            {
                await backToBureaux();
            };

            page.Controls.Add(backButton);

            // Title
            Label title = new Label
            {
                Text = "القوائم الانتخابية",
                Size = new Size(700, 50),
                ForeColor = BlueDark,
                Font = new Font("Cairo", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            page.Controls.Add(title);

            // Grid
            DataGridView grid = CreateDataGrid();
            EnableGridHover(grid);

            grid.Size = new Size(980, 450);

            // Internal ID - hidden
            grid.Columns.Add(
                "Id",
                "Id");

            grid.Columns["Id"].Visible = false;

            grid.Columns.Add(
                "NumIdentifiant",
                "رقم التعريف");

            grid.Columns.Add(
                "Lib",
                "اسم القائمة");

            // Action button
            DataGridViewButtonColumn actionColumn =
                new DataGridViewButtonColumn
                {
                    Name = "Action",
                    HeaderText = "العملية",
                    Text = "اختيار",
                    UseColumnTextForButtonValue = true,
                    Width = 120,
                    FlatStyle = FlatStyle.Flat
                };

            actionColumn.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    Alignment =
                        DataGridViewContentAlignment.MiddleCenter,

                    BackColor = BlueDark,
                    ForeColor = Color.White,

                    Font = new Font(
                        "Cairo",
                        9,
                        FontStyle.Bold)
                };

            grid.Columns.Add(actionColumn);

            page.Controls.Add(grid);

            // Center
            Action centerControls = delegate
            {
                title.Location = new Point(
                    (page.ClientSize.Width - title.Width) / 2,
                    10);

                grid.Location = new Point(
                    (page.ClientSize.Width - grid.Width) / 2,
                    70);

                backButton.Location = new Point(
                    grid.Left,
                    15);
            };

            page.Resize += delegate
            {
                centerControls();
            };

            centerControls();

            try
            {
                Label loadingLabel = new Label
                {
                    Text = "جاري تحميل القوائم...",
                    Dock = DockStyle.Fill,
                    ForeColor = TextMuted,
                    Font = new Font("Cairo", 11),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                page.Controls.Add(loadingLabel);
                loadingLabel.BringToFront();

                List<ListeDto> listes =
                    await _services.GetListesAsync();

                page.Controls.Remove(loadingLabel);
                loadingLabel.Dispose();

                foreach (ListeDto liste in listes)
                {
                    grid.Rows.Add(
                        liste.Id,
                        liste.NumIdentifiant,
                        liste.Lib);
                }

                // Row button
                grid.CellContentClick += async delegate (
                    object sender,
                    DataGridViewCellEventArgs e)
                {
                    if (e.RowIndex < 0)
                        return;

                    if (e.ColumnIndex !=
                        grid.Columns["Action"].Index)
                        return;

                    int id =
                        Convert.ToInt32(
                            grid.Rows[e.RowIndex]
                                .Cells["Id"]
                                .Value);

                    // Here you can open the liste
                    await ShowListeCandidats( id,
                            async () => await ShowListes(
                                codeCommune,
                                codeCentre,
                                backToBureaux));
                                        };
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async Task ShowListeCandidats(
    int idListe,
    Func<Task> backToListes)
        {
            contentPanel.Controls.Clear();

            Panel page = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            contentPanel.Controls.Add(page);

            // Back button
            Button backButton = new Button
            {
                Text = "رجوع",
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = TextMain,
                Font = new Font("Cairo", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            backButton.FlatAppearance.BorderColor = BorderColor;
            backButton.FlatAppearance.BorderSize = 1;

            backButton.MouseEnter += delegate
            {
                backButton.BackColor =
                    Color.FromArgb(226, 232, 240);
            };

            backButton.MouseLeave += delegate
            {
                backButton.BackColor = Color.White;
            };

            backButton.Click += async delegate
            {
                await backToListes();
            };

            page.Controls.Add(backButton);

            // Title
            Label title = new Label
            {
                Text = "قائمة المترشحين",
                Size = new Size(700, 50),
                ForeColor = BlueDark,
                Font = new Font("Cairo", 18, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            page.Controls.Add(title);

            // Grid
            DataGridView grid = CreateDataGrid();
            EnableGridHover(grid);

            grid.Size = new Size(980, 450);

            grid.Columns.Add(
                "Id",
                "Id");

            // Hide internal ID
            grid.Columns["Id"].Visible = false;

            grid.Columns.Add(
                "Order",
                "الترتيب");

            grid.Columns.Add(
                "Nom",
                "اسم المترشح");

            grid.Columns.Add(
                "Prenom",
                "لقب المترشح");

            page.Controls.Add(grid);

            // Center controls
            Action centerControls = delegate
            {
                title.Location = new Point(
                    (page.ClientSize.Width - title.Width) / 2,
                    10);

                grid.Location = new Point(
                    (page.ClientSize.Width - grid.Width) / 2,
                    70);

                backButton.Location = new Point(
                    grid.Left,
                    15);
            };

            page.Resize += delegate
            {
                centerControls();
            };

            centerControls();

            try
            {
                Label loadingLabel = new Label
                {
                    Text = "جاري تحميل المترشحين...",
                    Dock = DockStyle.Fill,
                    ForeColor = TextMuted,
                    Font = new Font("Cairo", 11),
                    TextAlign = ContentAlignment.MiddleCenter
                };

                page.Controls.Add(loadingLabel);
                loadingLabel.BringToFront();

                List<CandidatDto> candidats =
                    await _services.GetCandidatsAsync(idListe);

                page.Controls.Remove(loadingLabel);
                loadingLabel.Dispose();

                foreach (CandidatDto candidat in candidats)
                {
                    grid.Rows.Add(
                        candidat.Id,
                        candidat.Ordre,
                        candidat.FullNameAr,
                        candidat.FullNameFr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                await backToListes();
            }
        }

        private DataGridView CreateDataGrid()
        {
            DataGridView grid = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,

                ReadOnly = true,
                MultiSelect = false,

                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,

                RowHeadersVisible = false,

                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,

                CellBorderStyle =
                    DataGridViewCellBorderStyle.SingleHorizontal,

                GridColor =
                    Color.FromArgb(226, 232, 240),

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,

                RightToLeft = RightToLeft.Yes,

                Font = new Font("Cairo", 10),

                EnableHeadersVisualStyles = false,

                ColumnHeadersHeight = 48,

                RowTemplate =
        {
            Height = 44
        }
            };

            // Header
            grid.ColumnHeadersDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor =
                        Color.FromArgb(241, 245, 249),

                    ForeColor =
                        Color.FromArgb(30, 41, 59),

                    Font =
                        new Font(
                            "Cairo",
                            10,
                            FontStyle.Bold),

                    Alignment =
                        DataGridViewContentAlignment.MiddleCenter,

                    SelectionBackColor =
                        Color.FromArgb(241, 245, 249),

                    SelectionForeColor =
                        Color.FromArgb(30, 41, 59),

                    Padding =
                        new Padding(5)
                };

            // Rows
            grid.DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor = Color.White,

                    ForeColor =
                        Color.FromArgb(30, 41, 59),

                    Font =
                        new Font("Cairo", 10),

                    Alignment =
                        DataGridViewContentAlignment.MiddleCenter,

                    SelectionBackColor =
                        Color.FromArgb(226, 232, 240),

                    SelectionForeColor =
                        Color.FromArgb(30, 41, 59),

                    Padding =
                        new Padding(8, 0, 8, 0)
                };

            // Alternating rows
            grid.AlternatingRowsDefaultCellStyle =
                new DataGridViewCellStyle
                {
                    BackColor =
                        Color.FromArgb(248, 250, 252),

                    ForeColor =
                        Color.FromArgb(30, 41, 59),

                    SelectionBackColor =
                        Color.FromArgb(226, 232, 240),

                    SelectionForeColor =
                        Color.FromArgb(30, 41, 59)
                };

            // Remove the ugly selected-row border
            grid.RowTemplate.Resizable =
                DataGridViewTriState.False;

            return grid;
        }

        private void EnableGridHover(DataGridView grid)
        {
            grid.CellMouseEnter += delegate (
                object sender,
                DataGridViewCellEventArgs e)
            {
                if (e.RowIndex >= 0 &&
                    !grid.Rows[e.RowIndex].Selected)
                {
                    grid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                        Color.FromArgb(248, 250, 252);
                }
            };

            grid.CellMouseLeave += delegate (
                object sender,
                DataGridViewCellEventArgs e)
            {
                if (e.RowIndex >= 0 &&
                    !grid.Rows[e.RowIndex].Selected)
                {
                    if (e.RowIndex % 2 == 0)
                    {
                        grid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                            Color.White;
                    }
                    else
                    {
                        grid.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                            Color.FromArgb(248, 250, 252);
                    }
                }
            };
        }

       
    }
}
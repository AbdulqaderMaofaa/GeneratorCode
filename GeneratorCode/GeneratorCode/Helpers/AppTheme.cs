using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace GeneratorCode.Helpers
{
    public static class AppTheme
    {
        // Primary palette
        public static readonly Color Primary = Color.FromArgb(52, 152, 219);
        public static readonly Color PrimaryDark = Color.FromArgb(44, 62, 80);
        public static readonly Color PrimaryLight = Color.FromArgb(41, 128, 185);
        public static readonly Color Success = Color.FromArgb(46, 204, 113);
        public static readonly Color SuccessDark = Color.FromArgb(39, 174, 96);
        public static readonly Color Danger = Color.FromArgb(231, 76, 60);
        public static readonly Color DangerDark = Color.FromArgb(192, 57, 43);
        public static readonly Color Warning = Color.FromArgb(243, 156, 18);
        public static readonly Color WarningDark = Color.FromArgb(211, 84, 0);
        public static readonly Color Purple = Color.FromArgb(155, 89, 182);
        public static readonly Color PurpleDark = Color.FromArgb(142, 68, 173);
        public static readonly Color Gray = Color.FromArgb(149, 165, 166);
        public static readonly Color GrayDark = Color.FromArgb(127, 140, 141);

        // Surface colors
        public static readonly Color SurfaceWhite = Color.White;
        public static readonly Color FormBackground = Color.FromArgb(248, 249, 250);
        public static readonly Color SurfaceLight = Color.FromArgb(245, 245, 245);
        public static readonly Color PanelBackground = Color.FromArgb(236, 240, 241);
        public static readonly Color AlternateRow = Color.FromArgb(245, 245, 245);
        public static readonly Color GridLine = Color.FromArgb(220, 220, 220);
        public static readonly Color Transparent = Color.Transparent;
        public static readonly Color ConsoleBg = Color.FromArgb(30, 30, 30);
        public static readonly Color ConsoleFg = Color.FromArgb(204, 204, 204);
        public static readonly Color ConsoleHighlight = Color.FromArgb(220, 220, 170);

        // Text colors
        public static readonly Color TextPrimary = Color.FromArgb(44, 62, 80);
        public static readonly Color TextSecondary = Color.FromArgb(189, 195, 199);
        public static readonly Color TextOnDark = Color.White;
        public static readonly Color TextOnLight = Color.FromArgb(44, 62, 80);

        // Console output colors
        public static readonly Color ConsoleSuccess = Color.Lime;
        public static readonly Color ConsoleError = Color.Red;
        public static readonly Color ConsoleWarning = Color.Yellow;
        public static readonly Color ConsoleInfo = Color.Cyan;
        public static readonly Color ConsoleNeutral = Color.White;
        public static readonly Color ConsoleDetail = Color.LightGray;
        public static readonly Color ConsoleComplete = Color.Green;

        // Status indicator colors
        public static readonly Color StatusSuccess = Color.DarkGreen;
        public static readonly Color StatusError = Color.DarkRed;
        public static readonly Color StatusWarning = Color.DarkOrange;
        public static readonly Color StatusInfo = Color.DarkBlue;
        public static readonly Color StatusLedGreen = Color.Green;
        public static readonly Color StatusLedRed = Color.Red;
        public static readonly Color StatusLedOrange = Color.Orange;
        public static readonly Color StatusLedBlue = Color.Blue;

        // Log level colors
        public static readonly Color LogError = Color.FromArgb(231, 76, 60);
        public static readonly Color LogWarning = Color.FromArgb(243, 156, 18);
        public static readonly Color LogInfo = Color.FromArgb(52, 152, 219);
        public static readonly Color LogDebug = Color.FromArgb(149, 165, 166);

        // Syntax highlighting colors
        public static readonly Color SyntaxKeyword = Color.FromArgb(86, 156, 214);
        public static readonly Color SyntaxString = Color.FromArgb(214, 157, 133);
        public static readonly Color SyntaxComment = Color.FromArgb(106, 153, 85);

        // Fonts (cached via Lazy to avoid repeated allocations)
        private static readonly Lazy<Font> _defaultFont = new(() => new Font("Segoe UI", 10F));
        private static readonly Lazy<Font> _defaultFontSmall = new(() => new Font("Segoe UI", 9F));
        private static readonly Lazy<Font> _defaultFontBold = new(() => new Font("Segoe UI", 10F, FontStyle.Bold));
        private static readonly Lazy<Font> _headerFont = new(() => new Font("Segoe UI", 12F, FontStyle.Bold));
        private static readonly Lazy<Font> _headerFontSemibold = new(() => new Font("Segoe UI Semibold", 11F));
        private static readonly Lazy<Font> _consoleFont = new(() => new Font("Consolas", 10F));
        private static readonly Lazy<Font> _consoleFontSmall = new(() => new Font("Consolas", 9F));
        private static readonly Lazy<Font> _buttonFont = new(() => new Font("Segoe UI", 9F, FontStyle.Bold));
        private static readonly Lazy<Font> _buttonFontLarge = new(() => new Font("Segoe UI", 11F, FontStyle.Bold));

        public static Font DefaultFont => _defaultFont.Value;
        public static Font DefaultFontSmall => _defaultFontSmall.Value;
        public static Font DefaultFontBold => _defaultFontBold.Value;
        public static Font HeaderFont => _headerFont.Value;
        public static Font HeaderFontSemibold => _headerFontSemibold.Value;
        public static Font ConsoleFont => _consoleFont.Value;
        public static Font ConsoleFontSmall => _consoleFontSmall.Value;
        public static Font ButtonFont => _buttonFont.Value;
        public static Font ButtonFontLarge => _buttonFontLarge.Value;

        public static void StyleForm(Form form)
        {
            form.BackColor = FormBackground;
            form.Font = DefaultFont;
            form.RightToLeft = RightToLeft.Yes;
            form.RightToLeftLayout = true;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.KeyPreview = true;
        }

        public static void StyleButton(Button btn, Color bgColor, bool large = false)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Lighten(bgColor, 0.15f);
            btn.FlatAppearance.MouseDownBackColor = Darken(bgColor, 0.1f);
            btn.BackColor = bgColor;
            btn.ForeColor = Color.White;
            btn.Font = large ? ButtonFontLarge : ButtonFont;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
            if (btn.Height < 35) btn.Height = 35;
            if (btn.Width < 100) btn.Width = 100;
        }

        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = GridLine;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.EnableHeadersVisualStyles = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = DefaultFontBold;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(5);
            dgv.ColumnHeadersHeight = 40;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgv.DefaultCellStyle.Font = DefaultFontSmall;
            dgv.DefaultCellStyle.Padding = new Padding(4);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(60, Primary.R, Primary.G, Primary.B);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = AlternateRow;
            dgv.RowTemplate.Height = 34;
        }

        public static void StyleGroupBox(GroupBox grp, Color titleColor)
        {
            grp.Font = DefaultFontBold;
            grp.ForeColor = titleColor;
            grp.Padding = new Padding(10);
            grp.RightToLeft = RightToLeft.Yes;
        }

        public static void StyleRichTextBoxConsole(RichTextBox rtb)
        {
            rtb.BackColor = ConsoleBg;
            rtb.ForeColor = ConsoleFg;
            rtb.Font = ConsoleFont;
            rtb.ReadOnly = true;
            rtb.BorderStyle = BorderStyle.None;
            rtb.WordWrap = true;
            rtb.RightToLeft = RightToLeft.No;
        }

        public static void StyleTabControl(TabControl tab)
        {
            tab.Font = DefaultFont;
            tab.RightToLeft = RightToLeft.Yes;
            tab.RightToLeftLayout = true;
            tab.SizeMode = TabSizeMode.Fixed;
            tab.ItemSize = new Size(200, 35);
        }

        public static ToolTip CreateTooltipProvider()
        {
            return new ToolTip
            {
                AutoPopDelay = 5000,
                InitialDelay = 500,
                ReshowDelay = 200,
                ShowAlways = true,
                BackColor = PrimaryDark,
                ForeColor = Color.White
            };
        }

        public static Color GetLogLevelColor(string level)
        {
            return level?.ToLower() switch
            {
                "error" => LogError,
                "warning" => LogWarning,
                "info" or "information" => LogInfo,
                "debug" => LogDebug,
                _ => Color.White
            };
        }

        private static Color Lighten(Color color, float amount)
        {
            int r = Math.Min(255, color.R + (int)(255 * amount));
            int g = Math.Min(255, color.G + (int)(255 * amount));
            int b = Math.Min(255, color.B + (int)(255 * amount));
            return Color.FromArgb(color.A, r, g, b);
        }

        private static Color Darken(Color color, float amount)
        {
            int r = Math.Max(0, color.R - (int)(255 * amount));
            int g = Math.Max(0, color.G - (int)(255 * amount));
            int b = Math.Max(0, color.B - (int)(255 * amount));
            return Color.FromArgb(color.A, r, g, b);
        }
    }
}

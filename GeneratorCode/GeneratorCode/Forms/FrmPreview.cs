using GeneratorCode.Core.Models;
using GeneratorCode.Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GeneratorCode.Forms
{
    public partial class FrmPreview : Form
    {
        public FrmPreview(PreviewResult previewResult)
        {
            InitializeComponent();

            var content = previewResult.ToString();
            txtPreview.Text = content;

            int lineCount = content.Split('\n').Length;
            lblLineCount.Text = $"الأسطر: {lineCount}";

            btnCopy.Click += (s, e) =>
            {
                Clipboard.SetText(txtPreview.Text);
                btnCopy.Text = "تم النسخ!";
                var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                timer.Tick += (ts, te) => { btnCopy.Text = "نسخ الكل"; timer.Stop(); timer.Dispose(); };
                timer.Start();
            };

            btnSaveAs.Click += (s, e) =>
            {
                using var dlg = new SaveFileDialog
                {
                    Filter = "C# Files (*.cs)|*.cs|All Files (*.*)|*.*",
                    FileName = "Preview.cs",
                    Title = "حفظ الكود"
                };
                if (dlg.ShowDialog() == DialogResult.OK)
                    File.WriteAllText(dlg.FileName, txtPreview.Text);
            };

            KeyPreview = true;
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape) Close();
                if (e.Control && e.KeyCode == Keys.C && txtPreview.SelectionLength == 0)
                {
                    Clipboard.SetText(txtPreview.Text);
                    e.SuppressKeyPress = true;
                }
            };

            ApplyBasicHighlighting();
        }

        private void ApplyBasicHighlighting()
        {
            var keywords = new[] { "using", "namespace", "class", "public", "private", "protected", "internal",
                "static", "void", "int", "string", "bool", "var", "new", "return", "if", "else", "for",
                "foreach", "while", "async", "await", "get", "set", "virtual", "override", "abstract",
                "interface", "enum", "struct", "readonly", "const", "null", "true", "false", "this",
                "base", "try", "catch", "finally", "throw", "partial", "sealed", "Task" };

            var keywordColor = AppTheme.SyntaxKeyword;
            var stringColor = AppTheme.SyntaxString;
            var commentColor = AppTheme.SyntaxComment;

            txtPreview.SuspendLayout();

            string text = txtPreview.Text;

            // Color keywords
            foreach (var kw in keywords)
            {
                int idx = 0;
                while ((idx = text.IndexOf(kw, idx, StringComparison.Ordinal)) >= 0)
                {
                    bool validStart = idx == 0 || !char.IsLetterOrDigit(text[idx - 1]);
                    bool validEnd = idx + kw.Length >= text.Length || !char.IsLetterOrDigit(text[idx + kw.Length]);
                    if (validStart && validEnd)
                    {
                        txtPreview.Select(idx, kw.Length);
                        txtPreview.SelectionColor = keywordColor;
                    }
                    idx += kw.Length;
                }
            }

            // Color single-line comments
            int ci = 0;
            while ((ci = text.IndexOf("//", ci, StringComparison.Ordinal)) >= 0)
            {
                int lineEnd = text.IndexOf('\n', ci);
                if (lineEnd < 0) lineEnd = text.Length;
                txtPreview.Select(ci, lineEnd - ci);
                txtPreview.SelectionColor = commentColor;
                ci = lineEnd;
            }

            // Color strings
            int si = 0;
            while ((si = text.IndexOf('"', si)) >= 0)
            {
                int endQuote = text.IndexOf('"', si + 1);
                if (endQuote < 0) break;
                txtPreview.Select(si, endQuote - si + 1);
                txtPreview.SelectionColor = stringColor;
                si = endQuote + 1;
            }

            txtPreview.Select(0, 0);
            txtPreview.ResumeLayout();
        }
    }
}

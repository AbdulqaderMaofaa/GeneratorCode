using GeneratorCode.Core.Models;
using System.Windows.Forms;

namespace GeneratorCode.Forms
{
    public partial class FrmPreview : Form
    {
        public FrmPreview(PreviewResult previewResult)
        {
            InitializeComponent();
            txtPreview.Text = previewResult.ToString();
        }
    }
} 
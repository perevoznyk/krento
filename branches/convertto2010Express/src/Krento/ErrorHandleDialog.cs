using System;
using System.Drawing;
using System.Windows.Forms;
using Laugris.Sage;

namespace Krento
{
    internal partial class ErrorHandleDialog : Form
    {
        public ErrorHandleDialog()
        {
            InitializeComponent();
        }


        private void ErrorText_TextChanged(object sender, EventArgs e)
        {
            Size sz = new Size(ErrorText.ClientSize.Width, int.MaxValue);
            TextFormatFlags flags = TextFormatFlags.WordBreak;
            int padding = 3;
            int borders = ErrorText.Height - ErrorText.ClientSize.Height;
            sz = TextRenderer.MeasureText(ErrorText.Text, ErrorText.Font, sz, flags);
            int h = sz.Height + borders + padding;
            ErrorText.Height = h;
            this.Height = h + 92;
        }

        private void ErrorHandleDialog_Load(object sender, EventArgs e)
        {
            this.Text = SR.Error;
            btnClose.Text = SR.Close;
            btnRestart.Text = SR.Restart;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Laugris.Sage;

namespace Krento
{
    public partial class UpdateDialog : Form
    {
        public UpdateDialog()
        {
            InitializeComponent();
        }

        private void btnOK_MouseEnter(object sender, EventArgs e)
        {
            btnOK.FlatAppearance.BorderColor = SystemColors.Highlight;
        }

        private void btnOK_MouseLeave(object sender, EventArgs e)
        {
            btnOK.FlatAppearance.BorderColor = SystemColors.Window;
        }

        private void btnCancel_MouseEnter(object sender, EventArgs e)
        {
            btnCancel.FlatAppearance.BorderColor = SystemColors.Highlight;
            btnOK.FlatAppearance.BorderColor = SystemColors.Window;
        }

        private void btnCancel_MouseLeave(object sender, EventArgs e)
        {
            btnCancel.FlatAppearance.BorderColor = SystemColors.Window;
        }

        private void UpdateDialog_Load(object sender, EventArgs e)
        {
            lblAutomaticCheck.Text = SR.CheckUpdates;
            btnOK.Text = SR.CheckUpdateNow;
            btnCancel.Text = SR.DoNotCheckUpdate;
            btnDisable.Text = SR.DoNotAsk;
        }

    }
}

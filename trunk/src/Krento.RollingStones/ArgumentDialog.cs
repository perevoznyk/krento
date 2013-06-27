using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Laugris.Sage;

namespace Krento.RollingStones
{
    internal partial class ArgumentDialog : AeroForm
    {
        public ArgumentDialog()
        {
            InitializeComponent();
        }

        public string Argument
        {
            get { return edtArgument.Text; }
            set { edtArgument.Text = value; }
        }

        public string Description
        {
            get { return lblArgument.Text; }
            set { lblArgument.Text = value; }
        }

        private void ArgumentDialog_Load(object sender, EventArgs e)
        {
            btnOK.Text = SR.OK;
            btnCancel.Text = SR.Cancel;
            this.Text = SR.KrentoShortName;
           // lblArgument.Text = SR.Parameter;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace Laugris.Sage
{
    /// <summary>
    /// This is international message box to make FXCop happy
    /// </summary>
    public static class RtlAwareMessageBox
    {
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return MessageBox.Show(text, caption, buttons, icon);
        }

    }
}

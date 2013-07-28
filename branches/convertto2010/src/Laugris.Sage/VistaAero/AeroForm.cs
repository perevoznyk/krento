//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;

namespace Laugris.Sage
{
    /// <summary>
    /// Form with support of Windows Vista Aero glass effects.
    /// This is only available on Windows Vista with Aero, otherwise it looks like a normal form
    /// </summary>
    [Description("Form with support of Windows Vista Aero glass effects.")]
    public class AeroForm : Form
    {
        private GlassFrame glassFrame;


        /// <summary>
        /// Gets or sets the glass frame.
        /// </summary>
        /// <value>The glass frame.</value>
        [Browsable(true)]
        [Description("GlassFrame controls Windows Vista Aero glass effects")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GlassFrame GlassFrame
        {
            get { return glassFrame; }
        }


        public AeroForm() : base()
        {
            glassFrame = new GlassFrame(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.DesignMode)
            {
                if (this.glassFrame.Enabled)
                {
                    HatchBrush aeroBrush = new HatchBrush(HatchStyle.ForwardDiagonal, Color.Navy, this.BackColor);
                    if (this.glassFrame.SheetOfGlass)
                    {
                        e.Graphics.FillRectangle(aeroBrush, this.ClientRectangle);
                    }
                    else
                    {
                        if (glassFrame.Left > 0)
                        {
                            e.Graphics.FillRectangle(aeroBrush, new Rectangle(0, 0, glassFrame.Left, ClientRectangle.Height));
                        }

                        if (glassFrame.Top > 0)
                        {
                            e.Graphics.FillRectangle(aeroBrush, new Rectangle(0, 0, ClientRectangle.Width, glassFrame.Top));
                        }

                        if (glassFrame.Right > 0)
                        {
                            e.Graphics.FillRectangle(aeroBrush, new Rectangle(ClientRectangle.Width - glassFrame.Right,
                                0, glassFrame.Right, ClientRectangle.Height));
                        }

                        if (glassFrame.Bottom > 0)
                        {
                            e.Graphics.FillRectangle(aeroBrush, new Rectangle(0, ClientRectangle.Bottom - glassFrame.Bottom,
                                Width, glassFrame.Bottom));
                        }
                    }
                }
            }
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case NativeMethods.WM_DWMCOMPOSITIONCHANGED:
                case NativeMethods.WM_DWMNCRENDERINGCHANGED:
                    glassFrame.UpdateGlassFrame();
                    break;
            }
        }

        public bool DesignedState
        {
            get { return this.DesignMode; }
        }
    }
}
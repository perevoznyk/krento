//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Laugris.Sage
{
    /// <summary>
    /// GlassFrame uses the DwmExtendFrameIntoClientArea API to extend glass effects into the client area. 
    /// Use the Top, Left, Bottom, and Right properties to specify the amount to extend the glass effect 
    /// into the client area, or use SheetOfGlass to render the entire client area as a glass surface. 
    /// This is only available on Windows Vista with Aero. 
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Description("GlassFrame controls Windows Vista Aero glass effects")]
    public sealed class GlassFrame
    {
        private AeroForm client;
        private bool enabled;
        private int left;
        private int top;
        private int right;
        private int bottom;
        private bool sheetOfGlass;

        private void SetEnabled(bool value)
        {
            if (value != enabled)
            {
                enabled = value;
                if (enabled && (client.AllowTransparency))
                    client.AllowTransparency = false;
                Change();
            }
        }

        private void SetExtendedFrame(int index, int value)
        {
            switch (index)
            {
                case 0:
                    if (value != left)
                    {
                        left = value;
                        if (value == -1)
                            sheetOfGlass = true;
                    }
                    break;
                case 1:
                    if (value != top)
                    {
                        top = value;
                        if (value == -1)
                            sheetOfGlass = true;
                    }
                    break;
                case 2:
                    if (value != right)
                    {
                        right = value;
                        if (value == -1)
                            sheetOfGlass = true;
                    }
                    break;
                case 3:
                    if (value != bottom)
                    {
                        bottom = value;
                        if (value == -1)
                            sheetOfGlass = true;
                    }
                    break;
                default: break;
            }

            Change();
        }

        private void SetSheetOfGlass(bool value)
        {
            if ((value != sheetOfGlass) && !(sheetOfGlass && ((left == -1) || (top == -1) || (right == -1) || (bottom == -1))))
            {
                sheetOfGlass = value;
                Change();
            }
        }

        public bool FrameExtended()
        {
            return enabled && GlassHelper.CompositionEnabled &&
                (sheetOfGlass || (left != 0) || (top != 0) || (right != 0) || (bottom != 0));
        }

        internal void Change()
        {
            UpdateGlassFrame();
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GlassFrame"/> class.
        /// </summary>
        /// <param name="client">The client.</param>
        public GlassFrame(AeroForm client)
        {
            this.client = client;
            if (client == null)
                throw new ArgumentNullException("client");
        }

        /// <summary>
        /// Occurs when [changed].
        /// </summary>
        public event EventHandler Changed;

        /// <summary>
        /// Raises the <see cref="E:Changed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        internal void OnChanged(EventArgs e)
        {
            EventHandler handler = Changed;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GlassFrame"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Browsable(true)]
        [Description("Controls whether glass effects are extended into the client area or not.")]
        public bool Enabled
        {
            get { return enabled; }
            set { SetEnabled(value); }
        }

        /// <summary>
        /// Extends glass effects into the entire client area
        /// </summary>
        /// <value><c>true</c> if glass effect extended to the entire client area; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Browsable(true)]
        [Description("Extends glass effects into the entire client area")]
        public bool SheetOfGlass
        {
            get { return sheetOfGlass; }
            set { SetSheetOfGlass(value); }
        }

        /// <summary>
        /// Specifies the amount (in pixels) glass effects are extended into the client area.
        /// </summary>
        /// <value>The left.</value>
        [DefaultValue(0), Browsable(true)]
        [Description("Specifies the amount (in pixels) glass effects are extended into the client area.")]
        public int Left
        {
            get { return left; }
            set { SetExtendedFrame(0, value); }
        }

        /// <summary>
        /// Specifies the amount (in pixels) glass effects are extended into the client area.
        /// </summary>
        /// <value>The top.</value>
        [DefaultValue(0), Browsable(true)]
        [Description("Specifies the amount (in pixels) glass effects are extended into the client area.")]
        public int Top
        {
            get { return top; }
            set { SetExtendedFrame(1, value); }
        }

        /// <summary>
        /// Specifies the amount (in pixels) glass effects are extended into the client area.
        /// </summary>
        /// <value>The right.</value>
        [DefaultValue(0), Browsable(true)]
        [Description("Specifies the amount (in pixels) glass effects are extended into the client area.")]
        public int Right
        {
            get { return right; }
            set { SetExtendedFrame(2, value); }
        }

        /// <summary>
        /// Specifies the amount (in pixels) glass effects are extended into the client area.
        /// </summary>
        /// <value>The bottom.</value>
        [DefaultValue(0), Browsable(true)]
        [Description("Specifies the amount (in pixels) glass effects are extended into the client area.")]
        public int Bottom
        {
            get { return bottom; }
            set { SetExtendedFrame(3, value); }
        }

        public void UpdateGlassFrame()
        {
            GlassMargins margins = new GlassMargins();

            if ((client.Handle != IntPtr.Zero) && (GlassHelper.CompositionEnabled))
            {
                if (enabled)
                {
                    if (sheetOfGlass)
                    {
                        margins.SheetOfGlass();
                    }
                    else
                    {
                        margins.LeftWidth = left;
                        margins.RightWidth = right;
                        margins.BottomHeight = bottom;
                        margins.TopHeight = top;
                    }
                }
                else
                {
                    margins.None();
                }

                if (client.DesignedState)
                {
                    client.Invalidate();
                }
                else
                {
                    GlassHelper.Extend(client, margins);
                    client.Invalidate();
                }
            }
            else
                client.Invalidate();
        }
    }
}

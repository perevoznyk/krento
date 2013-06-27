//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Laugris.Sage
{
    /// <summary>
    /// Position of Windows taskbar on the screen
    /// </summary>
    internal enum TaskbarPosition : int
    {
        /// <summary>
        /// Taskbar is located at the left side of the screen
        /// </summary>
        ScreenLeft = 0,
        /// <summary>
        /// Taskbar is located at the top of the screen
        /// </summary>
        ScreenTop = 1,
        /// <summary>
        /// Taskbar is located at the right side of the screen
        /// </summary>
        ScreenRight = 2,
        /// <summary>
        /// Taskbar is located at the bottom of the screen
        /// </summary>
        ScreenBottom = 3
    }

    internal enum NotifyTimers : int
    {
        Hidden = 0,
        Appearing = 1,
        Waiting = 2,
        Disappearing = 3
    }

    public partial class Notifier : Component
    {
        private static Font defaultFont;
        private string caption;
        private Color backColor = Color.FromKnownColor(KnownColor.Window);
        private Font font;
        private int glyphSize = 64;
        private int width = 400;
        private int scrollTime = 500;
        private int height = 100;
        private int showTime = 5000;
        private string url;
        private Color textColor = Color.Gray;
        private bool sliding = true;
        private int precision = 10;
        internal bool active;
        private Color captionColor = Color.Black;
        private NotifyWindow notifyWindow;
        private const int ABM_GETTASKBARPOS = 5;
        private bool flatBorder;
        private Image glyph;

        private string text;
        private Color borderLightColor = Color.FromArgb(0xA6, 0xCA, 0xF0);
        private Color borderDarkColor = Color.Navy;

        [Description("Message text")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        [Description("Glyph image")]
        [Category("Appearance")]
        public Image Glyph
        {
            get { return glyph; }
            set { glyph = value; }
        }

        [Description("Border color for non-flat border")]
        [Category("Appearance")]
        public Color BorderDarkColor
        {
            get { return borderDarkColor; }
            set { borderDarkColor = value; }
        }

        [Description("Border color for non-flat border")]
        [Category("Appearance")]
        public Color BorderLightColor
        {
            get { return borderLightColor; }
            set { borderLightColor = value; }
        }

        [Description("Border style")]
        [Category("Appearance")]
        public bool FlatBorder
        {
            get { return flatBorder; }
            set { flatBorder = value; }
        }


        [Description("Notifier state")]
        public bool Active
        {
            get { return active; }
        }

        [DefaultValue(10)]
        [Category("Behaviour")]
        public int Precision
        {
            get { return precision; }
            set
            {
                if (value > 0)
                {
                    precision = value;
                }
            }
        }

        [Description("Notification window caption")]
        public string Caption
        {
            get { return caption; }
            set { caption = value; }
        }

        [Description("The background color of the component")]
        [Category("Appearance")]
        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        [Description("The font used to display the text and the caption")]
        [Category("Appearance")]
        public Font Font
        {
            get
            {
                if (font != null)
                {
                    return font;
                }
                else
                    return DefaultFont;
            }
            set { font = value; }
        }

        [DefaultValue(64)]
        [Description("Width and Height of the glyph image")]
        public int GlyphSize
        {
            get { return glyphSize; }
            set { glyphSize = value; }
        }

        [DefaultValue(400)]
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        [DefaultValue(100)]
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        [DefaultValue(500)]
        [Category("Behaviour")]
        public int ScrollTime
        {
            get { return scrollTime; }
            set { scrollTime = value; }
        }

        [DefaultValue(5000)]
        [Category("Behaviour")]
        public int ShowTime
        {
            get { return showTime; }
            set { showTime = value; }
        }

        [DefaultValue(true)]
        [Category("Behaviour")]
        public bool Sliding
        {
            get { return sliding; }
            set { sliding = value; }
        }

        [Description("the text color of the caption")]
        public Color CaptionColor
        {
            get { return captionColor; }
            set { captionColor = value; }
        }

        [Description("The text color of the message")]
        public Color TextColor
        {
            get { return textColor; }
            set { textColor = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private void InitializeWindow()
        {
            if (notifyWindow == null)
            {
                notifyWindow = new NotifyWindow();
                notifyWindow.Notifier = this;
                notifyWindow.Hide();
                notifyWindow.Click += new EventHandler(DoClick);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Notifier"/> class.
        /// </summary>
        public Notifier()
        {
            InitializeComponent();
            InitializeWindow();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Notifier"/> class.
        /// </summary>
        /// <param name="container">The container.</param>
        public Notifier(IContainer container)
        {
            if (container != null)
                container.Add(this);

            InitializeComponent();
            InitializeWindow();
        }

        public static Font DefaultFont
        {
            get
            {
                if (defaultFont == null)
                {
                    defaultFont = SystemFonts.DefaultFont;
                }
                return defaultFont;
            }
        }

        public void Show()
        {
            if (active)
            {
                return;
            }
            if (Activate != null)
                Activate(this, EventArgs.Empty);
            DoShow();
        }

        internal void DoClick(object sender, EventArgs e)
        {
            Uri uri;
            if (Click != null)
                Click(this, EventArgs.Empty);
            else
            {
                if (!string.IsNullOrEmpty(url))
                {
                    try
                    {
                        uri = new Uri(url);
                    }
                    catch (UriFormatException)
                    {
                        return;
                    }
                    try
                    {
                        Process.Start(uri.OriginalString);
                    }
                    catch (Win32Exception)
                    {
                        return;
                    }
                }
            }
        }

        #region Events
        public event EventHandler Click;
        public event EventHandler Activate;
        public event EventHandler Deactivate;
        #endregion

        public int AdjustedHeight { get; set; }
        public int AdjustedWidth { get; set; }

        public void DoShow()
        {
            if (precision == 0)
            {
                return;
            }

            int TimesToShow;

            TimesToShow = scrollTime / precision;
            if (TimesToShow == 0)
                return;

            notifyWindow.Text = this.Text;
            notifyWindow.StepSize = 254 / TimesToShow;

            Rectangle r;

            this.AdjustedHeight = (int)(this.Height * NativeMethods.DpiY() / 96);
            this.AdjustedWidth = (int)(this.Width * NativeMethods.DpiY() / 96);

            notifyWindow.BackColor = this.BackColor;
            notifyWindow.Height = this.AdjustedHeight;
            notifyWindow.Width = this.AdjustedWidth;
            notifyWindow.FlatBorder = this.FlatBorder;
            notifyWindow.BorderDarkPen.Color = this.BorderDarkColor;
            notifyWindow.BorderLightPen.Color = this.BorderLightColor;
            notifyWindow.DoBeforeShow();

            if ((Click != null) || (!string.IsNullOrEmpty(url)))
                notifyWindow.Cursor = Cursors.Hand;
            else
                notifyWindow.Cursor = Cursors.Default;


            r = Screen.GetWorkingArea(new Point(0, 0));


            notifyWindow.WARect = r;
            notifyWindow.Edge = GetEdge();

            switch (notifyWindow.Edge)
            {
                case TaskbarPosition.ScreenLeft:
                    {
                        if (sliding)
                            notifyWindow.Width = 1;
                        notifyWindow.Left = r.Left;
                        notifyWindow.Top = r.Bottom - this.AdjustedHeight;
                        notifyWindow.ScrollSpeed = AdjustedWidth / TimesToShow;
                        break;
                    }

                case TaskbarPosition.ScreenTop:
                    {
                        if (sliding)
                            notifyWindow.Height = 1;
                        notifyWindow.Left = r.Right - this.AdjustedWidth;
                        notifyWindow.Top = r.Top;
                        notifyWindow.ScrollSpeed = AdjustedHeight / TimesToShow;
                        break;
                    }

                case TaskbarPosition.ScreenBottom:
                    {
                        if (sliding)
                            notifyWindow.Height = 1;
                        notifyWindow.Left = r.Right - this.AdjustedWidth;
                        notifyWindow.Top = r.Bottom - notifyWindow.Height;
                        notifyWindow.ScrollSpeed = AdjustedHeight / TimesToShow;
                        break;
                    }

                case TaskbarPosition.ScreenRight:
                    {
                        if (sliding)
                            notifyWindow.Width = 1;
                        notifyWindow.Left = r.Right - notifyWindow.Width;
                        notifyWindow.Top = r.Bottom - this.AdjustedHeight;
                        notifyWindow.ScrollSpeed = AdjustedWidth / TimesToShow;
                        break;
                    }
            }

            notifyWindow.AlphaBlend = true;
            notifyWindow.AlphaBlendValue = 1;

            if (notifyWindow.ScrollSpeed > 0)
            {
                active = true;
                NativeMethods.ShowWindow(notifyWindow.Handle, (int)ShowWindowStyles.SW_SHOWNOACTIVATE);
                NativeMethods.SetWindowPos(notifyWindow.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, (int)SetWindowPosFlags.SWP_NOMOVE |
                        (int)SetWindowPosFlags.SWP_NOSIZE | (int)SetWindowPosFlags.SWP_NOACTIVATE);
                NativeMethods.SetTimer(new HandleRef(null, notifyWindow.Handle), new HandleRef(null, (IntPtr)NotifyTimers.Appearing), precision, new HandleRef(null, IntPtr.Zero));
            }
        }

        public void CloseUp()
        {
            if (active)
            {
                notifyWindow.KillTimers();
                NativeMethods.ShowWindow(notifyWindow.Handle, (int)ShowWindowStyles.SW_HIDE);
                DoDeactivate();
            }

        }

        internal static TaskbarPosition GetEdge()
        {
            TaskbarPosition result = TaskbarPosition.ScreenBottom;
            try
            {
                int returnCode = NativeMethods.GetTaskbarPosition();

                    if (returnCode == 0)
                        result = TaskbarPosition.ScreenLeft;
                    else
                        if (returnCode == 1)
                            result = TaskbarPosition.ScreenTop;
                        else
                            if (returnCode == 2)
                                result = TaskbarPosition.ScreenBottom;
                            else
                                result = TaskbarPosition.ScreenRight;
            }
            catch 
            {
                result = TaskbarPosition.ScreenBottom;
            }

            return result;
        }

        internal void DoDeactivate()
        {
            active = false;
            if (Deactivate != null)
                Deactivate(this, EventArgs.Empty);
        }
    }
}

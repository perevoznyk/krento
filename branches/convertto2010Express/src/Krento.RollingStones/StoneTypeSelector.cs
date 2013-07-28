using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Laugris.Sage;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Krento.RollingStones
{
    internal partial class StoneTypeSelector : AeroForm
    {
        private Image buttonBg = NativeThemeManager.Load("ButtonBG.png");
        private List<Image> icons = new List<Image>();

        public StoneTypeSelector()
        {
            InitializeComponent();
        }

        public void AddStandardIcons()
        {
            icons.Clear();
            icons.Add(NativeThemeManager.Load("SmallKrento.png", 32, 32));
            icons.Add(NativeThemeManager.Load("Launcher.png", 32, 32));
            icons.Add(NativeThemeManager.Load("DefaultRing.png", 32, 32));
            icons.Add(NativeThemeManager.Load("MyIP.png", 32, 32));
            icons.Add(NativeThemeManager.Load("MyDocuments.png", 32, 32));
            icons.Add(NativeThemeManager.Load("MyPictures.png", 32, 32));
            icons.Add(NativeThemeManager.Load("MyMusic.png", 32, 32));
            icons.Add(NativeThemeManager.Load("MyComputer.png", 32, 32));
            icons.Add(NativeThemeManager.Load("RecycleBin.png", 32, 32));

            icons.Add(NativeThemeManager.Load("Shutdown.png", 32, 32));
            icons.Add(NativeThemeManager.Load("Restart.png", 32, 32));
            icons.Add(NativeThemeManager.Load("Suspend.png", 32, 32));
            icons.Add(NativeThemeManager.Load("Hibernate.png", 32, 32));
            icons.Add(NativeThemeManager.Load("Timer.png", 32, 32));
            icons.Add(NativeThemeManager.Load("Calendar.png", 32, 32));
            icons.Add(NativeThemeManager.Load("CloseKrento.png", 32, 32));

            icons.Add(NativeThemeManager.Load("control.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppAddRemove.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppDateTime.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppPrinters.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppNetwork.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppFonts.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppBackground.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppAppearance.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppUsers.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppThemes.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppAccess.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppMouse.png", 32, 32));
            icons.Add(NativeThemeManager.Load("AppKeyboard.png", 32, 32));

            icons.Add(NativeThemeManager.Load("WebWiki.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebGoogle.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebGmail.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebTwitter.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebBlogger.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebFacebook.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebYoutube.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebWordpress.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebYahoo.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebReggit.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebFlickr.png", 32, 32));
            icons.Add(NativeThemeManager.Load("WebDelicious.png", 32, 32));

        }


        public void AddCustomIcon(string fileName)
        {
            bool found = false;
            if (!string.IsNullOrEmpty(fileName))
            {
                if (FileOperations.FileExists(fileName))
                    found = true;
            }

            if (found)
            {
                Bitmap tmp = FastBitmap.FromFile(fileName);
                icons.Add(BitmapPainter.ResizeBitmap(tmp, 32, 32, true));
            }
            else
                icons.Add(BitmapPainter.ResizeBitmap(NativeThemeManager.Load("SmallKrento.png"), 32, 32, true));
        }

        public void ClearIcons()
        {
            for (int i = 0; i < icons.Count; i++)
            {
                icons[i].Dispose();
            }
            icons.Clear();
        }

        private void StoneTypeSelector_Load(object sender, EventArgs e)
        {
            btnOK.Text = SR.OK;
            btnCancel.Text = SR.Cancel;
            this.Text = SR.SelectStoneType;

        }

        private void StonesSelector_DoubleClick(object sender, EventArgs e)
        {
            if (this.AcceptButton != null)
                this.AcceptButton.PerformClick();
        }

        private void StonesSelector_DrawItem(object sender, DrawItemEventArgs e)
        {
            StringFormat sr;

            using (Bitmap background = new Bitmap(e.Bounds.Width, e.Bounds.Height, PixelFormat.Format32bppPArgb))
            {
                using (Graphics g = Graphics.FromImage(background))
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = GlobalConfig.InterpolationMode;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;


                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                    //Brush backBrush;
                    if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    {
                        g.DrawImage(buttonBg, new Rectangle(0, 0, background.Width, background.Height));
                        //backBrush = SystemBrushes.Highlight;
                        //g.FillRectangle(backBrush, new Rectangle(0, 0, background.Width, background.Height));
                    }
                    else
                        using (Brush b = new SolidBrush(e.BackColor))
                        {

                            g.FillRectangle(b, new Rectangle(0, 0, background.Width, background.Height));
                        }

                    if (e.Index > -1)
                    {
                        sr = StringFormat.GenericDefault;
                        sr.LineAlignment = StringAlignment.Center;
                        
                        Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected ? Brushes.Black : SystemBrushes.WindowText);
                        Font headerFont = new Font(SystemFonts.CaptionFont.FontFamily, SystemFonts.CaptionFont.Size, FontStyle.Regular, GraphicsUnit.Point);

                        int idx;
                        if (e.Index +1 >= icons.Count)
                            idx = 0;
                        else
                            idx = e.Index + 1;

                        g.DrawImage(icons[idx], 4, 4, 32, 32);
                        g.DrawString((string)StonesSelector.Items[e.Index] , headerFont, brush,
                            new Rectangle(40, 0, background.Width - 40, background.Height), sr);

                        sr.Dispose();

                        headerFont.Dispose();
                    }
                }

                e.Graphics.DrawImage(background, e.Bounds);
            }

            //e.DrawFocusRectangle();

        }

        private void StoneTypeSelector_Shown(object sender, EventArgs e)
        {
            if (StonesSelector.CanFocus)
                StonesSelector.Focus();
        }
    }
}

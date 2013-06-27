using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Krento.RollingStones;
using System.Drawing.Drawing2D;
using Laugris.Sage;
using System.Drawing.Imaging;

namespace Krento.RollingStones
{
    public partial class LoadRingDialog : AeroForm
    {
        private Image buttonBg = NativeThemeManager.Load("ButtonBG.png");
        private string currentCircle;
        private StonesManager manager;

        public LoadRingDialog()
        {
            InitializeComponent();
        }

        public LoadRingDialog(StonesManager manager)
        {
            InitializeComponent();
            this.manager = manager;
        }



        public string CurrentCircle
        {
            get { return currentCircle; }
            set { currentCircle = value; }
        }


        public void FillList()
        {
            KrentoRing ring;
            string[] rings = Directory.GetFiles(GlobalConfig.RollingStonesFolder, "*.circle");
            foreach (string fileName in rings)
            {
                ring = new KrentoRing(fileName);
                lstRings.Items.Add(ring);
            }
        }

        public void ClearList()
        {
            for (int i = 0; i < lstRings.Items.Count; i++)
            {
                ((KrentoRing)lstRings.Items[i]).Dispose();
            }

            lstRings.Items.Clear();
        }

        public string FileName
        {
            get
            {
                if (lstRings.SelectedIndex >= 0)
                    return ((KrentoRing)lstRings.Items[lstRings.SelectedIndex]).FileName;
                else
                    return string.Empty;
            }
        }

        private void lstRings_DrawItem(object sender, DrawItemEventArgs e)
        {
            StringFormat sr;

            //e.DrawBackground();

            //e.Graphics.FillRectangle(backBrush, e.Bounds);


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
                       // g.FillRectangle(backBrush, new Rectangle(0, 0, background.Width, background.Height));
                    }
                    else
                        using (Brush b = new SolidBrush(e.BackColor))
                        {

                            g.FillRectangle(b, new Rectangle(0, 0, background.Width, background.Height));
                        }

                    if (e.Index > -1)
                    {
                        sr = StringFormat.GenericDefault;

                        KrentoRing ring = (KrentoRing)lstRings.Items[e.Index];
                        Brush brush = ((e.State & DrawItemState.Selected) == DrawItemState.Selected ? Brushes.Black : SystemBrushes.WindowText);
                        Font headerFont = new Font(SystemFonts.DialogFont.FontFamily, 12, FontStyle.Bold, GraphicsUnit.Pixel);
                        g.DrawImage(ring.Logo, new Rectangle(2, 2, 48, 48));
                        g.DrawString(ring.Caption, headerFont, brush,
                            new Rectangle(52, 2, background.Width - 52, (int) Math.Ceiling((double)headerFont.GetHeight(g)) + 4), sr);


                        sr.Trimming = StringTrimming.EllipsisWord;
                        sr.FormatFlags = StringFormatFlags.LineLimit;

                        string desc = ring.Description;
                        if (string.IsNullOrEmpty(desc))
                            desc = SR.CircleDefaultDescription;
                        else
                            desc = desc.Replace(Environment.NewLine, " ");

                        g.DrawString(desc, e.Font, brush,
                            new Rectangle(52, 6 + headerFont.Height, background.Width - 52, background.Height - (int) Math.Ceiling((double)headerFont.GetHeight(g)) - 8), sr);
                        sr.Dispose();

                        headerFont.Dispose();
                    }
                }

                e.Graphics.DrawImage(background, e.Bounds);
            }

          //  e.DrawFocusRectangle();
        }

        private void lstRings_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void LoadRingDialog_Shown(object sender, EventArgs e)
        {
            int idx = -1;

            for (int i = 0; i < lstRings.Items.Count; i++)
            {
                if (TextHelper.SameText(((KrentoRing)lstRings.Items[i]).FileName, CurrentCircle))
                {
                    idx = i;
                    break;
                }
            }

            if (idx >= 0)
                lstRings.SelectedIndex = idx;
        }

        private void LoadRingDialog_Load(object sender, EventArgs e)
        {
            this.Text = SR.SelectCircle;
            btnCancel.Text = SR.Cancel;
            btnOK.Text = SR.OK;
        }


    }
}

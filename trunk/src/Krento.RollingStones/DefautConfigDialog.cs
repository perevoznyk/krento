using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public partial class DefautConfigDialog : Krento.RollingStones.StoneRingConfig
    {
        private Bitmap defaultImage;
        private string resourceName;
        private RollingStoneTask stone;

        public DefautConfigDialog(RollingStoneTask stone)
        {
            this.stone = stone;
            InitializeComponent();
        }

        public Bitmap DefaultImage
        {
            get { return defaultImage; }
            set { defaultImage = value; }
        }

        public string ResourceName
        {
            get { return resourceName; }
            set { resourceName = value; }
        }

        protected override void AssignDefaultImage()
        {
            Image tmp = null;
            EmptyCustomIcon();
            if (!string.IsNullOrEmpty(resourceName))
                tmp = NativeThemeManager.LoadBitmap(resourceName);
            else
                if (defaultImage != null)
                    tmp = BitmapPainter.ConvertToRealColors(defaultImage, false);
                else
                    tmp = NativeThemeManager.Load("UnknownFile.png");
            if (tmp != null)
            {
                if (imgLogo.Image != null)
                {
                    imgLogo.Image.Dispose();
                    imgLogo.Image = null;
                }
                imgLogo.Image = BitmapPainter.ConvertToRealColors(tmp, false);
                tmp.Dispose();
                tmp = null;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            AssignDefaultImage();
        }
    }
}

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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public partial class StoneRingConfig : AeroForm
    {
        private string customIcon;
        private string fileName;
        private StonesManager manager;

        public StoneRingConfig()
        {
            InitializeComponent();
        }

        public StoneRingConfig(StonesManager manager)
        {
            InitializeComponent();
            this.manager = manager;
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = FileOperations.StripFileName(value);
                KrentoRing ring = new KrentoRing(fileName);
                edtTarget.Text = ring.Caption;
                ring.Dispose();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            LoadRingDialog loadRing = new LoadRingDialog(manager);
            try
            {
                loadRing.FillList();
                if (loadRing.ShowDialog() == DialogResult.OK)
                {

                    FileName = loadRing.FileName;
                    UpdateTarget();
                }
                loadRing.ClearList();
            }
            finally
            {
                loadRing.Dispose();
            }
        }

        private void UpdateTarget()
        {
            KrentoRing ring = new KrentoRing(fileName);
            edtTarget.Text = ring.Caption;
            if (!string.IsNullOrEmpty(ring.Description))
                edtDescription.Text = ring.Description;
            else
                edtDescription.Text = ring.Caption;
            if (!string.IsNullOrEmpty(ring.LogoFile))
            {
                if (FileOperations.FileExists(ring.LogoFile))
                {
                    if (imgLogo.Image != null)
                        imgLogo.Image.Dispose();
                    imgLogo.Image = FastBitmap.FromFile(FileOperations.StripFileName(ring.LogoFile));
                    customIcon = ring.LogoFile;
                }
                else
                    AssignDefaultImage();
            }
            else
                AssignDefaultImage();
            ring.Dispose();
        }

        /// <summary>
        /// Gets or sets the target file name or url.
        /// </summary>
        /// <value>The target.</value>
        public string Target
        {
            get { return edtTarget.Text; }
            set { edtTarget.Text = value; }
        }

        /// <summary>
        /// Gets or sets the description of the target.
        /// </summary>
        /// <value>The description of the target.</value>
        public string Description
        {
            get { return edtDescription.Text; }
            set { edtDescription.Text = value; }
        }

        /// <summary>
        /// Gets or sets the custom icon.
        /// </summary>
        /// <value>The custom icon.</value>
        public string CustomIcon
        {
            get { return customIcon; }
            set { customIcon = value; UpdateTargetIcon(); }
        }

        protected void EmptyCustomIcon()
        {
            customIcon = null;
        }

        /// <summary>
        /// Updates the target icon if the custom icon file name is provided
        /// </summary>
        private void UpdateTargetIcon()
        {
            if (!string.IsNullOrEmpty(customIcon))
            {
                string fullCustomIcon = FileOperations.StripFileName(customIcon);
                if (FileOperations.FileExists(fullCustomIcon))
                {
                    Bitmap tmp = FastBitmap.FromFile(fullCustomIcon);
                    if (tmp != null)
                    {
                        if (imgLogo.Image != null)
                        {
                            imgLogo.Image.Dispose();
                            imgLogo.Image = null;
                        }
                        imgLogo.Image = new Bitmap(tmp);
                        tmp.Dispose();
                    }
                    else
                        AssignDefaultImage();
                }
            }
            else
                AssignDefaultImage();
        }

        public Image Image
        {
            get { return imgLogo.Image; }
            set { imgLogo.Image = value; }
        }

        private void btnSelectImage_Click(object sender, EventArgs e)
        {
            if (openIconDialog.ShowDialog() == DialogResult.OK)
            {
                imgLogo.Image = FastBitmap.FromFile(openIconDialog.FileName);
                customIcon = openIconDialog.FileName;
            }
        }

        private void StoneRingConfig_Load(object sender, EventArgs e)
        {
            this.Text = SR.FileConfigTitle;

            toolTip.SetToolTip(lblTarget, SR.FileConfigTargetHint);
            toolTip.SetToolTip(lblDescription, SR.FileConfigDescriptionHint);

            toolTip.SetToolTip(edtTarget, SR.FileConfigTargetHint);
            toolTip.SetToolTip(edtDescription, SR.FileConfigDescriptionHint);

            toolTip.SetToolTip(btnBrowse, SR.FileConfigBrowseHint);
            toolTip.SetToolTip(btnOK, SR.FileConfigOKHint);
            toolTip.SetToolTip(btnReset, SR.FileConfigDefaultHint);
            toolTip.SetToolTip(btnSelectImage, SR.FileConfigSelectHint);
            toolTip.SetToolTip(btnCancel, SR.FileConfigCancelHint);

            lblTarget.Text = SR.TargetCircle;
            lblDescription.Text = SR.FileConfigDescription;

            btnOK.Text = SR.OK;
            btnCancel.Text = SR.Cancel;
            btnBrowse.Text = SR.Browse;
            btnSelectImage.Text = SR.FileConfigSelect;
            btnReset.Text = SR.FileConfigDefault;
            try
            {
                openIconDialog.Filter = SR.ImageFilesFilter;
            }
            catch
            {
                openIconDialog.Filter = @"Image Files (*.PNG;*.BMP;*.JPG;*.GIF;*.ICO)|*.PNG;*.BMP;*.JPG;*.GIF;*.ICO";
            }
        }

        protected virtual void AssignDefaultImage()
        {
            Image tmp = null;
            customIcon = null;
            tmp = BitmapPainter.ConvertToRealColors(KrentoRing.DefaultRingImage, false);
            if (tmp != null)
            {
                if (imgLogo.Image != null)
                {
                    imgLogo.Image.Dispose();
                    imgLogo.Image = null;
                }
                imgLogo.Image = BitmapPainter.ConvertToRealColors(tmp, true);
            }
        }

        public void DisposeLogoImage()
        {
            if (imgLogo.Image != null)
            {
                imgLogo.Image.Dispose();
                imgLogo.Image = null;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(edtTarget.Text))
                return;
            AssignDefaultImage();
        }
    }
}

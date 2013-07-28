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
    public partial class StoneFileConfig : AeroForm
    {
        private string customIcon;

        public StoneFileConfig()
        {
            InitializeComponent();
        }


        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                edtTarget.Text = FileOperations.UnExpandPath(openFileDialog.FileName);
                UpdateTarget();
                AssignDefaultIcon();
            }
        }

        private void UpdateTarget()
        {
            string fullName = FileOperations.StripFileName(edtTarget.Text);
            edtDescription.Text = Path.GetFileNameWithoutExtension(fullName);
            if (FileOperations.IsURL(edtTarget.Text))
            {
                if (string.IsNullOrEmpty(edtDescription.Text))
                    edtDescription.Text = FileOperations.GetPageTitle(edtTarget.Text);
            }
            else
            {
                if (string.IsNullOrEmpty(edtDescription.Text))
                    edtDescription.Text = fullName;
            }
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
        /// Gets or sets the argument. This is a substitute part of the command line
        /// </summary>
        /// <value>The argument.</value>
        public string Argument
        {
            get { return edtArgument.Text; }
            set { edtArgument.Text = value; }

        }

        /// <summary>
        /// Gets or sets the command line.
        /// </summary>
        /// <value>The command line.</value>
        public string CommandLine
        {
            get { return edtParameters.Text; }
            set { edtParameters.Text = value; }
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
                    if (imgLogo.Image != null)
                    {
                        imgLogo.Image.Dispose();
                        imgLogo.Image = null;
                    }
                    imgLogo.Image = BitmapPainter.ConvertToRealColors(tmp, true);
                }
            }
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
                customIcon = FileOperations.UnExpandPath(openIconDialog.FileName);
            }
        }

        private void StoneFileConfig_Load(object sender, EventArgs e)
        {
            this.Text = SR.FileConfigTitle;

            toolTip.SetToolTip(lblTarget, SR.FileConfigTargetHint);
            toolTip.SetToolTip(lblDescription, SR.FileConfigDescriptionHint);
            toolTip.SetToolTip(lblParameters, SR.FileConfigCommandLineHint);
            toolTip.SetToolTip(lblArgument, SR.FileConfigArgumentHint);

            toolTip.SetToolTip(edtTarget, SR.FileConfigTargetHint);
            toolTip.SetToolTip(edtDescription, SR.FileConfigDescriptionHint);
            toolTip.SetToolTip(edtParameters, SR.FileConfigCommandLineHint);
            toolTip.SetToolTip(edtArgument, SR.FileConfigArgumentHint);

            toolTip.SetToolTip(btnBrowse, SR.FileConfigBrowseHint);
            toolTip.SetToolTip(btnFolder, SR.FileConfigBrowseHint);
            toolTip.SetToolTip(btnOK, SR.FileConfigOKHint);
            toolTip.SetToolTip(btnReset, SR.FileConfigDefaultHint);
            toolTip.SetToolTip(btnSelectImage, SR.FileConfigSelectHint);
            toolTip.SetToolTip(btnCancel, SR.FileConfigCancelHint);

            lblTarget.Text = SR.FileConfigTarget;
            lblDescription.Text = SR.FileConfigDescription;
            lblParameters.Text = SR.FileConfigCommandLine;
            lblArgument.Text = SR.FileConfigArgument;

            btnOK.Text = SR.OK;
            btnCancel.Text = SR.Cancel;
            btnBrowse.Text = SR.Browse;
            btnSelectImage.Text = SR.FileConfigSelect;
            btnReset.Text = SR.FileConfigDefault;

            openIconDialog.InitialDirectory = GlobalConfig.IconsFolder;
            try
            {
                openIconDialog.Filter = SR.ImageFilesFilter;
            }
            catch
            {
                openIconDialog.Filter = @"Image Files (*.PNG;*.BMP;*.JPG;*.GIF;*.ICO)|*.PNG;*.BMP;*.JPG;*.GIF;*.ICO";
            }

            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            try
            {
                openFileDialog.Filter = SR.ApplicationsFilter;
            }
            catch
            {
                openFileDialog.Filter = @"Applications|*.exe|All files|*.*";
            }

            btnFolder.Text = SR.FileConfigSelect;
        }

        private void AssignDefaultIcon()
        {
            Image tmp = null;

            if (string.IsNullOrEmpty(edtTarget.Text))
                return;

            customIcon = null;
            tmp = FileOperations.GetFileLogo(edtTarget.Text);
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

        private void btnReset_Click(object sender, EventArgs e)
        {
            AssignDefaultIcon();
        }

        private void edtTarget_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(edtTarget.Text))
                btnOK.Enabled = false;
            else
                btnOK.Enabled = true;
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                edtTarget.Text = FileOperations.UnExpandPath(folderBrowserDialog.SelectedPath);
                UpdateTarget();
                AssignDefaultIcon();
            }

        }
    }
}

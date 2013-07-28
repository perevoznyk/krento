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
    public partial class RingSettingsDialog : AeroForm
    {
        private string fileName;
        private KrentoRing ring;
        private string logoFile;

        public RingSettingsDialog()
        {
            InitializeComponent();
        }

        public bool DefaultRing
        {
            get { return edtDefaultRing.Checked; }
            set { edtDefaultRing.Checked = value; }
        }

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(edtName.Text.Trim()))
                    fileName = string.Empty;
                else
                    if (FileOperations.IsValidFileName(edtName.Text.Trim()))
                        fileName = Path.Combine(GlobalConfig.RollingStonesFolder, edtName.Text.Trim() + ".circle");
                    else
                    {
                        string attempt = edtName.Text.Trim();
                        char[] invalid = Path.GetInvalidFileNameChars();
                        StringBuilder sb = new StringBuilder();
                        bool ok;
                        for (int i = 0; i < attempt.Length; i++)
                        {
                            ok = true;
                            for (int j = 0; j < invalid.Length; j++)
                            {
                                if (attempt[i] == invalid[j])
                                {
                                    ok = false;
                                    break;
                                }

                            }

                            if (ok)
                                sb.Append(attempt[i]);
                        }

                        fileName = sb.ToString();
                        if (FileOperations.IsValidFileName(fileName))
                            fileName = Path.Combine(GlobalConfig.RollingStonesFolder, fileName + ".circle");
                        else
                            fileName = string.Empty;
                    }
                return fileName;
            }
            set
            {
                fileName = value;
                ring = new KrentoRing(fileName);
                try
                {
                    edtDescription.Text = ring.Description;
                    edtName.Text = ring.Name;
                    if (imgLogo.Image != null)
                        imgLogo.Image.Dispose();
                    imgLogo.Image = BitmapPainter.ConvertToRealColors(ring.Logo, false);
                }
                finally
                {
                    ring.Dispose();
                    ring = null;
                }
            }
        }


        private void LoadLogo()
        {
            string fullLogoFile = FileOperations.StripFileName(logoFile);
            if (FileOperations.FileExists(fullLogoFile))
            {
                if (imgLogo.Image != null)
                    imgLogo.Image.Dispose();
                Bitmap tmp = FastBitmap.FromFile(fullLogoFile);
                imgLogo.Image = BitmapPainter.ConvertToRealColors(tmp, false);
                tmp.Dispose();
            }
        }

        public string Description
        {
            get
            {
                string desc = edtDescription.Text;
                desc = desc.Replace(Environment.NewLine, @"\n");
                return desc;
            }
        }

        public string LogoFile
        {
            get { return logoFile; }
        }

        public void Save()
        {
            ring = new KrentoRing(FileName);
            try
            {
                ring.Description = edtDescription.Text;
                ring.LogoFile = logoFile;
                ring.Save();
            }
            finally
            {
                ring.Dispose();
                ring = null;
            }
        }

        private void btlLoad_Click(object sender, EventArgs e)
        {
            if (openIconDialog.ShowDialog() == DialogResult.OK)
            {
                logoFile = FileOperations.UnExpandPath(openIconDialog.FileName);
                LoadLogo();
            }
        }

        private void RingSettingsDialog_Load(object sender, EventArgs e)
        {
            btnCancel.Text = SR.Cancel;
            btnOK.Text = SR.OK;
            this.Text = SR.Settings;
            lblDescription.Text = SR.Description;
            lblName.Text = SR.Name;
            btlLoad.Text = SR.Load;
            openIconDialog.InitialDirectory = GlobalConfig.RingIconsFolder;
            try
            {
                openIconDialog.Filter = SR.ImageFilesFilter;
            }
            catch
            {
                openIconDialog.Filter = @"Image Files (*.PNG;*.BMP;*.JPG;*.GIF;*.ICO)|*.PNG;*.BMP;*.JPG;*.GIF;*.ICO";
            }
            edtDefaultRing.Text = SR.FileConfigDefault;
            btnOK.Enabled = (!string.IsNullOrEmpty(edtName.Text));
            ActiveControl = edtName;
        }

        private void edtName_TextChanged_1(object sender, EventArgs e)
        {
            btnOK.Enabled = (!string.IsNullOrEmpty(edtName.Text));
        }


    }
}

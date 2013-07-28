using System;
using System.Drawing;
using System.IO;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public class KrentoRing : FileItem
    {
        private static Bitmap defaultRingImage;
        private string logoFile;
        MemIniFile iniFile;

        public KrentoRing(string fileName)
            : base(fileName)
        {
            if (FileOperations.FileExists(fileName))
            {
                Load();
            }
            else
                LoadDefaultLogo();

        }

        public KrentoRing(MemIniFile iniFile)
        {
            this.iniFile = iniFile;
            FileName = FileOperations.StripFileName(iniFile.FileName);
            LoadIniFileData();
            LoadLogo(logoFile);
        }

        private void LoadIniFileData()
        {
            Description = iniFile.ReadString("Settings", "Description", string.Empty);
            Description = Description.Replace(@"\n", Environment.NewLine);
            logoFile = FileOperations.StripFileName(iniFile.ReadString("Settings", "Logo", string.Empty));
            TranslationId = iniFile.ReadString("Settings", "TranslationId", string.Empty);

            if (!string.IsNullOrEmpty(TranslationId))
                Caption = SR.Keys.GetString(TranslationId);
            else
                Caption = string.Empty;

            if (string.IsNullOrEmpty(Caption))
            {
                if (TextHelper.SameText(GlobalConfig.HomeCircleName, FileName))
                    Caption = SR.DefaultRingName;
                else
                    Caption = Path.GetFileNameWithoutExtension(FileName);
            }
        }

        private void Load()
        {
            try
            {
                iniFile = new MemIniFile(FileName);
                try
                {
                    iniFile.Load();
                    LoadIniFileData();
                }
                finally
                {
                    iniFile.Dispose();
                    iniFile = null;
                }

                LoadLogo(logoFile);
            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex);
            }
        }

        private void LoadLogo(string name)
        {
            if (string.IsNullOrEmpty(name))
                LoadDefaultLogo();
            else
            {
                if (Logo != null)
                {
                    Logo.Dispose();
                    Logo = null;
                }

                string fullName = FileOperations.StripFileName(name);
                if (FileOperations.FileExists(fullName))
                {
                    Bitmap tmp = FastBitmap.FromFile(fullName);
                    if (tmp != null)
                    {
                        Logo = BitmapPainter.ConvertToRealColors(tmp, false);
                        tmp.Dispose();
                    }
                }

                if (Logo == null)
                    LoadDefaultLogo();
            }
        }

        private void LoadDefaultLogo()
        {
            if (Logo != null)
                Logo.Dispose();
            Logo = BitmapPainter.ConvertToRealColors(DefaultRingImage, false);
        }

        public string TranslationId { get; set; }

        /// <summary>
        /// Gets the default ring image. Use Clone() if you want to dispose your bitmap in the future
        /// or you will dispose original bitmap
        /// </summary>
        /// <value>The default ring image.</value>
        public static Bitmap DefaultRingImage
        {
            get
            {
                if (defaultRingImage == null)
                    defaultRingImage = NativeThemeManager.LoadBitmap("DefaultRing.png");
                return defaultRingImage;
            }
        }


        public string LogoFile
        {
            get { return logoFile; }
            set
            {
                logoFile = value;
                LoadLogo(logoFile);
            }
        }

        public void Save()
        {
            if (iniFile == null)
            {
                iniFile = new MemIniFile(FileName);
                try
                {
                    iniFile.Load();
                    iniFile.WriteString("Settings", "Description", Description);
                    iniFile.WriteString("Settings", "Logo", logoFile);
                    iniFile.Save();
                }
                finally
                {
                    iniFile.Dispose();
                }
            }
            else
            {
                iniFile.WriteString("Settings", "Description", Description);
                iniFile.WriteString("Settings", "Logo", logoFile);
                iniFile.Save();
            }
        }


        public static void DisposeDefaultRingImage()
        {
            if (defaultRingImage != null)
            {
                defaultRingImage.Dispose();
                defaultRingImage = null;
            }
        }

    }
}

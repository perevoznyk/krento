using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Reflection;

namespace Krento.RollingStones
{
    public abstract class RollingStoneTask : RollingStoneBase
    {
        private Bitmap logo;
        private Bitmap reflection;

        private string customIcon;
        private string resourceName;
        private string cacheLogoName;

        protected RollingStoneTask(StonesManager manager)
            : base(manager)
        {
            try
            {
                this.ResourceName = "UnknownFile.png";
                this.ConfigureStone += new EventHandler(RollingStoneTask_ConfigureStone);
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }

        public string ResourceName
        {
            get { return resourceName; }
            set { resourceName = value; }
        }

        public string CustomIcon
        {
            get
            {
                return customIcon;
            }
            set
            {
                if (FileOperations.FileExists(value))
                    customIcon = value;
                else
                    customIcon = null;
            }
        }

        public bool AnimatedLogo { get; set; }

        protected void HandleAnimation(object sender, EventArgs e)
        {
            if (AnimatedLogo)
            {
                Update(true);
                UpdateFrames();
            }
        }


        protected override void WindowShown()
        {
            if (logo != null)
            {
                if (AnimatedLogo)
                {
                    ImageAnimator.Animate(logo, HandleAnimation);
                }
            }
        }

        protected void StopAnimation()
        {
            if (logo != null)
            {
                if (AnimatedLogo)
                {
                    ImageAnimator.StopAnimate(logo, HandleAnimation);
                }
            }
        }

        protected override void WindowHides()
        {
            StopAnimation();
        }

        protected internal virtual void ExecuteConfigurationCall()
        {

            Manager.SuppressHookMessage(true);
            try
            {
                DefautConfigDialog config = new DefautConfigDialog(this);
                try
                {
                    config.ResourceName = resourceName;
                    config.DefaultImage = Logo;
                    config.Description = this.TargetDescription;
                    config.CustomIcon = this.CustomIcon;
                    if (config.ShowDialog() == DialogResult.OK)
                    {
                        if ( (!string.IsNullOrEmpty(config.Description)) || (!string.IsNullOrEmpty(TranslationId)))
                            this.TargetDescription = config.Description;
                        
                        this.CustomIcon = config.CustomIcon;
                        DestroyLogoImage();
                        FixupConfiguration();
                        Update(true);
                        Manager.RedrawScreenHint();
                        Manager.FlushCurrentCircle();
                    }
                    config.DisposeLogoImage();
                }
                finally
                {
                    config.Dispose();
                }
            }
            finally
            {
                Manager.SuppressHookMessage(false);
            }
        }

        public void DestroyLogoImage()
        {
            DeleteCacheIcon();
            if (Logo != null)
            {
                Logo.Dispose();
                Logo = null;
            }
        }

        void RollingStoneTask_ConfigureStone(object sender, EventArgs e)
        {
            Manager.HideAndExecute(ExecuteConfigurationCall);
        }


        public override void ReadConfiguration(MemIniFile ini)
        {
            if (!IsVirtual)
            {
                string stringValue;

                base.ReadConfiguration(ini);
                try
                {
                    stringValue = ini.ReadString(this.StoneID, "CustomIcon", null);
                    if (!string.IsNullOrEmpty(stringValue))
                    {
                        CustomIcon = FileOperations.StripFileName(stringValue);
                    }
                }
                catch (Exception ex)
                {
                    throw new StoneSettingsException("Read stone settings error", ex);
                }
            }
        }

        public override void SaveConfiguration(MemIniFile ini)
        {
            if (!IsVirtual)
            {
                base.SaveConfiguration(ini);
                if (!string.IsNullOrEmpty(TargetDescription))
                    ini.WriteString(this.StoneID, "Description", TargetDescription);
                if (!string.IsNullOrEmpty(CustomIcon))
                {
                    ini.WriteString(this.StoneID, "CustomIcon", FileOperations.UnExpandPath(CustomIcon));
                }
            }
        }

        public override void FixupConfiguration()
        {
            try
            {

                if (!string.IsNullOrEmpty(TranslatedDescription))
                    Caption = TranslatedDescription;
                else
                    Caption = TargetDescription;

                FixupLogoImage();
            }
            catch (Exception ex)
            {
                throw new StoneSettingsException("Stone settings parse error", ex);
            }
        }


        public void FixupLogoImage()
        {
            StopAnimation();
            FixupCacheLogo();
            if (logo == null)
                FixupCustomLogo();
            if (logo == null)
                FixupTargetLogo();
            FixupResourceLogo();
        }

        /// <summary>
        /// Deletes the cache icon.
        /// </summary>
        protected void DeleteCacheIcon()
        {
            FileOperations.DeleteFile(CacheLogoName);
        }

        public Bitmap Logo
        {
            get { return logo; }
            set
            {
                logo = value;
                if (reflection != null)
                {
                    reflection.Dispose();
                    reflection = null;
                }
                if (logo != null)
                {
                    if (!AnimatedLogo)
                        reflection = (Bitmap)BitmapPainter.CreateReflection(logo, FileImage.ImageSize / 2);
                }
            }
        }

        public Bitmap Reflection
        {
            get { return reflection; }
        }

        protected void FixupCacheLogo()
        {
            //Reload image from cache
            Bitmap cacheOriginal;
            if (FileOperations.FileExists(CacheLogoName))
            {
                if (Logo != null)
                {
                    Logo.Dispose();
                    Logo = null;
                }

                cacheOriginal = FastBitmap.FromFile(CacheLogoName);
                if (cacheOriginal != null)
                {
                    if (cacheOriginal.Width == FileImage.ImageSize)
                    {
                        Logo = BitmapPainter.ConvertToRealColors(cacheOriginal, true);
                        AnimatedLogo = false;
                    }
                }
            }
        }

        protected void FixupCustomLogo()
        {
            StopAnimation();

            if (string.IsNullOrEmpty(customIcon))
                return;

            Bitmap newImage = FastBitmap.FromFile(customIcon);
            if (newImage == null)
                return;

            if (Logo != null)
            {
                Logo.Dispose();
                Logo = null;
            }

            AnimatedLogo = NativeMethods.IsAnimatedGIF(customIcon);
            if (AnimatedLogo)
                Logo = newImage;
            else
                Logo = BitmapPainter.ResizeBitmap(newImage, FileImage.ImageSize, FileImage.ImageSize, true);
            SaveLogoToCache();
        }

        protected internal void FixupResourceLogo()
        {
            if (Logo == null)
            {
                Bitmap tmp = NativeThemeManager.LoadBitmap(ResourceName);
                Logo = BitmapPainter.ResizeBitmap(tmp, FileImage.ImageSize, FileImage.ImageSize);
                tmp.Dispose();
                AnimatedLogo = false;
                SaveLogoToCache();
            }
        }

        protected virtual void FixupTargetLogo()
        {
        }

        /// <summary>
        /// Saves the logo to cache (only in case if logo is not an animated image).
        /// </summary>
        protected void SaveLogoToCache()
        {
            DeleteCacheIcon();
            if (!IsVirtual)
            {
                if (Manager.IsVirtual)
                    return;
                if (Logo != null)
                {
                    if (!AnimatedLogo)
                        if (!string.IsNullOrEmpty(StoneID))
                            Logo.Save(CacheLogoName, ImageFormat.Png);
                }
            }

        }

        protected string CacheLogoName
        {
            get
            {
                if (string.IsNullOrEmpty(StoneID))
                    return null;
                else
                {
                    if (string.IsNullOrEmpty(cacheLogoName))
                    {
                        cacheLogoName = Path.Combine(GlobalConfig.RollingStonesCache, StoneID) + ".png";
                    }
                    return cacheLogoName;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                StopAnimation();
                if (logo != null)
                {
                    logo.Dispose();
                    logo = null;
                }

                if (reflection != null)
                {
                    reflection.Dispose();
                    reflection = null;
                }
            }
            base.Dispose(disposing);
        }


        public override void Paint(Graphics canvas, Rectangle workingArea, PaintMode paintMode)
        {
            if (canvas == null)
                return;

            base.Paint(canvas, workingArea, paintMode);

            if (paintMode == PaintMode.Stone)
            {
                if (logo != null)
                {
                    int ImageSize = (int)(FileImage.ImageSize / MaxScale);
                    Rectangle R = new Rectangle(0, 0, ImageSize, ImageSize);
                    R.Offset((workingArea.Width - ImageSize) / 2, (workingArea.Height - ImageSize) / 2);
                    if (canvas != null)
                    {
                        if (Manager.LiveReflection)
                        {
                            if (reflection != null)
                            {
                                Rectangle M = new Rectangle(0, 0, ImageSize, ImageSize / 2);
                                M.Offset((workingArea.Width - ImageSize) / 2, R.Bottom - 4);
                                M.Inflate(-4, -4);
                                canvas.DrawImage(reflection, M);
                            }
                        }
                        canvas.DrawImage(logo, R);
                    }
                }
            }
            else
            {
                DrawTargetDescription();
            }


        }
    }
}

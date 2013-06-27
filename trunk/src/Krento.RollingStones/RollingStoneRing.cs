using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Krento.RollingStones
{
    public class RollingStoneRing : RollingStoneTask
    {
        private string targetName;

        public RollingStoneRing(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "DefaultRing.png";
                TargetDescription = null;
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }

        public override void DrawTargetDescription()
        {
            if (string.IsNullOrEmpty(TargetDescription))
            {
                Manager.DrawText(SR.StoneRing);
                MoveStoneHint();
            }
            else
                base.DrawTargetDescription();
        }

        protected internal override void ExecuteConfigurationCall()
        {
            Manager.SuppressHookMessage(true);
            try
            {
                StoneRingConfig config = new StoneRingConfig(Manager);
                try
                {
                    config.FileName = this.targetName;
                    config.Description = this.TargetDescription;

                    config.CustomIcon = this.CustomIcon;
                    if (config.ShowDialog() == DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(config.Target))
                        {
                            this.targetName = FileOperations.UnExpandPath(config.FileName);

                            string newDescription = config.Description;
                            if (string.IsNullOrEmpty(newDescription))
                            {
                                if (string.IsNullOrEmpty(TranslationId))
                                    newDescription = FileOperations.GetFileDescription(targetName);
                            }
                            this.TargetDescription = newDescription;

                            this.CustomIcon = config.CustomIcon;

                            DestroyLogoImage();
                            FixupConfiguration();
                            Update(true);
                            Manager.RedrawScreenHint();
                        }

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

        /// <summary>
        /// Load selected ring
        /// </summary>
        public override void Run()
        {
            if (!string.IsNullOrEmpty(targetName))
            {
                if (!IsVirtual)
                    Manager.ExecutingCircle = Manager.CurrentCircle;
                Manager.ReplaceCurrentCircle(targetName);
            }
            else
                Configure();
            //base.Run();
        }


        private static Bitmap GetLogoFromRing(string ringName)
        {
            if (!FileOperations.FileExists(ringName))
                return null;

            Bitmap result = null;

            KrentoRing ring = new KrentoRing(ringName);
            try
            {
                result = BitmapPainter.ConvertToRealColors(ring.Logo, false);
            }
            finally
            {
                ring.Dispose();
                ring = null;
            }
            return result;
        }


        protected override void FixupTargetLogo()
        {
            if (Logo != null)
            {
                Logo.Dispose();
                Logo = null;
            }

            if (!string.IsNullOrEmpty(targetName))
            {
                if (NativeMethods.FileExtensionIs(targetName, ".circle"))
                    Logo = GetLogoFromRing(targetName);
            }

            if (Logo == null)
                Logo = BitmapPainter.ConvertToRealColors(KrentoRing.DefaultRingImage, false);

            AnimatedLogo = ImageAnimator.CanAnimate(Logo);
            if (!AnimatedLogo)
            {
                Logo = BitmapPainter.ResizeBitmap(Logo, FileImage.ImageSize, FileImage.ImageSize, true);
            }

            SaveLogoToCache();
        }

        public override void ReadConfiguration(MemIniFile ini)
        {
            if (!IsVirtual)
            {
                string stringValue;

                base.ReadConfiguration(ini);

                try
                {
                    stringValue = ini.ReadString(this.StoneID, "Target", null);
                    if (!string.IsNullOrEmpty(stringValue))
                        TargetName = stringValue;
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
                if (!string.IsNullOrEmpty(TargetName))
                    ini.WriteString(this.StoneID, "Target", TargetName);
            }
        }

        /// <summary>
        /// Gets or sets the name of the target file or URL.
        /// </summary>
        /// <value>The name of the target file or URL.</value>
        public string TargetName
        {
            get { return targetName; }
            set { targetName = value; }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    targetName = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

    }
}

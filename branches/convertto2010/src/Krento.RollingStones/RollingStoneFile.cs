//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.InteropServices;

namespace Krento.RollingStones
{
    /// <summary>
    /// Special stone for executing files or URLs
    /// </summary>
    public class RollingStoneFile : RollingStoneTask
    {

        private string targetParameters;
        private string argumentDescription;
        private string targetName;

        //for drag and drop helper must be visible
        internal string programName;
        internal string dynamicParameters;

        public RollingStoneFile(StonesManager manager)
            : base(manager)
        {
            try
            {
                AllowDrop = true;
                DragEnter += new DragEventHandler(window_DragEnter);
                DragDrop += new DragEventHandler(window_DragDrop);
                TargetDescription = null;
                ResourceName = "UnknownFile.png";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }

        void window_DragEnter(object sender, DragEventArgs e)
        {
            DragDropHelper.DragOverTarget(e);
        }


        protected internal override void ExecuteConfigurationCall()
        {
            Manager.SuppressHookMessage(true);
            try
            {
                StoneFileConfig config = new StoneFileConfig();
                try
                {
                    config.Target = this.TargetName;
                    if (!TextHelper.SameText(TargetDescription, SR.MissingFile))
                        config.Description = this.TargetDescription;

                    config.CommandLine = this.TargetParameters;
                    config.Image = (Image)BitmapPainter.ConvertToRealColors(this.Logo, false);
                    config.CustomIcon = this.CustomIcon;
                    config.Argument = this.ArgumentDescription;

                    if (config.ShowDialog() == DialogResult.OK)
                    {
                        if (!string.IsNullOrEmpty(config.Target))
                        {
                            this.targetName = config.Target;
                            this.targetParameters = config.CommandLine;

                            if (!string.IsNullOrEmpty(targetParameters))
                            {
                                targetParameters = FileOperations.StripFileName(targetParameters);
                            }

                            string newDescription = config.Description;
                            if (string.IsNullOrEmpty(newDescription))
                            {
                                if (string.IsNullOrEmpty(TranslationId))
                                    newDescription = FileOperations.GetFileDescription(targetName);
                            }
                            this.TargetDescription = newDescription;

                            this.argumentDescription = config.Argument;
                            CustomIcon = config.CustomIcon;

                            //If Target is missing the description is not stored
                            DestroyLogoImage();
                            FixupConfiguration();
                            Update(true);
                            Manager.RedrawScreenHint();
                        }

                        Manager.FlushCurrentCircle();
                    }
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


        private void window_DragDrop(object sender, DragEventArgs e)
        {
            DragDropHelper.DragDropTarget(this, e);

            Manager.BringToFront();
        }



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


        /// <summary>
        /// Updates the logo URL. This method must be used only in case if custom logo is not
        /// present or not provided. Check for custom logo using WebsiteImages.CustomSiteIcon method
        /// and compare result with empty string.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        private void UpdateLogoURL(string newTarget)
        {

            StopAnimation();
            Bitmap tmpBitmap = (Bitmap)FileOperations.GetSiteLogo(newTarget);

            if (Logo != null)
            {
                Logo.Dispose();
                Logo = null;
            }

            AnimatedLogo = ImageAnimator.CanAnimate(tmpBitmap);

            if (!AnimatedLogo)
            {
                Logo = BitmapPainter.ResizeBitmap(tmpBitmap, FileImage.ImageSize, FileImage.ImageSize, true);
            }
            else
                Logo = tmpBitmap;

            SaveLogoToCache();
        }

        private static Bitmap GetLogoFromRing(string ringName)
        {
            if (!FileOperations.FileExists(ringName))
                return null;

            KrentoRing ring = new KrentoRing(ringName);
            Bitmap result = BitmapPainter.ConvertToRealColors(ring.Logo, false);
            ring.Dispose();
            return result;
        }

        private void UpdateLogoBitmap(Bitmap newLogo)
        {
            StopAnimation();
            if (Logo != null)
            {
                Logo.Dispose();
                Logo = null;
            }

            Logo = newLogo;
            SaveLogoToCache();
        }

        private void UpdateLogoFile(string newTarget)
        {
            StopAnimation();
            if (Logo != null)
            {
                Logo.Dispose();
                Logo = null;
            }

            string fullTarget = FileOperations.StripFileName(newTarget);

            if (!FileOperations.IsValidPathName(fullTarget))
            {
                Logo = NativeThemeManager.LoadBitmap("UnknownFile.png");
            }
            else
            {
                if (!string.IsNullOrEmpty(fullTarget))
                {
                    if (NativeMethods.FileExtensionIs(fullTarget, ".circle"))
                        Logo = GetLogoFromRing(fullTarget);
                }

                if (Logo == null)
                    try
                    {
                        Logo = (Bitmap)FileImage.FileNameImage(fullTarget);
                    }
                    catch
                    {
                        Logo = null;
                    }

                if (Logo == null)
                {
                    if (FileOperations.DirectoryExists(fullTarget))
                    {
                        Logo = NativeThemeManager.LoadBitmap("Folder.png");
                    }
                    else
                    {
                        Logo = FileOperations.GetExtensionLogo(fullTarget);
                        if (Logo == null)
                            Logo = NativeThemeManager.LoadBitmap("UnknownFile.png");
                    }
                }
            }

            AnimatedLogo = ImageAnimator.CanAnimate(Logo);
            if (!AnimatedLogo)
            {
                Logo = BitmapPainter.ResizeBitmap(Logo, FileImage.ImageSize, FileImage.ImageSize, true);
            }

            SaveLogoToCache();
        }

        public override void DrawTargetDescription()
        {
            if (!string.IsNullOrEmpty(targetName))
                base.DrawTargetDescription();
            else
            {
                Caption = SR.MissingFile;
                Manager.DrawText(SR.MissingFile);
                MoveStoneHint();
            }
        }


        internal void UpdateTargetURL(string newTarget, string newDescription)
        {
            CustomIcon = null;
            string urlCustomIcon = WebsiteImage.CustomSiteIcon(newTarget);
            if (string.IsNullOrEmpty(urlCustomIcon))
                UpdateLogoURL(newTarget);
            else
                CustomIcon = urlCustomIcon;

            targetName = newTarget;
            TargetDescription = newDescription;
            if (string.IsNullOrEmpty(TargetDescription))
            {
                TargetDescription = newTarget;
            }
            FixupConfiguration();
            Update(true);
        }

        /// <summary>
        /// Updates the target.
        /// </summary>
        /// <param name="newTarget">The new target.</param>
        internal void UpdateTarget(string newTarget)
        {
            CustomIcon = null;
            if (FileOperations.IsURL(newTarget))
            {
                string urlCustomIcon = WebsiteImage.CustomSiteIcon(newTarget);
                if (string.IsNullOrEmpty(urlCustomIcon))
                    UpdateLogoURL(newTarget);
                else
                    CustomIcon = urlCustomIcon;
                targetName = newTarget;
                TranslationId = string.Empty;
                TargetDescription = FileOperations.GetPageTitle(newTarget);
                if (string.IsNullOrEmpty(TargetDescription))
                {
                    TargetDescription = newTarget;
                }
                Update(true);
                Manager.FlushCurrentCircle();
            }
            else
            {
                string fileCustomIcon = FileImage.CustomFileIcon(newTarget);
                if (string.IsNullOrEmpty(fileCustomIcon))
                {
                    UpdateLogoFile(newTarget);
                }
                else
                {
                    CustomIcon = fileCustomIcon;
                    DestroyLogoImage();
                    FixupLogoImage();
                }

                //if the image was dragged on the stone, the logo is updated
                if (!FileOperations.FileIsImage(newTarget))
                {
                    targetName = newTarget;
                    TranslationId = string.Empty;
                    TargetDescription = Path.GetFileNameWithoutExtension(targetName);
                    if (string.IsNullOrEmpty(TargetDescription))
                    {
                        string rootPath = Path.GetPathRoot(targetName);
                        if (TextHelper.SameText(rootPath, targetName))
                        {
                            string volumeName;
                            string volumeLabel;
                            DriveInfo di = new DriveInfo(rootPath);
                            volumeName = di.Name;
                            volumeLabel = di.VolumeLabel;
                            if (!string.IsNullOrEmpty(volumeLabel))
                                TargetDescription = volumeLabel + "(" + volumeName + ")";
                            else
                                TargetDescription = "(" + volumeName + ")";
                        }
                        else
                            TargetDescription = targetName;
                    }
                }
                else
                {
                    CustomIcon = newTarget;
                }



                FixupConfiguration();
                Update(true);
            }
        }


        public override void ReadConfiguration(MemIniFile ini)
        {
            if (!IsVirtual)
            {
                string stringValue;

                //we need target name before reading the base config
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

                base.ReadConfiguration(ini);

                try
                {
                    stringValue = ini.ReadString(this.StoneID, "Parameters", null);
                    if (!string.IsNullOrEmpty(stringValue))
                        TargetParameters = stringValue;

                    stringValue = ini.ReadString(this.StoneID, "Argument", null);
                    if (!string.IsNullOrEmpty(stringValue))
                        ArgumentDescription = stringValue;
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
                if (!string.IsNullOrEmpty(TargetParameters))
                    ini.WriteString(this.StoneID, "Parameters", TargetParameters);
                if (!string.IsNullOrEmpty(ArgumentDescription))
                    ini.WriteString(this.StoneID, "Argument", ArgumentDescription);
            }
        }

        /// <summary>
        /// This method is called by the manager, when the loading of the stone configuration is finished
        /// </summary>
        public override void FixupConfiguration()
        {
            base.FixupConfiguration();

            try
            {
                if (!FileOperations.IsURL(targetName))
                {
                    if (!FileOperations.FileOrFolderExists(FileSearch.FullPath(targetName)))
                    {
                        string tmpTarget = FileOperations.RemoveURI(targetName);
                        if (FileOperations.FileOrFolderExists(FileSearch.FullPath(tmpTarget)))
                        {
                            targetName = tmpTarget;
                        }
                    }

                }
                if ((string.IsNullOrEmpty(TargetDescription)) && (string.IsNullOrEmpty(TranslationId)))
                    TargetDescription = FileOperations.GetFileDescription(targetName);

            }
            catch (Exception ex)
            {
                throw new StoneSettingsException("Stone settings parse error", ex);
            }
        }

        protected override void FixupTargetLogo()
        {
            StopAnimation();
            if (FileOperations.IsURL(targetName))
            {
                string urlCustomIcon = WebsiteImage.CustomSiteIcon(targetName);
                if (string.IsNullOrEmpty(urlCustomIcon))
                    UpdateLogoURL(targetName);
                else
                    CustomIcon = urlCustomIcon;
            }
            else
            {
                UpdateLogoFile(targetName);
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

        public string TargetParameters
        {
            get { return targetParameters; }
            set { targetParameters = value; }
        }

        public string ArgumentDescription
        {
            get { return argumentDescription; }
            set { argumentDescription = value; }
        }

        /// <summary>
        /// Execute the file (application or folder). This method is used by Run method
        /// </summary>
        private void ExecuteAsync()
        {
            string workingFolder;

            if (string.IsNullOrEmpty(programName))
                return;


            try
            {
                if (FileOperations.FileIsExe(programName))
                {
                    workingFolder = Path.GetDirectoryName(programName);
                    FileExecutor.ProcessExecute(programName, targetParameters, workingFolder);
                }
                else
                {
                    FileExecutor.Execute(programName, targetParameters);
                }
            }
            finally
            {
                programName = string.Empty;
            }
        }

        private void ExecuteAsyncWithParams()
        {
            string workingFolder;

            if (string.IsNullOrEmpty(programName))
                return;

            try
            {
                if (NativeMethods.FileExtensionIs(programName, ".exe"))
                {
                    workingFolder = Path.GetDirectoryName(programName);
                    FileExecutor.ProcessExecute(programName, dynamicParameters, workingFolder);
                }
                else
                {
                    FileExecutor.Execute(programName, dynamicParameters);
                }
            }
            finally
            {
                dynamicParameters = string.Empty;
                programName = string.Empty;
            }
        }

        internal void PerformExecution()
        {
            base.Run();
            Thread t = new Thread(new ThreadStart(ExecuteAsync));
            t.Name = "KrentoExecutor";
            t.IsBackground = true;
            t.Start();
            Thread.Sleep(0);
        }

        internal void PerformDynamicExecution()
        {
            base.Run();
            Thread t = new Thread(new ThreadStart(ExecuteAsyncWithParams));
            t.Name = "KrentoExecutor";
            t.IsBackground = true;
            t.Start();
            Thread.Sleep(0);
        }

        /// <summary>
        /// Runs this target. The file or URL linked to the stone will be executed
        /// </summary>
        public override void Run()
        {
            bool perform = true;
            string args;

            string fullName = FileOperations.StripFileName(targetName);

            if (string.IsNullOrEmpty(fullName))
            {
                Configure();
                //base.Run();
                return;
            }

            if (!FileOperations.IsValidPathName(fullName))
            {
                base.Run();
                return;
            }

            if (NativeMethods.FileExtensionIs(fullName, ".circle"))
            {
                Manager.ReplaceCurrentCircle(fullName);
                return;
            }


            programName = fullName;
            if (targetName.Contains("##"))
            {
                ArgumentDialog ad = new ArgumentDialog();
                Manager.SuppressHookMessage(true);
                try
                {
                    ad.Location = this.Location;
                    if (!string.IsNullOrEmpty(argumentDescription))
                        ad.Description = argumentDescription;
                    if (ad.ShowDialog() == DialogResult.OK)
                        args = ad.Argument;
                    else
                    {
                        args = string.Empty;
                        perform = false;
                    }
                    if (string.IsNullOrEmpty(args))
                        args = string.Empty;
                }
                finally
                {
                    Manager.SuppressHookMessage(false);
                }

                programName = programName.Replace("##", args);

            }
            else
            {
                if (FileOperations.FileIsLink(fullName))
                {
                    string linkTarget = FileOperations.ExtractFileNameFromShellLink(fullName);
                    if (FileOperations.DirectoryExists(linkTarget))
                        fullName = linkTarget;
                }

                if (FileOperations.DirectoryExists(fullName) && (FileOperations.GetFilesCount(fullName) > 0))
                {
                    Manager.SuppressHookMessage(true);
                    try
                    {
                        LiveFolder liveFolder = new LiveFolder(this.Manager.Handle, fullName);
                        try
                        {
                            perform = liveFolder.Execute();
                            if (perform)
                                programName = liveFolder.Items[liveFolder.SelectedItem].FileName;
                        }
                        finally
                        {
                            liveFolder.Dispose();
                        }
                    }
                    finally
                    {
                        Manager.SuppressHookMessage(false);
                    }
                }
            }


            if (perform)
            {
                PerformExecution();
            }
        }

    }
}

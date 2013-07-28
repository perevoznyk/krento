//===============================================================================
// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Permissions;
using Krento.RollingStones;
using Krento.Properties;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using Laugris.Sage;
using System.Globalization;
using System.Collections.Generic;

namespace Krento
{
    /// <summary>
    /// Invisible form, main form of the application
    /// </summary>
    internal partial class MainForm : Form
    {
        private bool wasException;
        private Hotkeys hotKeys;
        private Hotkeys ringKeys;
        private MouseHook mouseHook;
        private SplashScreen splashScreen;
        private bool systemReady;
        private Point notifyMouse;
        private KrentoContext context;
        private Dictionary<string, PulsarDropFileHandler> dropFileHandlers;
        private VersionChecker checker = new VersionChecker();
        private bool trackRightDown;
        private bool trackRightUp;

        public MainForm(KrentoContext context)
            : base()
        {
            this.context = context;
            dropFileHandlers = new Dictionary<string, PulsarDropFileHandler>();
            const string doc = ".doc;.xls;.ppt;.docx;.docm;.xlsx;.xlsm;.pptx;.pptm;.vso;.vsd;.mpp;.pub;.xsn;.txt;.rtf;.pdf;.html;.htm;.chm";
            string[] ext = doc.Split(';');
            for (int i = 0; i < ext.Length; i++)
            {
                if (!string.IsNullOrEmpty(ext[i]))
                    dropFileHandlers.Add(ext[i], new PulsarDropFileHandler(MyDocumentsHandler));
            }
            const string pic = ".jpg;.jpeg;.jpe;.gif;.tiff;.tif;.png;.bmp;.rle;.dib;.wmf;.wmz;.emf;.emz;.cgm;.pict;.pic";
            ext = pic.Split(';');
            for (int i = 0; i < ext.Length; i++)
            {
                if (!string.IsNullOrEmpty(ext[i]))
                    dropFileHandlers.Add(ext[i], new PulsarDropFileHandler(MyPicturesHandler));
            }
            const string music = ".wma;.mp3;.acc;.aiff;.wav;.ra;.mid";
            ext = music.Split(';');
            for (int i = 0; i < ext.Length; i++)
            {
                if (!string.IsNullOrEmpty(ext[i]))
                    dropFileHandlers.Add(ext[i], new PulsarDropFileHandler(MyMusicHandler));
            }
            InitializeComponent();
        }

        public Dictionary<string, PulsarDropFileHandler> DropFileHandler
        {
            get { return dropFileHandlers; }
        }

        private void MyDocumentsHandler(PulsarEventArgs e)
        {
            string shortName = Path.GetFileName(e.FileName);
            File.Copy(e.FileName, FileOperations.IncludeTrailingPathDelimiter(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) + shortName, true);
            if (GlobalSettings.ShowPopupAlerts)
            {
                notifier.Caption = SR.KrentoNotification;
                notifier.Text = SR.CopyDocument(shortName);
                notifier.Url = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                notifier.Show();
            }
        }

        private void MyPicturesHandler(PulsarEventArgs e)
        {
            string shortName = Path.GetFileName(e.FileName);
            File.Copy(e.FileName, FileOperations.IncludeTrailingPathDelimiter(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)) + shortName, true);
            if (GlobalSettings.ShowPopupAlerts)
            {
                notifier.Caption = SR.KrentoNotification;
                notifier.Text = SR.CopyPicture(shortName);
                notifier.Url = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                notifier.Show();
            }
        }

        private void MyMusicHandler(PulsarEventArgs e)
        {
            string shortName = Path.GetFileName(e.FileName);
            File.Copy(e.FileName, FileOperations.IncludeTrailingPathDelimiter(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)) + shortName, true);
            if (GlobalSettings.ShowPopupAlerts)
            {
                notifier.Caption = SR.KrentoNotification;
                notifier.Text = SR.CopyMusic(shortName);
                notifier.Url = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                notifier.Show();
            }
        }

        public void ActivateEngine()
        {
            TraceDebug.Trace("Activate Engine");

            bool doCheck = GlobalSettings.CheckUpdate;
            bool disableCheck = false;

            Visible = false;
            NativeMethods.ShowWindow(this.Handle, 0);
            NativeMethods.RemoveSystemMenu(this.Handle);


            #region First Instance Execution
            if (context.FirstInstance)
            {

                if (!string.IsNullOrEmpty(GlobalSettings.IconName))
                {
                    Icon customIcon = null;

                    if (FileOperations.FileExists(GlobalSettings.IconName))
                    {
                        Icon loadIcon = new Icon(FileOperations.StripFileName(GlobalSettings.IconName));
                        try
                        {
                            customIcon = (Icon)loadIcon.Clone();
                        }
                        finally
                        {
                            loadIcon.Dispose();
                            loadIcon = null;
                        }

                        notifyIcon.Icon = customIcon;
                        //this.Icon = customIcon;
                    }
                }

                if (splashScreen != null)
                    splashScreen.Show();

                if (!string.IsNullOrEmpty(GlobalSettings.GlyphName))
                {
                    if (FileOperations.FileExists(GlobalSettings.GlyphName))
                    {
                        Bitmap tmp = FastBitmap.FromFile(GlobalSettings.GlyphName);
                        notifier.Glyph = BitmapPainter.ResizeBitmap(tmp, notifier.GlyphSize, notifier.GlyphSize, true);
                    }
                }

                if (notifier.Glyph == null)
                {
                    notifier.Glyph = NativeThemeManager.Load("BigKrento.png");
                }

                context.Manager.SetHookMessage(context);

                ReloadAll();


                PreparePulsar();


                context.Manager.Rotate(0);
                if (!GlobalSettings.ShowManagerButtons)
                    context.Manager.DrawText(SR.KrentoWelcome);
                context.Manager.Update();
                context.Manager.Visible = false;


                notifyIcon.Visible = GlobalSettings.ShowTrayIcon;


                if (splashScreen != null)
                {
                    if (KrentoContext.FirstRun)
                    {
                        splashScreen.Hide();
                        NativeMethods.PostMessage(InteropHelper.MainWindow, NativeMethods.CM_SPLASHCLOSE, IntPtr.Zero, IntPtr.Zero);
                    }
                    else
                    {
                        splashScreen.Close(200);
                    }
                }

                systemReady = true;

                string keyMessage = HotKeyMessage;



                notifier.Caption = SR.KrentoWelcome;

                if (GlobalSettings.MouseHook == MouseHookButton.None)
                {
                    notifier.Text = SR.WelcomeMessage(keyMessage, "", "");
                }
                else
                {
                    string mouseText = "";
                    switch (GlobalSettings.MouseHook)
                    {
                        case MouseHookButton.Wheel:
                            mouseText = MouseKeyMessage + SR.MouseWheelButton;
                            break;
                        case MouseHookButton.XButton1:
                            mouseText = MouseKeyMessage + SR.MouseXButton1;
                            break;
                        case MouseHookButton.XButton2:
                            mouseText = MouseKeyMessage + SR.MouseXButton2;
                            break;
                        default:
                            mouseText = "";
                            break;
                    }

                    notifier.Text = SR.WelcomeMessage(keyMessage, SR.OrClick, mouseText);
                }

                //Only show popup window when asked
                if (GlobalSettings.ShowPopupAlerts)
                {
                    if (GlobalConfig.LowMemory)
                    {
                        notifyIcon.BalloonTipTitle = notifier.Caption;
                        notifyIcon.BalloonTipText = notifier.Text;
                        notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                        notifyIcon.ShowBalloonTip(5000);
                    }
                    else
                    {
                        notifier.Show();
                    }

                }


                Manager.ClearUnusedMemory();

                #region manage autoupdate
                DateTime lastCheck = Settings.Default.LastCheck;
                TimeSpan ts = DateTime.Now - lastCheck;
                if (ts.Days >= 7)
                {
                    Settings.Default.LastCheck = DateTime.Now;
                    Settings.Default.Save();
                    if (Settings.Default.AskForCheck)
                    {
                        UpdateDialog upd = new UpdateDialog();
                        try
                        {
                            upd.ShowDialog();
                            doCheck = (upd.DialogResult == DialogResult.OK);
                            disableCheck = (upd.btnDisable.Checked);
                            if (disableCheck)
                            {
                                GlobalSettings.CheckUpdate = doCheck;
                                Settings.Default.AskForCheck = false;
                                Settings.Default.Save();
                            }
                        }
                        finally
                        {
                            upd.Dispose();
                        }
                    }

                    if ((GlobalSettings.CheckUpdate) && (doCheck))
                    {
                        checker.CheckNewVersion();
                    }
                }
                #endregion

                if (GlobalSettings.StartVisible || KrentoContext.FirstRun)
                {
                    InteropHelper.ShowDesktop();
                    if (!Manager.Visible)
                    {
                        if (KrentoContext.FirstRun)
                            Manager.ShowAndExecute(Manager.SimulateWheelRotation);
                        else
                            ChangeVisibility();
                    }
                }

            }
            #endregion
            else
            {
                systemReady = false;
                context.FirstInstance = false;
            }
        }

        public void ShowNewVersion()
        {
            notifier.CloseUp();
            string downloadSite = checker.Url;
            if (string.IsNullOrEmpty(downloadSite))
                downloadSite = Settings.Default.Website;
            notifier.Text = SR.NewVersionAvailable(checker.Version, downloadSite);
            notifier.Url = downloadSite;
            if (GlobalConfig.LowMemory)
            {
                notifyIcon.BalloonTipTitle = notifier.Caption;
                notifyIcon.BalloonTipText = notifier.Text;
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.ShowBalloonTip(5000);
            }
            else
            {
                notifier.Show();
            }

        }

        public SplashScreen SplashScreen
        {
            get { return splashScreen; }
            set { splashScreen = value; }
        }

        public StonesManager Manager
        {
            get { return this.context.Manager; }
        }

        public Pulsar Pulsar
        {
            get { return this.context.Pulsar; }
        }


        public MainForm()
        {
            InitializeComponent();
        }

        internal void PrepareKeyboardHook()
        {
            KeyModifiers mod = KeyModifiers.None;
            try
            {
                if (hotKeys != null)
                {
                    hotKeys.Dispose();
                    hotKeys = null;
                }

                if (ringKeys != null)
                {
                    ringKeys.Dispose();
                    ringKeys = null;
                }

                if (GlobalSettings.UseKeyboardActivation)
                {
                    try
                    {
                        if ((GlobalSettings.Modifiers & Keys.LWin) != 0)
                            mod |= KeyModifiers.Windows;
                        if ((GlobalSettings.Modifiers & Keys.Shift) != 0)
                            mod |= KeyModifiers.Shift;
                        if ((GlobalSettings.Modifiers & Keys.Alt) != 0)
                            mod |= KeyModifiers.Alt;
                        if ((GlobalSettings.Modifiers & Keys.Control) != 0)
                            mod |= KeyModifiers.Control;
                        hotKeys = new Hotkeys(mod, GlobalSettings.Shortcut, this);
                    }
                    catch (ArgumentException)
                    {
                        hotKeys = null;
                        RtlAwareMessageBox.Show(SR.HotkeyCollision(HotKeyMessage), SR.KrentoShortName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (GlobalSettings.CircleSelector)
                    {
                        try
                        {
                            ringKeys = new Hotkeys(KeyModifiers.Windows, Keys.N, this);
                        }
                        catch
                        {
                            ringKeys = null;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TraceDebug.Trace("PrepareKeyboardHook: ", ex);
                throw;
            }
        }

        internal void PrepareMouseHook()
        {
            if (mouseHook != null)
            {
                mouseHook.Dispose();
                mouseHook = null;
            }


            mouseHook = new MouseHook(this.Handle);

            MouseHook.TrackMove = false;
            MouseHook.TrackWheel = false;
            MouseHook.DesktopClick = GlobalSettings.DesktopClick;
            MouseHook.TrackClick = false;
            MouseHook.InterceptClick = false;
            MouseHook.TrackWheelClick = false;
            MouseHook.TrackXButtonClick = false;

            if (GlobalSettings.UseMouseActivation)
            {
                if (GlobalSettings.MouseHook == MouseHookButton.None)
                {
                    MouseHook.TrackClick = false;
                    MouseHook.InterceptClick = false;
                }
                else
                {
                    MouseHook.TrackClick = true;

                    if (GlobalSettings.UseRightClick)
                    {
                        MouseHook.InterceptClick = true;
                    }
                    else
                    {
                        MouseHook.InterceptClick = false;
                    }

                    if (GlobalSettings.MouseHook == MouseHookButton.Wheel)
                        MouseHook.TrackWheelClick = true;
                    else
                        MouseHook.TrackWheelClick = false;


                    if ((GlobalSettings.MouseHook == MouseHookButton.XButton1) || (GlobalSettings.MouseHook == MouseHookButton.XButton2))
                        MouseHook.TrackXButtonClick = true;
                    else
                        MouseHook.TrackXButtonClick = false;
                }
            }
        }

        internal static string GetCurrentCircleName()
        {
            string circleName = GlobalSettings.CircleName;
            if (string.IsNullOrEmpty(circleName))
                circleName = GlobalConfig.DefaultCircleName;
            if (!FileOperations.FileExists(circleName))
                circleName = GlobalConfig.HomeCircleName;

            return circleName;
        }


        internal void ReloadAll()
        {

            if (!string.IsNullOrEmpty(KrentoContext.CircleParameter))
                GlobalSettings.CircleName = KrentoContext.CircleParameter;



            notifyIcon.Text = this.Text;

            context.Manager.FadeDelay = GlobalSettings.FadeDelay;
            context.Manager.LoadStoneClasses();
            context.ApplyManagerParameters();
            context.Manager.LoadSettings(GlobalConfig.KrentoSettingsFileName);

            /*
             *  Load circle
             * ===============================
             * Here we have 2 options:
             * 1. Circle name provided, but file does not exists
             * 2. File exists, but currupted
             * 
             * When no file found, load default circle. If no default circle, create one.
             * If file exists, make it empty circle
             */

            string initialCircle = GetCurrentCircleName();
            if (!FileOperations.FileExists(initialCircle))
            {
                context.Manager.CreateDefaultCircle();
                GlobalSettings.CircleName = GlobalConfig.HomeCircleName;
            }


            if (KrentoContext.CreateDesktopCircle)
                context.Manager.CreateDesktopCircle();

            context.Manager.LoadCircle(initialCircle);

            PrepareKeyboardHook();
            PrepareMouseHook();


        }

        /// <summary>
        /// Unhook the keyboard and dispose hotkeys object
        /// </summary>
        private void UnloadKeyboardHook()
        {
            if (hotKeys != null)
                try
                {
                    hotKeys.Dispose();
                }
                finally
                {
                    hotKeys = null;
                }

            if (ringKeys != null)
            {
                ringKeys.Dispose();
                ringKeys = null;
            }
        }

        /// <summary>
        /// Unhook the mouse adn dispose mouse hook object
        /// </summary>
        private void UnloadMouseHook()
        {
            if (mouseHook != null)
                try
                {
                    mouseHook.Dispose();
                }
                finally
                {
                    mouseHook = null;
                }
        }

        /// <summary>
        /// Saves all settings and current circle to the hard disk.
        /// Use this method to save all user settings
        /// </summary>
        internal void SaveAll()
        {
            if (systemReady)
            {
                Settings.Default.PulsarLeft = Pulsar.Left;
                Settings.Default.PulsarTop = Pulsar.Top;
                Settings.Default.ManagerLeft = Manager.Left;
                Settings.Default.ManagerTop = Manager.Top;
                Settings.Default.ShowPulsar = Pulsar.Visible;
                Settings.Default.Save();
                NativeMethods.WriteString(GlobalConfig.KrentoSettingsFileName, "General", "CircleName", FileOperations.UnExpandPath(Manager.CurrentCircle));
            }
        }

        internal void UnloadAll()
        {
            try
            {
                SaveAll();
            }
            catch (Exception ex)
            {
                //do not raise an exception here because Krento wants to shutdown
                TraceDebug.Trace(ex);
            }
            UnloadMouseHook();
            UnloadKeyboardHook();
            context.FreeAllResources();
        }


        private void PreparePulsar()
        {

            Pulsar.Run();
            Pulsar.MoveBottomRight();
            Pulsar.Update();
            Pulsar.AllowDrop = true;

            Pulsar.Left = Settings.Default.PulsarLeft;
            Pulsar.Top = Settings.Default.PulsarTop;

            if ((Pulsar.Left == -1) && (Pulsar.Top == -1))
            {
                if (InteropHelper.RunningOnWin8)
                {
                    Pulsar.Left = 4;
                    Pulsar.Top = (int)(PrimaryScreen.Bounds.Bottom - Pulsar.Dimension);
                }
                else
                {
                    Pulsar.Left = (int)(PrimaryScreen.Bounds.Right - Pulsar.Dimension);
                    Pulsar.Top = (int)(PrimaryScreen.Bounds.Bottom - Pulsar.Dimension);
                }
            }


            Pulsar.MouseClick += new MouseEventHandler(HandlePulsarMouseClick);
            Pulsar.MouseDoubleClick += new MouseEventHandler(HandlePulsarMouseDoubleClick);

            Pulsar.DragEnter += new DragEventHandler(pulsar_DragEnter);
            Pulsar.DragDrop += new DragEventHandler(Pulsar_DragDrop);
            Pulsar.DragOver += new DragEventHandler(Pulsar_DragOver);
            Pulsar.Visible = Settings.Default.ShowPulsar;

        }

        private void Pulsar_DragOver(object sender, DragEventArgs e)
        {
            if (e == null)
                return;

            DragDropEffects allowed = e.AllowedEffect;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if ((allowed & DragDropEffects.Link) == DragDropEffects.Link)
                {

                    e.Effect = DragDropEffects.Link;
                }
                else
                    if ((allowed & DragDropEffects.Copy) == DragDropEffects.Copy)
                    {

                        e.Effect = DragDropEffects.Copy;
                    }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }


        internal void InstallKrentoPart(string fileName)
        {
            string destination = null;
            bool compressed = false;
            bool loadNewToy = false;
            bool loadNewDocklet = false;
            bool reloadStones = false;
            string mainFolderName = null;

            string ext = Path.GetExtension(fileName);
            if (TextHelper.SameText(ext, ".toy"))
            {
                destination = GlobalConfig.ToysFolder;
                compressed = true;
                loadNewToy = true;
            }
            else
                if (TextHelper.SameText(ext, ".kmenu"))
                {
                    destination = GlobalConfig.MenusFolder;
                    compressed = true;
                }
                else
                    if (TextHelper.SameText(ext, ".kskin"))
                    {
                        destination = GlobalConfig.SkinsFolder;
                        compressed = true;
                    }
                    else
                        if (TextHelper.SameText(ext, ".stone"))
                        {
                            destination = GlobalConfig.StoneClasses;
                            compressed = true;
                            reloadStones = true;
                        }
                        else
                            if (TextHelper.SameText(ext, ".lng"))
                            {
                                destination = GlobalConfig.LanguagesFolder;
                                compressed = false;
                            }
                            else
                                if (TextHelper.SameText(ext, ".kadd"))
                                {
                                    destination = GlobalConfig.AddInRootFolder;
                                    compressed = true;
                                }
                                else
                                    if (TextHelper.SameText(ext, ".docklet"))
                                    {
                                        destination = GlobalConfig.DockletsFolder;
                                        compressed = true;
                                        loadNewDocklet = true;
                                    }
                                    else
                                        if (TextHelper.SameText(ext, ".circle"))
                                        {
                                            destination = GlobalConfig.RollingStonesFolder;
                                            compressed = false;
                                        }

            if (string.IsNullOrEmpty(destination))
            {
                return;
            }

            if (!compressed)
            {
                string destinationName = Path.Combine(destination, Path.GetFileName(fileName));
                try
                {
                    FileOperations.CopyFile(fileName, destinationName);
                    if (TextHelper.SameText(destination, GlobalConfig.RollingStonesFolder))
                    {
                        if (FileOperations.FileExists(destinationName))
                            Manager.HistoryAdd(destinationName);
                    }
                }
                catch (Exception ex)
                {
                    TraceDebug.Trace(ex);
                }
            }
            else
            {

                try
                {
                    destination = Path.Combine(destination, Path.GetFileNameWithoutExtension(fileName));
                    if (!Directory.Exists(destination))
                        Directory.CreateDirectory(destination);
                }
                catch (Exception ex)
                {
                    TraceDebug.Trace(ex);
                }

                NativeMethods.ExtractArchive(fileName, destination);
                FileOperations.CopyFile(fileName, Path.Combine(GlobalConfig.DownloadsFolder, Path.GetFileName(fileName)));

                if (reloadStones)
                {
                    try
                    {
                        string newStonePath;
                        if (!string.IsNullOrEmpty(mainFolderName))
                            newStonePath = Path.Combine(destination, mainFolderName);
                        else
                            newStonePath = destination;
                        Language.Merge(Path.Combine(newStonePath, GlobalSettings.Language + ".lng"));
                    }
                    catch (Exception ex)
                    {
                        //Toy load error happens
                        TraceDebug.Trace(ex);
                    }

                    //Loading of the new stone must be AFTER merging of the language
                    Manager.LoadStoneClasses();
                }
                else
                    if (loadNewToy)
                    {
                        try
                        {
                            string newToyPath;
                            if (!string.IsNullOrEmpty(mainFolderName))
                                newToyPath = Path.Combine(destination, mainFolderName);
                            else
                                newToyPath = destination;
                            context.LoadKrentoToys(newToyPath);
                        }
                        catch (Exception ex)
                        {
                            //Toy load error happens
                            TraceDebug.Trace(ex);
                        }
                    }
                    else
                        if (loadNewDocklet)
                        {
                            try
                            {
                                string newDockletPath;

                                if (!string.IsNullOrEmpty(mainFolderName))
                                    newDockletPath = Path.Combine(destination, mainFolderName);
                                else
                                    newDockletPath = destination;
                                context.LoadKrentoDocklets(newDockletPath);
                            }
                            catch (Exception ex)
                            {
                                //Toy load error happens
                                TraceDebug.Trace(ex);
                            }
                        }
            }
        }

        private void Pulsar_DragDrop(object sender, DragEventArgs e)
        {
            bool handled = false;
            PulsarDropFileHandler handler;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] strArray = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (strArray == null)
                    return;

                if (strArray.Length < 1)
                    return;

                for (int i = 0; i < strArray.Length; i++)
                {
                    string str2 = FileOperations.RemoveURI(strArray[i]);
                    string fullName = FileOperations.StripFileName(str2);
                    string fileExt = Path.GetExtension(fullName).ToLower(CultureInfo.InvariantCulture);

                    if (dropFileHandlers.ContainsKey(fileExt))
                        handler = dropFileHandlers[fileExt];
                    else
                        handler = null;

                    if (handler != null)
                    {
                        PulsarEventArgs arg = new PulsarEventArgs(fullName);
                        try
                        {
                            handler(arg);
                            handled = arg.Handled;
                        }
                        catch
                        {
                            handled = false;
                        }
                        arg.Dispose();
                    }
                    if (!handled)
                    {
                        InstallKrentoPart(fullName);
                    }
                }
            }
        }

        public static string HotKeyMessage
        {
            get
            {
                string keyMessage = "";
                if ((GlobalSettings.Modifiers & Keys.LWin) != 0)
                    keyMessage += "Win + ";
                if ((GlobalSettings.Modifiers & Keys.Shift) != 0)
                    keyMessage += "Shift + ";
                if ((GlobalSettings.Modifiers & Keys.Alt) != 0)
                    keyMessage += "Alt + ";
                if ((GlobalSettings.Modifiers & Keys.Control) != 0)
                    keyMessage += "Ctrl + ";

                keyMessage += GlobalSettings.Shortcut.ToString();
                return keyMessage;
            }
        }


        public static string MouseKeyMessage
        {
            get
            {
                string keyMessage = "";
                if ((GlobalSettings.MouseModifiers & Keys.LWin) != 0)
                    keyMessage += "Win + ";
                if ((GlobalSettings.MouseModifiers & Keys.Shift) != 0)
                    keyMessage += "Shift + ";
                if ((GlobalSettings.MouseModifiers & Keys.Alt) != 0)
                    keyMessage += "Alt + ";
                if ((GlobalSettings.MouseModifiers & Keys.Control) != 0)
                    keyMessage += "Ctrl + ";
                return keyMessage;
            }
        }

        void pulsar_DragEnter(object sender, DragEventArgs e)
        {
            //context.SuppressHookMessage(true);
            Pulsar.ActivatePulsar();
        }

        void HandlePulsarMouseClick(object sender, MouseEventArgs e)
        {
            if (KeyboardInfo.ModifierKeys == Keys.Control)
                Manager.ShowStartMenuDialog(true);
            else
            {
                if (InteropHelper.IsTabletPC)
                    ChangeVisibility();
            }
        }

        void HandlePulsarMouseDoubleClick(object sender, MouseEventArgs e)
        {
            ChangeVisibility();
        }

        internal void HideManager()
        {
            if (!systemReady)
                return;

            if (context.Manager.Fading)
                return;

            if (context.Manager.PopupMenu.Active)
                context.Manager.PopupMenu.CloseUp();

            if (context.Manager.StoneMenu != null)
            {
                if (context.Manager.StoneMenu.Active)
                    context.Manager.StoneMenu.CloseUp();
            }

            context.Manager.HideAllMenus();
            context.Manager.HideRingHint();
            context.Manager.HideButtonHint();
            context.Manager.HideScreenHint();


            if (GlobalConfig.LowMemory)
            {
                context.Manager.HideAll();
                context.Manager.ChangeVisibility(false);
            }
            else
                context.Manager.Fade();

            context.Manager.RestoreFocusedWindow();

        }

        internal void ShowManager()
        {
            if (!systemReady)
                return;

            if (context.Manager.Fading)
                return;

            context.Manager.StoreFocusedWindow();

            if (!GlobalSettings.IgnoreFullScreen)
            {
                if (PrimaryScreen.IsUserPlayingFullscreen())
                    return;
            }


            if (Pulsar.Visible)
            {
                Pulsar.BringToFront();
            }


            if (GlobalConfig.LowMemory)
            {
                context.Manager.ShowAll();
                context.Manager.ChangeVisibility(true);
            }
            else
                context.Manager.Fade();

        }


        internal void ChangeVisibility()
        {
            if (!systemReady)
                return;

            if (context.Manager.Fading)
                return;

            if (context.Manager.Visible)
            {
                HideManager();
            }
            else
            {
                ShowManager();
            }

        }

        /// <summary>
        /// Activates the stones manager using mouse.
        /// </summary>
        private void ActivateUsingMouse(bool moveToCenter)
        {
            if (context.HookActive)
                return;

            if (NativeMethods.IsMetroActive())
                return;

            if (!context.Manager.Visible)
            {
                if (context.Manager.CircleLocation != CircleLocation.Fixed)
                {
                    if ((context.Manager.CircleLocation == CircleLocation.Point) || (!moveToCenter))
                    {
                        Point screenPos = PrimaryScreen.CursorPosition;
                        context.Manager.Position = screenPos;
                    }
                    else
                    {
                        Point screenPos = PrimaryScreen.Center;
                        context.Manager.Position = screenPos;
                    }
                }
            }
            ChangeVisibility();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            try
            {
                if (wasException)
                {
                    base.WndProc(ref m);
                    return;
                }


                if (!systemReady)
                {
                    if (m.Msg == NativeMethods.CM_STARTENGINE)
                    {
                        ActivateEngine();
                        return;
                    }

                    if ((m.Msg == NativeMethods.WM_CLOSE) || (m.Msg == NativeMethods.WM_QUIT) || (m.Msg == NativeMethods.WM_QUERYENDSESSION))
                    {
                        UnloadAll();
                    }
                    base.WndProc(ref m);
                    return;
                }
                else //do it only if the system is ready
                {
                    if (context.Dispatch(ref m))
                        return;

                    int code = m.Msg;
                    switch (code)
                    {
                        case NativeMethods.WM_ACTIVATEAPP:
                            // The WParam value identifies what is occurring.
                            bool appActive = (((int)m.WParam != 0));
                            {
                                if (GlobalSettings.HideOnClick)
                                {
                                    if (!appActive)
                                    {
                                        if (Manager.Visible && (!Manager.Fading))
                                        {
                                            if (!context.HookActive)
                                            {
                                                ChangeVisibility();
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        case NativeMethods.WM_CLOSE:
                        case NativeMethods.WM_QUIT:
                            this.UnloadAll();
                            break;
                        case NativeMethods.WM_QUERYENDSESSION:
                            this.UnloadAll();
                            break;
                        case NativeMethods.CM_CHECKUPDATE:
                            ShowNewVersion();
                            break;
                        case NativeMethods.WM_POWERBROADCAST:
                            if (m.WParam == (IntPtr)NativeMethods.PBT_APMRESUMEAUTOMATIC)
                            {
                                context.HidePulsar();
                                if (Settings.Default.ShowPulsar)
                                {
                                    Pulsar.RestoreAlpha();
                                    Pulsar.Alpha = 60;
                                    context.ShowPulsar();
                                    Pulsar.DeactivatePulsar();

                                }
                            }
                            break;

                        case NativeMethods.CM_RBUTTONDOWN:
                            {
                                if (GlobalSettings.UseMouseActivation)
                                {
                                    if (GlobalSettings.UseRightClick)
                                    {
                                        TraceDebug.Trace("CM_RBUTTONDOWN");
                                        if (!trackRightDown)
                                        {
                                            trackRightDown = true;
                                            if (!context.Manager.Visible)
                                            {
                                                timerIntercept.Enabled = true;
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        case NativeMethods.CM_RBUTTONUP:
                            {
                                if (GlobalSettings.UseMouseActivation)
                                {
                                    if (GlobalSettings.UseRightClick)
                                    {
                                        TraceDebug.Trace("CM_RBUTTONUP");
                                        timerIntercept.Enabled = false;

                                        if (trackRightDown)
                                        {
                                            trackRightDown = false;
                                            if (!trackRightUp)
                                            {
                                                trackRightUp = true;
                                                MouseHook.PauseIntercept();
                                                VirtualMouse.RightClick();
                                            }
                                            else
                                            {
                                                trackRightUp = false;
                                                MouseHook.ResumeIntercept();
                                            }

                                        }
                                    }
                                }
                                break;
                            }
                        case NativeMethods.CM_MBUTTONUP:
                        case NativeMethods.CM_XBUTTONUP:

                            if (GlobalSettings.UseMouseActivation)
                            {
                                int xButton = (int)m.WParam;
                                MouseHookButton hookButton = MouseHookButton.None;

                                if (m.Msg == NativeMethods.CM_MBUTTONUP)
                                    hookButton = MouseHookButton.Wheel;
                                else
                                    switch (xButton)
                                    {
                                        case NativeMethods.XBUTTON1:
                                            hookButton = MouseHookButton.XButton1;
                                            break;
                                        case NativeMethods.XBUTTON2:
                                            hookButton = MouseHookButton.XButton2;
                                            break;
                                        default:
                                            hookButton = MouseHookButton.None;
                                            break;
                                    }

                                if (GlobalSettings.MouseHook != hookButton)
                                    return;

                                Keys modifiers = (Keys)m.LParam;

                                if (GlobalSettings.MouseModifiers != modifiers)
                                    return;

                                ActivateUsingMouse(true);
                            }
                            return;
                        case NativeMethods.CM_DESKTOPCLICK:
                            if (GlobalSettings.UseMouseActivation)
                            {
                                if (Manager != null)
                                {
                                    if (context.HookActive)
                                        return;

                                    if (GlobalSettings.DesktopClick)
                                    {
                                        ActivateUsingMouse(true);
                                    }
                                }
                            }
                            return;
                        case NativeMethods.WM_HOTKEY:
                            if (NativeMethods.IsMetroActive())
                                return;
                            if (GlobalSettings.UseKeyboardActivation)
                            {
                                if (Manager != null)
                                {
                                    if (context.HookActive)
                                        return;

                                    if (GlobalSettings.CircleSelector)
                                        if (ringKeys != null)
                                        {
                                            if (((int)m.WParam) == (ringKeys.Id))
                                            {
                                                Manager.SwitchRing();
                                            }
                                        }

                                    if (hotKeys != null)
                                    {
                                        if (((int)m.WParam) == (hotKeys.Id))
                                        {
                                            if (Manager.Visible)
                                            {
                                                if (NativeMethods.GetForegroundWindow() != Manager.Handle)
                                                {
                                                    Manager.Visible = false;
                                                    Manager.Rotate(0);
                                                    ChangeVisibility();
                                                    return;
                                                }
                                            }
                                            ChangeVisibility();
                                        }
                                    }
                                }
                            }
                            return;

                        case NativeMethods.CM_SPLASHCLOSE:
                            if (splashScreen != null)
                            {
                                splashScreen.Dispose();
                                splashScreen = null;
                            }
                            break;
                        default:
                            break;
                    }
                    base.WndProc(ref m);
                }
            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex);
                if (!wasException)
                {
                    wasException = true;
                    throw new KrentoEngineException("Krento Engine Error: ", ex);
                }
            }
        }


        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (context.TrayMenu != null)
                {
                    context.TrayMenu.PopupAt(notifyMouse.X, notifyMouse.Y);
                }
            }
        }


        private void notifyIcon_MouseMove(object sender, MouseEventArgs e)
        {
            notifyMouse = PrimaryScreen.CursorPosition;
        }


        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            InteropHelper.ExitKrento();
        }

        private void timerIntercept_Tick(object sender, EventArgs e)
        {
            timerIntercept.Enabled = false;
            if (trackRightDown)
            {
                trackRightDown = false;
                trackRightUp = false;
                ActivateUsingMouse(false);
            }
        }

        private void notifier_Deactivate(object sender, EventArgs e)
        {
            notifier.Url = string.Empty;
        }


    }
}

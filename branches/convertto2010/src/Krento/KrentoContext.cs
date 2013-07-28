//===============================================================================
// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using Krento.RollingStones;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using Laugris.Sage;
using System.Globalization;
using Krento.Properties;

namespace Krento
{
    /// <summary>
    /// CrentoContext handles all operations in application context, like popup menus,
    /// actions, drag and drop operations, etc...
    /// In this case the biggest part of the code can be moved out of main form
    /// (kind of MVC model :)
    /// </summary>
    internal sealed partial class KrentoContext : IDisposable, IHookMessage, IServiceProvider, IMenuManager, IContextManager, IDropFileManager
    {
        private bool hookActive;
        private StonesManager manager;
        private Pulsar pulsar;

        private KrentoMenu pulsarMenu;
        private KrentoMenu trayMenu;
        private KrentoMenu managerMenu;

        private bool firstInstance;
        private string circleRequest;

        private ServiceProviders serviceProviders;
        private List<object> plugins;
        private IntPtr mongoose;
        private Messenger messenger;

        private Invocator invocator;

        private OneInstance oneInstance;


        public static bool FirstRun;


        /// <summary>
        /// Gets or sets a value indicating whether [first instance].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [first instance]; otherwise, <c>false</c>.
        /// </value>
        public bool FirstInstance
        {
            get { return firstInstance; }
            set { firstInstance = value; }
        }


        /// <summary>
        /// Starts the server.
        /// </summary>
        public void StartServer()
        {
            mongoose = NativeMethods.kr_start(GlobalConfig.ServerRoot, GlobalSettings.PortNumber.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void StopServer()
        {
            if (mongoose != IntPtr.Zero)
            {
                try
                {
                    NativeMethods.mg_stop(mongoose);
                }
                finally
                {
                    mongoose = IntPtr.Zero;
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="KrentoContext"/> class.
        /// </summary>
        public KrentoContext()
        {
            oneInstance = new OneInstance();
            oneInstance.UniqueAppStr = "Krento";
            oneInstance.InstanceMessageReceived += new EventHandler<OneInstanceEventArgs>(oneInstance_InstanceMessageReceived);
            
            if (!string.IsNullOrEmpty(PackageParameter))
                oneInstance.Execute(PackageParameter);
            else
                oneInstance.Execute(CircleParameter);

            if (oneInstance.FirstInstance)
            {
                firstInstance = true;
                plugins = new List<object>();

                serviceProviders = new ServiceProviders();

                messenger = new Messenger();
                invocator = new Invocator();

                messenger.RegisterMessage("krento_show", KrentoShowHandler);
                messenger.RegisterMessage("krento_hide", KrentoHideHandler);
                messenger.RegisterMessage("krento_about", KrentoAboutHandler);
                messenger.RegisterMessage("krento_options", KrentoOptionsHandler);
                messenger.RegisterMessage("krento_close", KrentoCloseHandler);
                messenger.RegisterMessage("pulsar_hide", PulsarHideHandler);
                messenger.RegisterMessage("pulsar_show", PulsarShowHandler);
                messenger.RegisterMessage("krento_help", KrentoHelpHandler);
                messenger.RegisterMessage("krento_portable", KrentoPortableHandler);


                manager = new StonesManager();
                managerMenu = CreateManagerMenu();
                manager.PopupMenu = managerMenu;
                manager.ActivateMouse = GlobalSettings.ActivateCursor;
                manager.AboutBoxShow += new EventHandler(manager_AboutBoxShow);
                manager.CircleLoaded += new EventHandler(manager_CircleLoaded);

                trayMenu = CreateTrayMenu();

                CreatePulsar();

                serviceProviders.AddService(typeof(IMenuManager), this);
                serviceProviders.AddService(typeof(IContextManager), this);
            }
        }

        private void oneInstance_InstanceMessageReceived(object sender, OneInstanceEventArgs e)
        {
            if (string.IsNullOrEmpty(e.FileName))
            {
                if (GlobalSettings.ShowPopupAlerts)
                {
                    if (MainForm != null)
                    {
                        MainForm.notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                        MainForm.notifyIcon.BalloonTipText = SR.KrentoIsRunning;
                        MainForm.notifyIcon.ShowBalloonTip(2000);
                    }
                }
            }
            else
            {
                if (FileOperations.FileExtensionIs(e.FileName, ".circle"))
                {
                    CircleParameter = e.FileName;
                    LoadNewCircleFile(e.FileName);
                }
                else
                {
                    
                    if (MainForm != null)
                    MainForm.InstallKrentoPart(e.FileName);
                }
            }
        }

        public void CreatePulsar()
        {
            pulsar = new Pulsar();
            pulsarMenu = CreatePulsarMenu();
            pulsar.PopupMenu = pulsarMenu;
        }

        public bool Dispatch(ref Message m)
        {
            return messenger.Dispatch(ref m);
        }

        /// <summary>
        /// Creates the tray icon popup menu.
        /// </summary>
        /// <returns>Krento Tray Icon Popup Menu</returns>
        private KrentoMenu CreateTrayMenu()
        {
            KrentoMenuItem item;

            KrentoMenu menu = new KrentoMenu();
            item = menu.AddItem(KrentoMenuTag.About);
            item.Caption = SR.PulsarAbout;
            item.Name = "About";

            item = menu.AddItem(KrentoMenuTag.Help, (Keys.F1));
            item.Caption = SR.PulsarHelp;
            item.Name = "Help";

            item = menu.AddItem(KrentoMenuTag.Settings, (Keys.Control | Keys.E));
            item.Caption = SR.PulsarOptions;
            item.Name = "Options";

            item = menu.AddItem(KrentoMenuTag.ShowManager);
            item.Caption = SR.ManagerShow;
            item.Name = "ShowManager";

            item = menu.AddItem(KrentoMenuTag.Applications);
            item.Caption = SR.Applications;
            item.Name = "Applications";

            item = menu.AddItem(KrentoMenuTag.ShowPulsar);
            item.Caption = SR.PulsarShow;
            item.Name = "ShowPulsar";

            item = menu.AddItem(KrentoMenuTag.CloseKrento, (Keys.Alt | Keys.X));
            item.Caption = SR.PulsarClose;
            item.Name = "CloseKrento";

            menu.NumberOfVisibleItems = 100;
            menu.MenuClick += new EventHandler<KrentoMenuArgs>(HanleTrayIconMenu);
            menu.BeforePopup += new EventHandler(TrayMenu_BeforePopup);
            return menu;
        }

        public void ShowKrentoSettings()
        {
            this.SuppressHookMessage(true);
            try
            {
                ShowSettingsDialog();
            }
            finally
            {
                this.SuppressHookMessage(false);
            }
        }

        void HanleTrayIconMenu(object sender, KrentoMenuArgs e)
        {
            switch (e.MenuItem.Tag)
            {
                case KrentoMenuTag.Help:
                    ShowHelpPages();
                    break;

                case KrentoMenuTag.Applications:
                    manager.ShowStartMenuDialog(true);
                    break;
                case KrentoMenuTag.Settings:
                    ShowKrentoSettings();
                    break;
                case KrentoMenuTag.About:
                    ShowAboutBox();
                    break;
                case KrentoMenuTag.ShowManager:
                    MainForm.ChangeVisibility();
                    break;
                case KrentoMenuTag.ShowPulsar:
                    if (pulsar.Visible)
                        HidePulsar();
                    else
                        ShowPulsar();
                    break;
                case KrentoMenuTag.CloseKrento:
                    {
                        Manager.HideAll();
                        MainForm.Close();
                        break;
                    }
            }
        }


        void TrayMenu_BeforePopup(object sender, EventArgs e)
        {
            KrentoMenuItem item = trayMenu.FindItemByTag(KrentoMenuTag.ShowManager);
            if (item != null)
            {
                if (manager.Visible)
                    item.Caption = SR.ManagerHide;
                else
                    item.Caption = SR.ManagerShow;
            }

            item = trayMenu.FindItemByTag(KrentoMenuTag.ShowPulsar);
            if (item != null)
            {
                if (pulsar.Visible)
                    item.Caption = SR.PulsarHide;
                else
                    item.Caption = SR.PulsarShow;
            }
        }


        /// <summary>
        /// Handles the CircleLoaded event of the manager control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void manager_CircleLoaded(object sender, EventArgs e)
        {
            GlobalSettings.CircleName = manager.CurrentCircle;
        }


        public static void AssociateRingsWithKrento()
        {
#if !PORTABLE
            try
            {
                if (!FileAssociation.IsAssociated(".circle"))
                {
                    string exeName = Application.ExecutablePath.ToString();
                    string resName = Path.Combine(Path.GetDirectoryName(exeName), "Laugris.Standard.dll");
                    FileAssociation.Associate(".circle", "Krento.Circle", "Krento Circle",
                        resName, exeName, 1);
                }
            }
            catch (Exception ex)
            {
                // no problem, just continue
                TraceDebug.Trace("AssociateRingsWithKrento exception",  ex);
            }
#endif
        }


        public static bool SkipDocklets { get; set; }

        public static bool SkipToys {get; set;}

        public static bool CreateDesktopCircle {get; set;}

        /// <summary>
        /// Gets or sets the circle name command line parameter.
        /// </summary>
        /// <value>The circle name command line parameter.</value>
        public static string CircleParameter {get; set;}

        public static string PackageParameter {get; set;}

        private void manager_AboutBoxShow(object sender, EventArgs e)
        {
            Type baseType = typeof(Krento.RollingStones.RollingStoneBase);
            if (GenericHelper.IsSubclassOfRawGeneric(baseType, sender.GetType()))
            {
                ShowAboutBox((RollingStoneBase)sender);
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="KrentoContext"/> is reclaimed by garbage collection.
        /// </summary>
        ~KrentoContext()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the stones manager.
        /// </summary>
        public StonesManager Manager
        {
            get { return this.manager; }
        }

        /// <summary>
        /// Gets the pulsar.
        /// </summary>
        public Pulsar Pulsar
        {
            get { return this.pulsar; }
        }

        /// <summary>
        /// Gets the tray menu.
        /// </summary>
        public KrentoMenu TrayMenu
        {
            get { return this.trayMenu; }
        }


        /// <summary>
        /// Gets the main form.
        /// </summary>
        /// <value>The main form.</value>
        public static MainForm MainForm
        {
            get { return KrentoStartup.MainForm; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hook active].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [hook active]; otherwise, <c>false</c>.
        /// </value>
        public bool HookActive
        {
            get { return hookActive; }
            set { hookActive = value; }
        }


        /// <summary>
        /// Creates the pulsar menu.
        /// </summary>
        /// <returns></returns>
        private KrentoMenu CreatePulsarMenu()
        {
            KrentoMenuItem item;
            KrentoMenu menu = new KrentoMenu();

            item = menu.AddItem(KrentoMenuTag.About);
            item.Caption = SR.PulsarAbout;
            item.Name = "About";

            item = menu.AddItem(KrentoMenuTag.Help, (Keys.F1));
            item.Caption = SR.PulsarHelp;
            item.Name = "Help";

            item = menu.AddItem(KrentoMenuTag.Settings, (Keys.Control | Keys.E));
            item.Caption = SR.PulsarOptions;
            item.Name = "Options";

            item = menu.AddItem(KrentoMenuTag.ShowManager);
            item.Caption = SR.ManagerShow;
            item.Name = "ShowManager";

            item = menu.AddItem(KrentoMenuTag.Applications);
            item.Caption = SR.Applications;
            item.Name = "Applications";

            item = menu.AddItem(KrentoMenuTag.ShowPulsar);
            item.Caption = SR.PulsarShow;
            item.Name = "ShowPulsar";

            item = menu.AddItem();
            item.Caption = "-";

            item = menu.AddItem(HandleInstallStoneCommand);
            item.Caption = SR.InstallStone;

            item = menu.AddItem(HandleInstallToyCommand);
            item.Caption = SR.InstallToy;

            item = menu.AddItem(HandleInstallSkinCommand);
            item.Caption = SR.InstallKrentoSkin;

            item = menu.AddItem();
            item.Caption = "-";

            item = menu.AddItem(KrentoMenuTag.CloseKrento, (Keys.Alt | Keys.X));
            item.Caption = SR.PulsarClose;
            item.Name = "CloseKrento";

            menu.NumberOfVisibleItems = menu.Items.Count;
            menu.MenuClick += new EventHandler<KrentoMenuArgs>(HandlePulsarMenu);
            menu.BeforePopup += new EventHandler(pulsarMenu_BeforePopup);
            if (Pulsar.PopupMenu != null)
            {
                Pulsar.PopupMenu.Dispose();
                Pulsar.PopupMenu = null;
            }
            return menu;
        }

        private void HandleInstallStoneCommand(object sender, EventArgs e)
        {
            if (manager.Visible)
                manager.Visible = false;

            OpenFileDialog dialog = new OpenFileDialog();
            try
            {
                try
                {
                    dialog.InitialDirectory = GlobalConfig.DownloadsFolder;
                    dialog.Filter = SR.StonesFilter;
                }
                catch
                {
                    dialog.Filter = @"Krento Stones (*.stone)|*.stone";
                    TraceDebug.Trace("HandleInstallStoneCommand exception in getting the dialog filter");
                }
                if (dialog.ShowDialog(MainForm) == DialogResult.OK)
                {
                    try
                    {
                        MainForm.InstallKrentoPart(dialog.FileName);
                    }
                    catch
                    {
                        //do not stop here
                        TraceDebug.Trace("HandleInstallStoneCommand exception in installing Krento part");
                    }
                }
            }
            finally
            {
                dialog.Dispose();
            }
        }

        private void HandleInstallToyCommand(object sender, EventArgs e)
        {
            if (manager.Visible)
                manager.Visible = false;

            OpenFileDialog dialog = new OpenFileDialog();
            try
            {
                try
                {
                    dialog.InitialDirectory = GlobalConfig.DownloadsFolder;
                    dialog.Filter = SR.ToysFilter;
                }
                catch
                {
                    dialog.Filter = @"Toys Files (*.toy;*.docklet)|*.toy;*.docklet";
                }
                if (dialog.ShowDialog(MainForm) == DialogResult.OK)
                {
                    try
                    {
                        MainForm.InstallKrentoPart(dialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        //do not stop here
                        TraceDebug.Trace("HandleInstallToyCommand exception", ex);
                    }
                }
            }
            finally
            {
                dialog.Dispose();
            }
        }

        private void HandleInstallSkinCommand(object sender, EventArgs e)
        {
            if (manager.Visible)
                manager.Visible = false;

            OpenFileDialog dialog = new OpenFileDialog();
            try
            {
                try
                {
                    dialog.InitialDirectory = GlobalConfig.DownloadsFolder;
                    dialog.Filter = SR.KrentoSkinFilter;
                }
                catch
                {
                    dialog.Filter = @"Krento Skins (*.kskin)|*.kskin";
                }
                if (dialog.ShowDialog(MainForm) == DialogResult.OK)
                {
                    try
                    {
                        MainForm.InstallKrentoPart(dialog.FileName);
                    }
                    catch
                    {
                        //do not stop here
                    }
                }
            }
            finally
            {
                dialog.Dispose();
            }
        }


        private void pulsarMenu_BeforePopup(object sender, EventArgs e)
        {
            KrentoMenuItem item = pulsarMenu.FindItemByTag(KrentoMenuTag.ShowManager);
            if (item != null)
            {
                if (manager.Visible)
                    item.Caption = SR.ManagerHide;
                else
                    item.Caption = SR.ManagerShow;
            }

            item = pulsarMenu.FindItemByTag(KrentoMenuTag.ShowPulsar);
            if (item != null)
            {
                if (pulsar.Visible)
                    item.Caption = SR.PulsarHide;
                else
                    item.Caption = SR.PulsarShow;
            }
        }

        /// <summary>
        /// Creates the manager menu.
        /// </summary>
        /// <returns></returns>
        private KrentoMenu CreateManagerMenu()
        {
            KrentoMenuItem item;

            KrentoMenu popupMenu;
            popupMenu = new KrentoMenu();

            item = popupMenu.AddItem(KrentoMenuTag.Help, (Keys.F1));
            item.Caption = SR.PulsarHelp;
            item.Name = "Help";

            item = popupMenu.AddItem(KrentoMenuTag.Settings, (Keys.Control | Keys.E));
            item.Caption = SR.PulsarOptions;
            item.Name = "Options";

            item = popupMenu.AddItem();
            item.Caption = "-";

            item = popupMenu.AddItem(KrentoMenuTag.SelectCircle, (Keys.Shift | Keys.Alt | Keys.O));
            item.Caption = SR.SelectCircle;
            item.Name = "SelectCircle";

            item = popupMenu.AddItem(KrentoMenuTag.TaskManager);
            item.Caption = SR.TaskManager;
            item.Name = "TaskManager";

            item = popupMenu.AddItem(KrentoMenuTag.DeleteCircle, (Keys.Shift | Keys.Delete));
            item.Caption = SR.Delete;
            item.Name = "DeleteCircle";

            item = popupMenu.AddItem(KrentoMenuTag.EditCircle);
            item.Caption = SR.Modify;
            item.Name = "EditCircle";

            item = popupMenu.AddItem(KrentoMenuTag.CreateCircle, (Keys.Control | Keys.N));
            item.Caption = SR.CreateCircle;
            item.Name = "CreateCircle";

            item = popupMenu.AddItem(KrentoMenuTag.AddStone, (Keys.Control | Keys.A));
            item.Caption = SR.AddStone;
            item.Name = "AddStone";

            item = popupMenu.AddItem();
            item.Caption = "-";

            item = popupMenu.AddItem(KrentoMenuTag.ChangeSkin, (Keys.Alt | Keys.S));
            item.Caption = SR.KrentoNewSkin;
            item.Name = "ChangeSkin";

            item = popupMenu.AddItem();
            item.Caption = "-";

            item = popupMenu.AddItem(KrentoMenuTag.PrevCircle, (Keys.Control | Keys.B));
            item.Caption = SR.PrevCircle;
            item.Name = "PrevCircle";

            item = popupMenu.AddItem(KrentoMenuTag.NextCircle, (Keys.Control | Keys.F));
            item.Caption = SR.NextCircle;
            item.Name = "NextCircle";

            item = popupMenu.AddItem();
            item.Caption = "-";

            item = popupMenu.AddItem(KrentoMenuTag.CloseKrento, (Keys.Alt | Keys.X));
            item.Caption = SR.PulsarClose;
            item.Name = "CloseKrento";

            popupMenu.NumberOfVisibleItems = 100;
            popupMenu.MenuClick += new EventHandler<KrentoMenuArgs>(ManagerMenuClick);
            popupMenu.BeforePopup += new EventHandler(ManagerMenu_BeforePopup);
            return popupMenu;
        }

        void ManagerMenu_BeforePopup(object sender, EventArgs e)
        {
            bool hasHistory;

            if (manager.HistoryCount < 2)
                hasHistory = false;
            else
                hasHistory = true;


            KrentoMenuItem item = managerMenu.FindItemByTag(KrentoMenuTag.PrevCircle);
            if (item != null)
            {
                item.Enabled = hasHistory;
            }

            item = managerMenu.FindItemByTag(KrentoMenuTag.NextCircle);
            if (item != null)
            {
                item.Enabled = hasHistory;
            }

            item = managerMenu.FindItemByTag(KrentoMenuTag.AddStone);
            if (item != null)
            {
                item.Enabled = !Manager.IsVirtual;
            }

        }

        /// <summary>
        /// Loads the new circle file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void LoadNewCircleFile(string fileName)
        {

            if (!FileOperations.FileExists(fileName))
                return;

            circleRequest = fileName;
            HandleCircleRequest();
        }

        internal void HandleCircleRequest()
        {
            if (string.IsNullOrEmpty(circleRequest))
                return;
            try
            {
                string newCircleName = circleRequest;
                if ((manager.CurrentCircle != newCircleName) || (manager.IsVirtual))
                {
                    manager.Visible = false;
                    string oldName = manager.CurrentCircle;
                    manager.LoadCircle(newCircleName, FileOperations.FileExists(oldName));
                    if (!FileOperations.FileExists(oldName))
                        manager.HistoryRemove(oldName);


                    //manager.Visible = true;
                    MainForm.ChangeVisibility();
                    manager.BringToFront();
                }

            }
            finally
            {
                circleRequest = null;
            }
        }

        public void SelectPreviousCircle()
        {
            manager.HistoryBack();
        }

        public void SelectNextCircle()
        {
            manager.HistoryForward();
        }

        private void ManagerMenuClick(object sender, KrentoMenuArgs e)
        {

            switch (e.MenuItem.Tag)
            {
                case KrentoMenuTag.AddStone:
                    manager.AddNewStone();
                    break;
                case KrentoMenuTag.Help:
                    ShowHelpPages();
                    break;
                case KrentoMenuTag.Settings:
                    ShowKrentoSettings();
                    break;
                case KrentoMenuTag.SelectCircle:
                    manager.SwitchRing();
                    break;
                case KrentoMenuTag.TaskManager:
                    manager.ShowRunningApplications();
                    break;
                case KrentoMenuTag.NextCircle:
                    SelectNextCircle();
                    break;
                case KrentoMenuTag.PrevCircle:
                    SelectPreviousCircle();
                    break;
                case KrentoMenuTag.DeleteCircle:
                    DeleteCircle();
                    break;
                case KrentoMenuTag.EditCircle:
                    EditCircle();
                    break;
                case KrentoMenuTag.CreateCircle:
                    CreateCircle();
                    break;
                case KrentoMenuTag.ChangeSkin:
                    manager.ShowSkinsMenu();
                    break;
                case KrentoMenuTag.CloseKrento:
                    {
                        Manager.HideAll();
                        MainForm.Close();
                        break;
                    }
            }
        }


        public void DeleteCircle()
        {
            if (manager.HistoryCount < 2)
                RtlAwareMessageBox.Show(SR.ErrorDeleteSingleRing, SR.KrentoShortName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
            {
                string fileName = manager.CurrentCircle;
                if (!string.IsNullOrEmpty(fileName))
                {
                    manager.HistoryForward();
                    FileOperations.DeleteFile(fileName);
                    manager.HistoryRemove(fileName);
                }
            }
        }

        public void EditCircle()
        {
            string oldName;
            this.SuppressHookMessage(true);
            try
            {
                RingSettingsDialog settingsDialog = new RingSettingsDialog();
                oldName = manager.CurrentCircle;
                settingsDialog.FileName = manager.CurrentCircle;

                if (settingsDialog.ShowDialog() == DialogResult.OK)
                {
                    if (settingsDialog.FileName != oldName)
                    {
                        FileOperations.RenameFile(oldName, settingsDialog.FileName);
                        manager.HistoryRemove(oldName);
                        manager.HistoryAdd(settingsDialog.FileName);
                        manager.CurrentCircle = settingsDialog.FileName;
                        manager.RepaintHintWindow();
                    }

                    if (settingsDialog.DefaultRing)
                        GlobalConfig.DefaultCircleName = settingsDialog.FileName;

                    Manager.History[settingsDialog.FileName].Description = settingsDialog.Description;
                    Manager.History[settingsDialog.FileName].LogoFile = settingsDialog.LogoFile;
                    Manager.History[settingsDialog.FileName].Save();
                }

                settingsDialog.Dispose();
            }
            finally
            {
                this.SuppressHookMessage(false);
            }
        }

        public void CreateCircle()
        {
            RingSettingsDialog settingsDialog;

            this.SuppressHookMessage(true);
            try
            {
                settingsDialog = new RingSettingsDialog();

                bool okOver = true;

                if (settingsDialog.ShowDialog(MainForm) == DialogResult.OK)
                {
                    string newCircleName = settingsDialog.FileName;
                    if (!string.IsNullOrEmpty(newCircleName))
                    {

                        if (FileOperations.IsValidPathName(newCircleName))
                        {
                            if (FileOperations.FileExists(newCircleName))
                            {
                                okOver = (RtlAwareMessageBox.Show(SR.WarningRingExists, SR.KrentoShortName, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK);

                            }

                            if (okOver)
                            {
                                if (manager.CurrentCircle != newCircleName)
                                {
                                    settingsDialog.Save();
                                    manager.Visible = false;
                                    manager.LoadCircle(newCircleName);
                                    manager.CreateEmptyCircle();
                                    SaveCurrentCircle();
                                    MainForm.ChangeVisibility();
                                    if (settingsDialog.DefaultRing)
                                        GlobalConfig.DefaultCircleName = newCircleName;
                                }
                                else
                                    RtlAwareMessageBox.Show(SR.ErrorOverWriteCurrentCircle, SR.KrentoShortName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            RtlAwareMessageBox.Show(SR.InvalidCircleName, SR.KrentoShortName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    } //new circle name is not empty
                    else
                        RtlAwareMessageBox.Show(SR.InvalidCircleName, SR.KrentoShortName, MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                settingsDialog.Dispose();
                manager.BringToFront();
            }
            finally
            {
                this.SuppressHookMessage(false);
            }
        }

        /// <summary>
        /// Shows the help pages.
        /// </summary>
        public void ShowHelpPages()
        {
            string serverURL;
            string helpURL;
            string fileName;

            if (Manager.Visible)
                Manager.Visible = false;

            fileName = GlobalConfig.IndexPage;

            if (GlobalSettings.ActivateServer)
            {
                serverURL = InteropHelper.CurrentIPAddress;

                helpURL = string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}/{2}", new string[3] { serverURL, GlobalSettings.PortNumber.ToString(CultureInfo.InvariantCulture), fileName });
            }
            else
            {
                helpURL = string.Format(CultureInfo.InvariantCulture, "file:///{0}/{1}", new string[2] { Path.Combine(GlobalConfig.ApplicationFolder, GlobalConfig.ServerRoot), fileName });
            }
            AsyncShellExecute ase = new AsyncShellExecute(helpURL);
            ase.Run();
        }

        /// <summary>
        /// Handles the pulsar menu.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        public void HandlePulsarMenu(object sender, KrentoMenuArgs e)
        {
            switch (e.MenuItem.Tag)
            {
                case KrentoMenuTag.Help:
                    ShowHelpPages();
                    break;
                case KrentoMenuTag.Applications:
                    manager.ShowStartMenuDialog(true);
                    break;
                case KrentoMenuTag.Settings:
                    ShowKrentoSettings();
                    break;
                case KrentoMenuTag.About:
                    ShowAboutBox();
                    break;
                case KrentoMenuTag.ShowManager:
                    MainForm.ChangeVisibility();
                    break;
                case KrentoMenuTag.ShowPulsar:
                    if (pulsar.Visible)
                        HidePulsar();
                    else
                        ShowPulsar();
                    break;
                case KrentoMenuTag.CloseKrento:
                    {
                        Manager.HideAll();
                        MainForm.Close();
                        break;
                    }
            }

        }

        /// <summary>
        /// Shows the settings dialog and save the settings changes
        /// </summary>
        private void ShowSettingsDialog()
        {
            bool restartNeeded = false;
            RollingStoneTask stone;

            if (manager.Visible)
                manager.Visible = false;

            SettingsDialog sd = new SettingsDialog();
            DialogResult result = sd.ShowDialog(MainForm);
            if (result == DialogResult.OK)
            {
#if !PORTABLE
                try
                {
                    InteropHelper.SetStartup("Krento", sd.RunWithWindows);
                }
                catch (Exception ex)
                {
                    TraceDebug.Trace(ex);
                }
#endif
                manager.CircleLocation = sd.CircleLocation;

                if (sd.UseDefaultKeys)
                {
                    GlobalSettings.Shortcut = Keys.S;
                    GlobalSettings.Modifiers = Keys.LWin;
                }
                else
                {
                    if (sd.Shortcut == Keys.None)
                    {
                        GlobalSettings.Shortcut = Keys.S;
                        GlobalSettings.Modifiers = Keys.LWin;
                    }
                    else
                    {
                        GlobalSettings.Shortcut = sd.Shortcut;
                        GlobalSettings.Modifiers = sd.ModifierHotKeys;
                    }
                }

                GlobalSettings.ShowSplashScreen = sd.ShowSplashScreen;
                GlobalSettings.ShowTrayIcon = sd.ShowTrayIcon;
                MainForm.notifyIcon.Visible = GlobalSettings.ShowTrayIcon;

                GlobalSettings.ShowPopupAlerts = sd.ShowPopupAlerts;

                GlobalSettings.MouseModifiers = sd.MouseModifiers;

                if (!TextHelper.SameText(GlobalSettings.Language, sd.Language))
                {
                    GlobalSettings.Language = sd.Language;
                    restartNeeded = true;
                }

                if (GlobalSettings.IconSize != sd.IconSize)
                {
                    FileOperations.ClearCacheFolder();
                    restartNeeded = true;
                }

                //if (GlobalSettings.ShowManagerButtons != sd.ShowManagerButtons)
                //{
                //    restartNeeded = true;
                //}

                GlobalSettings.Radius = sd.Radius;
                GlobalSettings.WindowWidth = sd.WindowWidth;
                GlobalSettings.WindowHeight = sd.WindowHeight;
                GlobalSettings.Transparency = sd.Transparency;
                GlobalSettings.FadeDelay = sd.FadeDelay;
                GlobalSettings.StoneSize = sd.StoneSize;
                GlobalSettings.IconSize = sd.IconSize;
                GlobalSettings.MenuSkin = sd.MenuSkin;
                GlobalSettings.ShowStoneHint = sd.ShowStoneHint;
                GlobalSettings.DefaultStonesNumber = sd.StonesNumber;
                GlobalSettings.UseSound = sd.UseSounds;
                GlobalSettings.HideOnClick = sd.HideOnClick;
                GlobalSettings.RotateOnClick = sd.RotateOnClick;
                GlobalSettings.CheckUpdate = sd.CheckUpdate;
                GlobalSettings.LiveReflection = sd.LiveReflection;
                GlobalSettings.DesktopClick = sd.DesktopClick;
                GlobalSettings.UseRightClick = sd.UseRightClick;
                GlobalSettings.CircleSelector = sd.CircleSelector;
                GlobalSettings.UseKeyboardActivation = sd.UseKeyboardActivation;
                GlobalSettings.UseMouseActivation = sd.UseMouseActivation;
                GlobalSettings.ShowManagerButtons = sd.ShowManagerButtons;

                Manager.ChangeMenuSkin(Manager.MenuSkin);
                //KrentoMenu.SkinFileName = FileOperations.StripFileName(GlobalSettings.MenuSkin);

                MainForm.PrepareKeyboardHook();

                GlobalSettings.MouseHook = sd.MouseHookButton;

                MainForm.PrepareMouseHook();

                SaveManagerSettings();
                GlobalSettings.SaveGlobalSettings();
                ApplyManagerParameters();
                //Reload saved parameters
                manager.LoadSettings(GlobalConfig.KrentoSettingsFileName);
                if (sd.ReloadCache)
                {
                    for (int i = 0; i < manager.Count; i++)
                    {
                        if (manager.Stones[i] is RollingStoneTask)
                        {
                            stone = (RollingStoneTask)manager.Stones[i];
                            stone.DestroyLogoImage();
                            stone.FixupLogoImage();
                        }
                        
                    }
                }
            }
            else
            {
                if (result == DialogResult.Ignore)
                {
                    GlobalSettings.SaveGlobalSettings();
                    RestartKrento();
                }
            }
            sd.Dispose();
            if (restartNeeded)
                RestartKrento();
        }

        public void RestartKrento()
        {
            Manager.HideAll();
            MainForm.Close();
            FileExecutor.ProcessExecute(Path.Combine(GlobalConfig.ApplicationFolder, "KrentoCommander.exe"), "/restart", null);
        }

        public void ApplyManagerParameters()
        {
            manager.StoneSize = GlobalSettings.StoneSize;
            manager.FadeDelay = GlobalSettings.FadeDelay;
            manager.Radius = GlobalSettings.Radius;
            manager.WindowWidth = GlobalSettings.WindowWidth;
            manager.WindowHeight = GlobalSettings.WindowHeight;
            manager.Transparency = GlobalSettings.Transparency;
            manager.RotateOnClick = GlobalSettings.RotateOnClick;
            manager.DefaultStonesNumber = GlobalSettings.DefaultStonesNumber;
            manager.LiveReflection = GlobalSettings.LiveReflection;
            manager.ShowStoneHint = GlobalSettings.ShowStoneHint;
        }

        /// <summary>
        /// Shows the about box.
        /// </summary>
        internal void ShowAboutBox()
        {
            this.SuppressHookMessage(true);
            try
            {
                if (manager.Visible)
                    manager.Visible = false;

                AboutBox aboutBox = new AboutBox();
                try
                {
                    aboutBox.Show();
                }
                finally
                {
                    aboutBox.Dispose();
                    aboutBox = null;
                }
            }
            finally
            {
                this.SuppressHookMessage(false);
            }
        }

        internal void ShowAboutBox(RollingStoneBase stone)
        {
            this.SuppressHookMessage(true);
            try
            {
                if (manager.Visible)
                    manager.Visible = false;

                AboutBox aboutBox = new AboutBox(stone);
                try
                {
                    aboutBox.Show(stone);
                }
                finally
                {
                    aboutBox.Dispose();
                    aboutBox = null;
                }
            }
            finally
            {
                this.SuppressHookMessage(false);
            }
        }

        internal void ShowPulsar()
        {
            Pulsar.Show();
        }

        internal void HidePulsar()
        {
            Pulsar.Hide();
        }

        #region Invoke support
        /// <summary>
        /// Begins the invoke.
        /// </summary>
        /// <param name="method">The method.</param>
        public void BeginInvoke(MethodInvoker method)
        {
            invocator.BeginInvoke(method);
        }

        /// <summary>
        /// Begins the invoke.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="errorMessage">The error message.</param>
        public void BeginInvoke(MethodInvoker method, string errorMessage)
        {
            invocator.BeginInvoke(method, errorMessage);
        }

        /// <summary>
        /// Invokes the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        public void Invoke(MethodInvoker method)
        {
            invocator.Invoke(method);
        }

        /// <summary>
        /// Invokes the specified method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="errorMessage">The error message.</param>
        public void Invoke(MethodInvoker method, string errorMessage)
        {
            invocator.Invoke(method, errorMessage);
        }
        #endregion


        #region IDisposable Members

        internal void FreeAllResources()
        {
            if (manager != null)
            {
                manager.AboutBoxShow -= manager_AboutBoxShow;
                manager.CircleLoaded -= manager_CircleLoaded;
                manager.Dispose();
                manager = null;
            }

            if (managerMenu != null)
            {
                managerMenu.Dispose();
                managerMenu = null;
            }

            if (trayMenu != null)
            {
                trayMenu.Dispose();
                trayMenu = null;
            }

            if (pulsarMenu != null)
            {
                pulsarMenu.Dispose();
                pulsarMenu = null;
            }

            if (pulsar != null)
            {
                pulsar.Dispose();
                pulsar = null;
            }

        }

        private void Dispose(bool disposing)
        {
            SuppressHookMessage(true);


            if (disposing)
            {

                FreeAllResources();

                if (messenger != null)
                {
                    messenger.Clear();
                    messenger = null;
                }

                if (invocator != null)
                {
                    invocator.Dispose();
                    invocator = null;
                }

                Language.Close();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region IHookMessage Members

        public void SuppressHookMessage(bool value)
        {
            HookActive = value;
        }

        public void SaveCurrentCircle()
        {
            try
            {
                Invoke(new MethodInvoker(SaveCurrentCircleDelegate));
            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex);
                throw new SaveCurrentCircleException("Save current circle error", ex);
            }
        }

        public void SaveManagerSettings()
        {
            try
            {
                Invoke(new MethodInvoker(SaveManagerSettingsDelegate));
            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex);
                throw new ManagerSettingsException("Save manager settings error", ex);
            }

        }

        /// <summary>
        /// Changes the current circle.
        /// </summary>
        /// <param name="circleName">Name of the circle.</param>
        public void ChangeCurrentCircle(string circleName)
        {
            circleRequest = circleName;
            try
            {
                Invoke(new MethodInvoker(HandleCircleRequest));
            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex);
                throw new ChangeCircleException("Change circle error", ex);
            }
        }

        #endregion

        /// <summary>
        /// Loads Krento plugins.
        /// </summary>
        internal void LoadKrentoPlugins()
        {
            Assembly assembly = null;
            Type[] types;
            Type package;
            object obj;

            string[] files = Directory.GetFiles(GlobalConfig.AddInRootFolder, "*.dll", SearchOption.AllDirectories);

            foreach (string fileName in files)
            {
                try
                {
                    if (NativeMethods.IsAssembly(fileName))
                        assembly = Assembly.LoadFile(fileName);
                    else
                        assembly = null;
                }
                catch (BadImageFormatException)
                {
                    //this is not a .NET assembly, skip it
                    continue;
                }

                if (assembly == null)
                    continue;

                types = assembly.GetExportedTypes();
                foreach (Type t in types)
                {
                    package = t.GetInterface("IPackage", true);
                    if (package != null)
                    {
                        obj = assembly.CreateInstance(t.FullName);
                        if (obj != null)
                        {
                            Language.Merge(Path.Combine(Path.GetDirectoryName(fileName), GlobalSettings.Language + ".lng"));
                            ((IPackage)obj).Load(this);
                            plugins.Add(obj);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unloads the krento plugins.
        /// </summary>
        internal void UnloadKrentoPlugins()
        {
            if (plugins != null)
            {
                for (int i = 0; i < plugins.Count; i++)
                {
                    ((IPackage)plugins[i]).Unload();
                }
            }
        }




        /// <summary>
        /// Loads the krento toys.
        /// </summary>
        internal void LoadKrentoToys()
        {
            if (SkipToys)
                return;

            LoadKrentoToys(GlobalConfig.ToysFolder);
        }


        internal void LoadKrentoToys(string baseFolder)
        {
            string[] files = Directory.GetFiles(baseFolder, "config.ini", SearchOption.AllDirectories);
            foreach (string fileName in files)
            {
                try
                {
                    LoadKrentoToy(fileName);
                }
                catch (Exception ex)
                {
                    //we need to catch exception here and Krento can continue to load other toys
                    TraceDebug.Trace(ex);
                }
            }
        }

        private void LoadKrentoToy(string fileName)
        {
            try
            {
                ToyThread toyThreadLauncher = new ToyThread(fileName);
                Thread toyThread = new Thread(new ThreadStart(toyThreadLauncher.Load));
                toyThread.IsBackground = true;
                toyThread.Priority = ThreadPriority.Lowest;
                toyThread.Start();

            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex);
            }

        }

        /// <summary>
        /// Unloads the krento toys.
        /// </summary>
        internal void UnloadKrentoToys()
        {
        }

        /// <summary>
        /// Loads the krento docklets.
        /// </summary>
        internal void LoadKrentoDocklets()
        {
            if (SkipDocklets)
                return;

            LoadKrentoDocklets(GlobalConfig.DockletsFolder);

        }

        internal void LoadKrentoDocklets(string baseFolder)
        {
            string[] files = Directory.GetFiles(baseFolder, "config.ini", SearchOption.AllDirectories);

            foreach (string fileName in files)
            {
                try
                {
                    DockletThread docketLauncher = new DockletThread(fileName);
                    Thread docketThread = new Thread(new ThreadStart(docketLauncher.Load));
                    docketThread.IsBackground = true;
                    docketThread.Priority = ThreadPriority.Lowest;
                    docketThread.Start();

                }
                catch (Exception ex)
                {
                    TraceDebug.Trace(ex);
                }
            }
        }

        /// <summary>
        /// Unloads the krento docklets.
        /// </summary>
        internal void UnloadKrentoDocklets()
        {
            KrentoStartup.BroadcastShutdownMessage();
        }

        internal void UnloadRingImage()
        {
            KrentoRing.DisposeDefaultRingImage();
        }

        #region IServiceProvider Members

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.-or- null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return serviceProviders.GetService(serviceType);
        }

        #endregion

        #region IMenuManager Members

        IKrentoMenu IMenuManager.GetMenu(string name)
        {
            if (TextHelper.SameText(name, "Pulsar"))
                return (IKrentoMenu)pulsarMenu;
            if (TextHelper.SameText(name, "Tray"))
                return (IKrentoMenu)pulsarMenu;
            if (TextHelper.SameText(name, "Manager"))
                return (IKrentoMenu)managerMenu;

            return null;
        }

        IKrentoMenu IMenuManager.GetPulsarMenu()
        {
            return (IKrentoMenu)pulsarMenu;
        }

        IKrentoMenu IMenuManager.GetManagerMenu()
        {
            return (IKrentoMenu)managerMenu;
        }

        IKrentoMenu IMenuManager.GetTrayMenu()
        {
            return (IKrentoMenu)trayMenu;
        }

        #endregion

        #region IContextManager Members

        Pulsar IContextManager.GetPulsarInstance()
        {
            return pulsar;
        }

        StonesManager IContextManager.GetStonesManagerInstance()
        {
            return manager;
        }

        #endregion


        #region IDropFileManager Members

        void IDropFileManager.RegisterDropFileHandler(string extension, PulsarDropFileHandler handler)
        {
            try
            {
                MainForm.DropFileHandler.Add(extension, handler);
            }
            catch
            {
            }
        }

        void IDropFileManager.RemoveDropFileHandler(string extension)
        {
            try
            {
                if (MainForm.DropFileHandler.ContainsKey(extension))
                {
                    MainForm.DropFileHandler.Remove(extension);
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}

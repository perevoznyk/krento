using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Laugris.Sage;
using Krento.RollingStones;
using System.IO;
using System.Globalization;

namespace Krento
{
    internal partial class SettingsDialog : AeroForm
    {
        private Keys modifierHotKeys;
        private Keys shortcut;
        private Panel activeWorkplace;
        private string language;
        private string menuSkinFile;
        private int languageIndex;
        private int menuSkinIndex;
        private int idx;
        private int goodNumber;

        public SettingsDialog()
        {
            InitializeComponent();
        }


        private void SettingsDialog_Load(object sender, EventArgs e)
        {
            CultureInfo culture;
            DockItem item;
            Image icon;
            fish.UseDenomination = true;
            fish.Settings.MaxScale = 1.5;
            fish.Settings.ScaleCaption = false;
            fish.ItemsLayout = ItemsLayout.Default;
            fish.Settings.LeftMargin = 12;
            fish.Settings.IconsSpacing = 34;
            fish.DockOrientation = DockOrientation.Vertical;

            icon = NativeThemeManager.Load("SmallKrento.png");

            item = fish.AddItem(SR.General, icon);
            item.Tag = 1;
            icon.Dispose();

            icon = NativeThemeManager.Load("exec.png");
            item = fish.AddItem(SR.Tuning, icon);
            item.Tag = 2;
            icon.Dispose();

            icon = BitmapPainter.ResizeBitmap(KrentoRing.DefaultRingImage, 48, 48, false);
            item = fish.AddItem(SR.Circle, icon);
            item.Tag = 3;
            icon.Dispose();

            activeWorkplace = workplaceGeneral;

            workplaceGeneral.Visible = true;
            workplaceManager.Visible = false;
            workplaceTuning.Visible = false;

            workplaceGeneral.BringToFront();
#if PORTABLE
            edtRunWindows.Enabled = false;
            btnCircle.Enabled = false;
#else
            edtRunWindows.Checked = InteropHelper.GetStartup("Krento");
#endif
            if ((GlobalSettings.Modifiers == Keys.LWin) && (GlobalSettings.Shortcut == Keys.S))
            {
                edtUseDefault.Checked = true;
                edtHotKey.Enabled = false;
            }
            else
            {
                edtUseDefault.Checked = false;
                edtHotKey.Enabled = true;
                this.modifierHotKeys = GlobalSettings.Modifiers;
                this.shortcut = GlobalSettings.Shortcut;
            }

            switch (GlobalSettings.MouseHook)
            {
                case MouseHookButton.Wheel:
                    rbWheel.Checked = true;
                    break;
                case MouseHookButton.XButton1:
                    rbXButton1.Checked = true;
                    break;
                case MouseHookButton.XButton2:
                    rbXButton2.Checked = true;
                    break;
            }


            edtShiftButton.Checked = ((GlobalSettings.MouseModifiers & Keys.Shift) != 0);
            edtAltButton.Checked = ((GlobalSettings.MouseModifiers & Keys.Alt) != 0);
            edtControlButton.Checked = ((GlobalSettings.MouseModifiers & Keys.Control) != 0);

            edtShowHintWindow.Checked = GlobalSettings.ShowStoneHint;
            edtShowAlerts.Checked = GlobalSettings.ShowPopupAlerts;
            edtTrayIcon.Checked = GlobalSettings.ShowTrayIcon;
            edtShowSplashScreen.Checked = GlobalSettings.ShowSplashScreen;
            edtManagerButtons.Checked = GlobalSettings.ShowManagerButtons;
            edtUseSounds.Checked = GlobalSettings.UseSound;
            edtHideOnClick.Checked = GlobalSettings.HideOnClick;
            edtRotateOnClick.Checked = GlobalSettings.RotateOnClick;
            edtCheckUpdates.Checked = GlobalSettings.CheckUpdate;
            edtLiveReflection.Checked = GlobalSettings.LiveReflection;
            edtDesktopClick.Checked = GlobalSettings.DesktopClick;
            edtRightClick.Checked = GlobalSettings.UseRightClick;
            edtCircleSelector.Checked = GlobalSettings.CircleSelector;
            edtUseMouse.Checked = GlobalSettings.UseMouseActivation;
            edtKeyboardActivation.Checked = GlobalSettings.UseKeyboardActivation;

            edtStonesNumber.Value = GlobalSettings.DefaultStonesNumber;
            goodNumber = Math.Min(GlobalSettings.FadeDelay, (int)edtFadeDelay.Maximum);
            edtFadeDelay.Value = Math.Max(goodNumber, edtFadeDelay.Minimum);

            goodNumber = Math.Min(GlobalSettings.Radius, (int)edtRadius.Maximum);
            edtRadius.Value = Math.Max(goodNumber, edtRadius.Minimum);

            goodNumber = Math.Min(GlobalSettings.WindowHeight, (int)edtWindowHeight.Maximum);
            edtWindowHeight.Value = Math.Max(goodNumber, edtWindowHeight.Minimum);

            goodNumber = Math.Min(GlobalSettings.WindowWidth, (int)edtWindowWidth.Maximum);
            edtWindowWidth.Value = Math.Max(goodNumber, edtWindowWidth.Minimum);

            goodNumber = Math.Min(GlobalSettings.Transparency, (int)edtTransparency.Maximum);
            edtTransparency.Value = Math.Max(goodNumber, edtTransparency.Minimum);

            goodNumber = Math.Min(GlobalSettings.StoneSize, (int)edtStoneSize.Maximum);
            edtStoneSize.Value = Math.Max(goodNumber, edtStoneSize.Minimum);

            goodNumber = Math.Min(GlobalSettings.IconSize, (int)edtIconSize.Maximum);
            edtIconSize.Value = Math.Max(goodNumber, edtIconSize.Minimum);

            switch (KrentoContext.MainForm.Manager.CircleLocation)
            {
                case CircleLocation.Point:
                    rbMousePosition.Checked = true;
                    break;
                case CircleLocation.ScreenCenter:
                    rbScreenCenter.Checked = true;
                    break;
                case CircleLocation.Fixed:
                    rbFixed.Checked = true;
                    break;
                default:
                    break;
            }

            if (NativeMethods.IsWow64())
            {
                edtDesktopClick.Enabled = false;
            }

            cbLanguage.ValueMember = "NativeName"; //"EnglishName"
            cbLanguage.Items.Add(new CultureInfo("en-US"));
            languageIndex = 0;

            string[] langFiles = Directory.GetFiles(GlobalConfig.LanguagesFolder, "*.lng");
            foreach (string lang in langFiles)
            {
                string langName = Path.GetFileNameWithoutExtension(lang);
                if (!TextHelper.SameText(langName, "en-US"))
                {
                    try
                    {
                        culture = new CultureInfo(langName);
                    }
                    catch
                    {
                        culture = null;
                    }

                    if (culture != null)
                    {
                        idx = cbLanguage.Items.Add(culture);
                        if (TextHelper.SameText(langName, GlobalSettings.Language))
                            languageIndex = idx;
                    }
                }
            }

            
            cbLanguage.SelectedIndex = languageIndex;

            KrentoSkinInfo defaultSkin = new KrentoSkinInfo(null, SR.DefaultSkin);

            cbMenuSkins.Items.Add(defaultSkin);
            menuSkinIndex = 0;

            string GlobalMenuSkinFile = "";
            if (!string.IsNullOrEmpty(GlobalSettings.MenuSkin))
                GlobalMenuSkinFile = FileOperations.StripFileName(GlobalSettings.MenuSkin);
            string[] skinFiles = Directory.GetFiles(GlobalConfig.MenusFolder, "*.ini", SearchOption.AllDirectories);
            foreach (string skinFile in skinFiles)
            {
                KrentoSkinInfo skin = new KrentoSkinInfo(skinFile, KrentoMenuSkin.GetSkinCaption(skinFile));
                idx = cbMenuSkins.Items.Add(skin);
                if (TextHelper.SameText(skin.FileName, GlobalMenuSkinFile))
                    menuSkinIndex = idx;
            }
            cbMenuSkins.ValueMember = "Caption";
            cbMenuSkins.SelectedIndex = menuSkinIndex;
            menuSkinFile = GlobalSettings.MenuSkin;

            this.Text = SR.Settings;
            btnCancel.Text = SR.Cancel;
            this.hintInfo.SetToolTip(this.btnCancel, SR.FileConfigCancelHint);
            btnOK.Text = SR.OK;
            gbRunWindows.Text = SR.KrentoStartup;
            edtRunWindows.Text = SR.RunWithWindows;
            gbKeyboard.Text = SR.KeyboardActivation;
            edtUseDefault.Text = SR.UseDefaultKey;
            gbMouseActivation.Text = SR.MouseActivation;
            hintInfo.SetToolTip(gbMouseActivation, SR.MouseActivation);
            edtControlButton.Text = SR.ButtonControl;
            this.hintInfo.SetToolTip(this.edtControlButton, SR.ButtonControl);
            edtShiftButton.Text = SR.ButtonShift;
            this.hintInfo.SetToolTip(this.edtShiftButton, SR.ButtonShift);
            edtAltButton.Text = SR.ButtonAlt;
            this.hintInfo.SetToolTip(this.edtAltButton, SR.ButtonAlt);
            gbRingLocation.Text = SR.RingLocation;
            rbScreenCenter.Text = SR.ScreenCenter;
            rbMousePosition.Text = SR.MouseCursorPosition;
            btnCircle.Text = SR.AssociateRing;
            lblHint.Text = SR.UseCtrlTab;
            rbXButton1.Text = SR.MouseActionOneButton;
            rbXButton2.Text = SR.MouseActionTwoButton;
            rbWheel.Text = SR.MouseWheelClick;
            gbPersonalization.Text = SR.Personalization;
            gbUserFolders.Text = SR.UserFolders;
            gbMaintainance.Text = SR.Maintainance;
            edtTrayIcon.Text = SR.ShowTrayIcon;
            edtShowSplashScreen.Text = SR.ShowSplashScreen;
            edtShowAlerts.Text = SR.ShowPopupAlerts;
            btnOpenSkins.Text = SR.OpenSkinsFolder;
            btnOpenStones.Text = SR.OpenStonesFolder;
            btnOpenData.Text = SR.OpenDataFolder;
            btnOpenCache.Text = SR.OpenCacheFolder;
            btnClearCache.Text = SR.ClearCache;
            gbLanguage.Text = SR.Language;
            lblWindowHeight.Text = SR.WindowHeight;
            lblWindowWidth.Text = SR.WindowWidth;
            lblRadius.Text = SR.Radius;
            lblStoneSize.Text = SR.StoneSize;
            lblTransparency.Text = SR.Trancparency;
            lblFadeDelay.Text = SR.FadeDelay;
            gbMenuSkins.Text = SR.MenuSkins;
            gbManager.Text = SR.Circle;
            btnReset.Text = SR.ResetToDefault;
            btnBackup.Text = SR.BackupData;
            btnSettings.Text = SR.AdvancedSettings + "...";
            edtKeyboardActivation.Text = SR.ActivateUsingKeyboard;
            edtUseMouse.Text = SR.ActivateUsingMouse;
            rbFixed.Text = SR.FixedPosition;
            edtCircleSelector.Text = SR.ShowSelector;
            edtDesktopClick.Text = SR.DesktopClick;
            edtRightClick.Text = SR.RightButtonActivation;
            lblStonesNumber.Text = SR.StonesNumber;
            lblIconSize.Text = SR.IconSize;
            edtRotateOnClick.Text = SR.RotateOnClick;
            edtShowCircleStartup.Text = SR.ActivateOnStart;
            edtUseSounds.Text = SR.UseSounds;
            edtCheckUpdates.Text = SR.CheckUpdates;
            edtLiveReflection.Text = SR.LiveReflection;
            edtManagerButtons.Text = SR.ShowManagerButtons;
            edtShowHintWindow.Text = SR.ShowStonesHint;
            edtHideOnClick.Text = SR.HideOnClick;
        }

        public string Language
        {
            get { return language; }
        }

        public bool ShowTrayIcon
        {
            get { return edtTrayIcon.Checked; }
        }

        public bool CircleSelector
        {
            get { return edtCircleSelector.Checked; }
        }

        public bool ShowPopupAlerts
        {
            get { return edtShowAlerts.Checked; }
        }

        public bool ShowSplashScreen
        {
            get { return edtShowSplashScreen.Checked; }
        }

        public int StonesNumber
        {
            get { return (int)edtStonesNumber.Value; }
        }

        public bool ShowStoneHint
        {
            get { return edtShowHintWindow.Checked; }
        }

        public bool LiveReflection
        {
            get { return edtLiveReflection.Checked; }
        }

        public bool ShowManagerButtons
        {
            get { return edtManagerButtons.Checked; }
        }

        public bool HideOnClick
        {
            get { return edtHideOnClick.Checked; }
        }

        public bool RotateOnClick
        {
            get { return edtRotateOnClick.Checked; }
        }

        public bool UseSounds
        {
            get { return edtUseSounds.Checked; }
        }

        public bool DesktopClick
        {
            get { return edtDesktopClick.Checked; }
        }

        public bool CheckUpdate
        {
            get { return edtCheckUpdates.Checked; }
        }

        public bool UseKeyboardActivation
        {
            get { return edtKeyboardActivation.Checked; }
        }

        public bool UseMouseActivation
        {
            get { return edtUseMouse.Checked; }
        }

        public CircleLocation CircleLocation
        {
            get
            {
                if (rbMousePosition.Checked)
                    return CircleLocation.Point;
                if (rbFixed.Checked)
                    return CircleLocation.Fixed;
                return CircleLocation.ScreenCenter;
            }
        }

        public bool RunWithWindows
        {
            get { return edtRunWindows.Checked; }
        }

        public bool UseRightClick
        {
            get { return edtRightClick.Checked; }
        }

        public Keys Shortcut
        {
            get { return this.shortcut; }
        }

        public Keys ModifierHotKeys
        {
            get { return this.modifierHotKeys; }
        }

        public bool UseDefaultKeys
        {
            get { return edtUseDefault.Checked; }
        }

        private void btnCircle_Click(object sender, EventArgs e)
        {
#if !PORTABLE
            KrentoContext.AssociateRingsWithKrento();
#endif
        }


        public MouseHookButton MouseHookButton
        {
            get
            {
                if (rbWheel.Checked)
                    return MouseHookButton.Wheel;
                if (rbXButton1.Checked)
                    return MouseHookButton.XButton1;
                if (rbXButton2.Checked)
                    return MouseHookButton.XButton2;
                return MouseHookButton.None;
            }
        }

        /// <summary>
        /// Gets the mouse modifiers keys.
        /// </summary>
        /// <value>The mouse modifiers keys.</value>
        public Keys MouseModifiers
        {
            get
            {
                Keys mouseModifiers = Keys.None;
                if (edtAltButton.Checked)
                    mouseModifiers = mouseModifiers | Keys.Alt;
                if (edtControlButton.Checked)
                    mouseModifiers = mouseModifiers | Keys.Control;
                if (edtShiftButton.Checked)
                    mouseModifiers = mouseModifiers | Keys.Shift;
                return mouseModifiers;
            }
        }

        private void SettingsDialog_Shown(object sender, EventArgs e)
        {
            NativeMethods.BringWindowToFront(this.Handle);
            this.BringToFront();
            edtRunWindows.Select();
            edtRunWindows.Focus();

            edtHotKey.SetHotKeyValue(this.modifierHotKeys, this.shortcut);

        }

        private void edtUseDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (edtUseDefault.Checked)
                edtHotKey.Enabled = false;
            else
                edtHotKey.Enabled = true;
        }

        private void edtHotKey_TextChanged(object sender, EventArgs e)
        {
            this.shortcut = edtHotKey.HotKey;
            this.modifierHotKeys = edtHotKey.HotModifierKeys;
        }


        private void fish_SelectItem(object sender, DockItemEventArgs e)
        {
            switch (e.Item.Tag)
            {
                case 2:
                    workplaceTuning.Visible = true;
                    workplaceTuning.BringToFront();
                    workplaceGeneral.Visible = false;
                    workplaceManager.Visible = false;
                    activeWorkplace = workplaceTuning;
                    break;
                case 3:
                    workplaceManager.Visible = true;
                    workplaceManager.BringToFront();
                    workplaceGeneral.Visible = false;
                    workplaceTuning.Visible = false;
                    activeWorkplace = workplaceManager;
                    break;
                default:
                    workplaceGeneral.Visible = true;
                    workplaceGeneral.BringToFront();
                    workplaceManager.Visible = false;
                    workplaceTuning.Visible = false;
                    activeWorkplace = workplaceGeneral;
                    break;
            }
        }

        private void btnOpenSkins_Click(object sender, EventArgs e)
        {
            FileExecutor.Execute(GlobalConfig.SkinsFolder);
        }

        private void btnOpenData_Click(object sender, EventArgs e)
        {
#if PORTABLE
            FileExecutor.Execute(GlobalConfig.MainFolder);
#else
            FileExecutor.Execute(GlobalConfig.UserSpecificDataFolder);
#endif
        }

        private void btnOpenStones_Click(object sender, EventArgs e)
        {
            FileExecutor.Execute(GlobalConfig.RollingStonesFolder);
        }

        private void btnOpenCache_Click(object sender, EventArgs e)
        {
            FileExecutor.Execute(GlobalConfig.RollingStonesCache);
        }

        public bool ReloadCache { get; set; }

        private void btnClearCache_Click(object sender, EventArgs e)
        {
            FileOperations.ClearCacheFolder();
            ReloadCache = true;
        }

        private void SettingsDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.KeyCode == Keys.Tab))
            {
                if (activeWorkplace == workplaceGeneral)
                {
                    workplaceTuning.Visible = true;
                    workplaceTuning.BringToFront();
                    workplaceGeneral.Visible = false;
                    workplaceManager.Visible = false;
                    activeWorkplace = workplaceTuning;
                }
                else
                    if (activeWorkplace == workplaceTuning)
                    {
                        workplaceManager.Visible = true;
                        workplaceManager.BringToFront();
                        workplaceGeneral.Visible = false;
                        workplaceTuning.Visible = false;
                        activeWorkplace = workplaceManager;
                    }
                    else
                    {
                        workplaceGeneral.Visible = true;
                        workplaceGeneral.BringToFront();
                        workplaceManager.Visible = false;
                        workplaceTuning.Visible = false;
                        activeWorkplace = workplaceGeneral;
                    }
            }
        }

        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            language = ((CultureInfo)cbLanguage.Items[cbLanguage.SelectedIndex]).Name;
        }


        public int Radius
        {
            get { return (int)edtRadius.Value; }
        }

        public int StoneSize
        {
            get { return (int)edtStoneSize.Value; }
        }

        public int IconSize
        {
            get { return (int)edtIconSize.Value; }
        }

        public int FadeDelay
        {
            get { return (int)edtFadeDelay.Value; }
        }

        public int Transparency
        {
            get { return (int)edtTransparency.Value; }
        }

        public int WindowWidth
        {
            get { return (int)edtWindowWidth.Value; }
        }

        public int WindowHeight
        {
            get { return (int)edtWindowHeight.Value; }
        }

        public string MenuSkin
        {
            get { return menuSkinFile; }
        }

        private void cbMenuSkins_SelectedIndexChanged(object sender, EventArgs e)
        {
            menuSkinFile = ((KrentoSkinInfo)cbMenuSkins.Items[cbMenuSkins.SelectedIndex]).FileName;
        }

        private void SettingsDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            cbMenuSkins.Items.Clear();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            edtRadius.Value = 200;
            edtWindowHeight.Value = 80;
            edtWindowWidth.Value = 300;
            edtStoneSize.Value = 128;
            edtIconSize.Value = 64;
            edtTransparency.Value = 230;
            edtFadeDelay.Value = 100;
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            string ZipFileToCreate = GlobalConfig.BackupFileName;

            this.Cursor = Cursors.WaitCursor;
            FileOperations.CreateBackup(ZipFileToCreate);
            this.Cursor = Cursors.Default;
            FileExecutor.Execute(GlobalConfig.BackupFolder);
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            GlobalInspector inspector = new GlobalInspector();
            inspector.propertyGrid.SelectedObject = new StaticPropertyHelper(typeof(GlobalSettings));
            DialogResult inspectorResult = inspector.ShowDialog();
            inspector.Dispose();
            if (inspectorResult == DialogResult.OK)
                this.DialogResult = DialogResult.Ignore;
        }

    }
}

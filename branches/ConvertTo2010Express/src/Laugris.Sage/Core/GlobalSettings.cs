using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.ComponentModel;

namespace Laugris.Sage
{
    /// <summary>
    /// This class keeps all parameters from Krento.ini file
    /// It's needed to avoid reading the file every time when parameter is needed
    /// </summary>
    public static class GlobalSettings
    {

        [Browsable(true)]
        [Category(Sections.Mouse)]
        [Description("The name of the mouse button which is assigned for activating the Krento Application Ring")]
        internal static MouseHookButton MouseHook { get; set; }

        [Browsable(true)]
        [Category(Sections.Mouse)]
        [Description("Keyboard shortcut which is used together with the mouse button for activating the Krento Application Ring")]
        internal static Keys MouseModifiers { get; set; }

        [Browsable(true)]
        [Category(Sections.Shortcut)]
        [Description("Key modifiers for activating Krento using the keyboard")]
        internal static Keys Modifiers { get; set; }

        [Browsable(true)]
        [Category(Sections.Shortcut)]
        [Description("Keyboard key for activating Krento")]
        internal static Keys Shortcut { get; set; }

        [Browsable(true)]
        [Category(Sections.Shortcut)]
        [Description("Select Krento ring using keyboard")]
        internal static bool CircleSelector { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("The name of the icon file. By default Krento uses the embedded icon")]
        internal static string IconName { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("The name of the image file for the alerts popup window.Uses for the branding of Krento")]
        internal static string GlyphName { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("The name of the image file for the Splash Screen background. Uses for the branding of Krento")]
        internal static string SplashName { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("The name of the image file for the About Box background. Uses for the branding of Krento")]
        internal static string AboutBoxName { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("The name of the image file for the Stone About Box background. Uses for the branding of Krento")]
        internal static string AboutStoneName { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("The name of the current Krento Circle file")]
        public static string CircleName { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("The name of the default Krento Circle file")]
        public static string DefaultCircle { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [DefaultValue(false)]
        [Description("Show the Krento Application Ring when Krento starts. The default value is false")]
        public static bool StartVisible { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("Check for the Krento update at the startup. If this parameter is set to true, Krento will connect to the Internet to check if the new version is available")]
        public static bool CheckUpdate { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [DefaultValue(true)]
        [Description("Show or hide the Krento tray icon")]
        public static bool ShowTrayIcon { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [DefaultValue(true)]
        [Description("Show the alerts popup box. The default value is true")]
        public static bool ShowPopupAlerts { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [DefaultValue(true)]
        [Description("Show the Krento Splash Screen when Krento starts. The default value is true")]
        public static bool ShowSplashScreen { get; set; }

        [Browsable(false)]
        [Category(Sections.Menu)]
        [Description("The name of the menu skin file")]
        public static string MenuSkin { get; set; }

        [Browsable(true)]
        [Category(Sections.Help)]
        [DefaultValue(8053)]
        [Description("Default port number of the built-in webserver. The default value is 8053")]
        public static int PortNumber { get; set; }

        [Browsable(true)]
        [Category(Sections.Help)]
        [DefaultValue(true)]
        [Description("Control built-in webserver. When the built-in webserver is activated you can browse the help pages via the browser")]
        public static bool ActivateServer { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("Krento user interface translation language ISO code")]
        public static string Language { get; set; }

        [Browsable(true)]
        [Category(Sections.General)]
        [Description("Use sound for the user input events")]
        public static bool UseSound { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(false)]
        [Description("Rotate the application ring every time when the stone is executed. The default value is false")]
        public static bool RotateOnClick { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(100)]
        [Description("Application Ring fade duration. The default value is 100")]
        public static int FadeDelay { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(200)]
        [Description("The application Ring radius. The default value is 200")]
        public static int Radius { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(230)]
        [Description("The transparency of the stones. The default value is 230")]
        public static int Transparency { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(300)]
        [Description("The stones manager window width. The default value is 300")]
        public static int WindowWidth { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(80)]
        [Description("The stones manager window height. The default value is 80")]
        public static int WindowHeight { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(128)]
        [Description("The size of the stone in pixels. The default value is 128")]
        public static int StoneSize { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(64)]
        [Description("The size of the stone image in pixels. The default value is 64")]
        public static int IconSize { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(false)]
        [Description("Hide the Application Ring if you are clicked outside the application ring. the default value is false")] 
        public static bool HideOnClick { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [DefaultValue(false)]
        [Description("Flat Krento ring. The default value is false")]
        public static bool FlatRing { get; set; }

        [Browsable(true)]
        [Category(Sections.Mouse)]
        [DefaultValue(false)]
        [Description("Move the cursor to the center of the stones manager window when activating the application ring using the keyboard shortcut. The default value is false")]
        public static bool ActivateCursor { get; set; }

        [Browsable(true)]
        [Category(Sections.Mouse)]
        [DefaultValue(true)]
        [Description("Activate the Application Ring by pressing and holding the right mouse button. The default value is true")]
        public static bool UseRightClick { get; set; }

        [Browsable(true)]
        [Category(Sections.Mouse)]
        [DefaultValue(false)]
        [Description("Activate Krento by double click on the Windows desktop")]
        public static bool DesktopClick { get; set; }

        [DefaultValue(true)]
        public static bool IgnoreFullScreen { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [Description("Show buttons inside the manager window. Restart Krento to apply this parameter")]
        public static bool ShowManagerButtons { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [Description("The number of empty stones when new circle is created. Default is 0")]
        public static int DefaultStonesNumber { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [Description("Show live icon reflection. Default is false")]
        public static bool LiveReflection { get; set; }

        [Browsable(true)]
        [Category(Sections.Manager)]
        [Description("Show stone hint. Default is true")]
        public static bool ShowStoneHint { get; set; }

        [Browsable(false)]
        public static bool UseKeyboardActivation { get; set; }

        [Browsable(false)]
        public static bool UseMouseActivation { get; set; }

        public static int ManagerLeft { get; set; }
        public static int ManagerTop { get; set; }

        private static class Sections
        {
            public const string Shortcut = "Shortcut";
            public const string General = "General";
            public const string Mouse = "Mouse";
            public const string Help = "Help";
            public const string Manager = "Options";
            public const string Menu = "Menu";
        }

        private static class Parameters
        {
            public const string ShowSplashScreen = "ShowSplashScreen";
            public const string PortNumber = "PortNumber";
            public const string ActivateServer = "ActivateServer";
            public const string FadeDelay = "FadeDelay";
            public const string HideOnClick = "HideOnClick";
            public const string Language = "Language";
            public const string Theme = "Theme";
            public const string Raduis = "Radius";
            public const string WindowWidth = "WindowWidth";
            public const string WindowHeight = "WindowHeight";
            public const string Transparency = "Transparency";
            public const string StoneSize = "StoneSize";
            public const string IconSize = "IconSize";
            public const string Alt = "Alt";
            public const string Win = "Win";
            public const string Ctrl = "Ctrl";
            public const string Shift = "Shift";
            public const string Key = "Key";
            public const string MouseButton = "MouseButton";
            public const string CircleName = "CircleName";
            public const string DefaultCircle = "DefaultCircle";
            public const string ShowTrayIcon = "ShowTrayIcon";
            public const string ShowPopupAlerts = "ShowPopupAlerts";
            public const string StartVisible = "StartVisible";
            public const string CheckUpdate = "CheckUpdate";
            public const string RotateOnClick = "RotateOnClick";
            public const string IconName = "IconName";
            public const string GlyphName = "GlyphName";
            public const string SplashName = "SplashName";
            public const string AboutBoxName = "AboutBoxName";
            public const string AboutStoneName = "AboutStoneName";
            public const string ActivateCursor = "ActivateCursor";
            public const string UseRightClick = "UseRightClick";
            public const string IgnoreFullScreen = "IgnoreFullScreen";
            public const string ManagerButtons = "ManagerButtons";
            public const string UseSound = "UseSound";
            public const string DefaultStonesNumber = "DefaultStonesNumber";
            public const string LiveReflection = "LiveReflection";
            public const string ShowStoneHint = "ShowStoneHint";
            public static string DesktopClick = "DesktopClick";
            public static string CircleSelector = "CircleSelector";
            public static string UseKeyboardActivation = "UseKeyboardActivation";
            public static string UseMouseActivation = "UseMouseActivation";
            public static string FlatRing = "FlatRing";
        }


        internal static void LoadGlobalSettings()
        {
            MemIniFile iniFile;
            bool keyAlt;
            bool keyCtrl;
            bool keyShift;
            bool keyWin;
            int goodNumber;

            iniFile = new MemIniFile(GlobalConfig.KrentoSettingsFileName);
            try
            {
                iniFile.Load();
                try
                {
                    MouseHook = (MouseHookButton)Enum.Parse(typeof(MouseHookButton), iniFile.ReadString(Sections.General, "MouseButton", "Wheel"));
                }
                catch
                {
                    MouseHook = MouseHookButton.Wheel;
                }

                try
                {
                    Shortcut = (Keys)Enum.Parse(typeof(Keys), iniFile.ReadString(Sections.Shortcut, Parameters.Key, "S"));
                }
                catch
                {
                    Shortcut = Keys.S;
                }

                CircleSelector = iniFile.ReadBool(Sections.Shortcut, Parameters.CircleSelector, false);
                UseKeyboardActivation = iniFile.ReadBool(Sections.General, Parameters.UseKeyboardActivation, true);
                UseMouseActivation = iniFile.ReadBool(Sections.General, Parameters.UseMouseActivation, true);

                keyAlt = iniFile.ReadBool(Sections.Shortcut, Parameters.Alt, false);
                keyCtrl = iniFile.ReadBool(Sections.Shortcut, Parameters.Ctrl, false);
                keyWin = iniFile.ReadBool(Sections.Shortcut, Parameters.Win, true);
                keyShift = iniFile.ReadBool(Sections.Shortcut, Parameters.Shift, false);

                if ((Shortcut == Keys.None))
                {
                    keyAlt = false;
                    keyWin = true;
                    keyCtrl = false;
                    keyShift = false;
                    Shortcut = Keys.S;
                }

                //for previous version compatibility
                if (keyWin && (Shortcut == Keys.C))
                    Shortcut = Keys.S;

                Modifiers = Keys.None;
                if (keyAlt)
                    Modifiers = Modifiers | Keys.Alt;
                if (keyCtrl)
                    Modifiers = Modifiers | Keys.Control;
                if (keyShift)
                    Modifiers = Modifiers | Keys.Shift;
                if (keyWin)
                    Modifiers = Modifiers | Keys.LWin;


                keyAlt = iniFile.ReadBool(Sections.Mouse, Parameters.Alt, false);
                keyCtrl = iniFile.ReadBool(Sections.Mouse, Parameters.Ctrl, false);
                keyShift = iniFile.ReadBool(Sections.Mouse, Parameters.Shift, false);

                MouseModifiers = Keys.None;
                if (keyAlt)
                    MouseModifiers = MouseModifiers | Keys.Alt;
                if (keyCtrl)
                    MouseModifiers = MouseModifiers | Keys.Control;
                if (keyShift)
                    MouseModifiers = MouseModifiers | Keys.Shift;

                ActivateCursor = iniFile.ReadBool(Sections.Mouse, Parameters.ActivateCursor, false);
                
                UseRightClick = iniFile.ReadBool(Sections.Mouse, Parameters.UseRightClick, false);
                DesktopClick = iniFile.ReadBool(Sections.Mouse, Parameters.DesktopClick, false);

                DefaultCircle = iniFile.ReadString(Sections.General, Parameters.DefaultCircle, GlobalConfig.DefaultCircleName);
                DefaultCircle = FileOperations.StripFileName(DefaultCircle);

                GlobalConfig.DefaultCircleName = DefaultCircle;

                CircleName = iniFile.ReadString(Sections.General, Parameters.CircleName, DefaultCircle);
                CircleName = FileOperations.StripFileName(CircleName);

                StartVisible = iniFile.ReadBool(Sections.General, Parameters.StartVisible, false);

                CheckUpdate = iniFile.ReadBool(Sections.General, Parameters.CheckUpdate, true);

                ShowPopupAlerts = iniFile.ReadBool(Sections.General, Parameters.ShowPopupAlerts, true);
                ShowTrayIcon = iniFile.ReadBool(Sections.General, Parameters.ShowTrayIcon, true);
                ShowSplashScreen = iniFile.ReadBool(Sections.General, Parameters.ShowSplashScreen, true);
                Language = iniFile.ReadString(Sections.General, Parameters.Language, CultureInfo.CurrentCulture.Name);
                UseSound = iniFile.ReadBool(Sections.General, Parameters.UseSound, true);

                IconName = iniFile.ReadString(Sections.General, Parameters.IconName, null);
                GlyphName = iniFile.ReadString(Sections.General, Parameters.GlyphName, null);
                SplashName = iniFile.ReadString(Sections.General, Parameters.SplashName, null);
                AboutBoxName = iniFile.ReadString(Sections.General, Parameters.AboutBoxName, null);
                AboutStoneName = iniFile.ReadString(Sections.General, Parameters.AboutStoneName, null);

                // read web server settings

                PortNumber = iniFile.ReadInteger(Sections.Help, Parameters.PortNumber, 8053);
                ActivateServer = iniFile.ReadBool(Sections.Help, Parameters.ActivateServer, true);

                HideOnClick = iniFile.ReadBool(Sections.Manager, Parameters.HideOnClick, false);
                FadeDelay = iniFile.ReadInteger(Sections.Manager, Parameters.FadeDelay, 100);
                WindowWidth = iniFile.ReadInteger(Sections.Manager, Parameters.WindowWidth, 300);
                WindowHeight = iniFile.ReadInteger(Sections.Manager, Parameters.WindowHeight, 80);
                RotateOnClick = iniFile.ReadBool(Sections.Manager, Parameters.RotateOnClick, false);
                IgnoreFullScreen = iniFile.ReadBool(Sections.Manager, Parameters.IgnoreFullScreen, true);
                
                DefaultStonesNumber = iniFile.ReadInteger(Sections.Manager, Parameters.DefaultStonesNumber, 0);
                goodNumber = Math.Max(DefaultStonesNumber, 0);
                DefaultStonesNumber = Math.Min(64, goodNumber);

                StoneSize = iniFile.ReadInteger(Sections.Manager, Parameters.StoneSize, 128);
                goodNumber = Math.Min(StoneSize, 512);
                StoneSize = Math.Max(64, goodNumber);

                IconSize = iniFile.ReadInteger(Sections.Manager, Parameters.IconSize, 64);
                goodNumber = Math.Min(IconSize, 512);
                IconSize = Math.Max(64, goodNumber);
                FileImage.ImageSize = IconSize;

                Transparency = iniFile.ReadInteger(Sections.Manager, Parameters.Transparency, 230);
                goodNumber = Math.Min(Transparency, 255);
                Transparency = Math.Max(10, goodNumber);

                Radius = iniFile.ReadInteger(Sections.Manager, Parameters.Raduis, 200);
                goodNumber = Math.Min(Radius, 1000);
                Radius = Math.Max(100, goodNumber);

                ShowManagerButtons = iniFile.ReadBool(Sections.Manager, Parameters.ManagerButtons, true);
                LiveReflection = iniFile.ReadBool(Sections.Manager, Parameters.LiveReflection, false);
                ShowStoneHint = iniFile.ReadBool(Sections.Manager, Parameters.ShowStoneHint, true);
                FlatRing = iniFile.ReadBool(Sections.Manager, Parameters.FlatRing, false);

                MenuSkin = iniFile.ReadString(Sections.Menu, Parameters.Theme, string.Empty);
                KrentoMenu.SkinFileName = FileOperations.StripFileName(MenuSkin);
            }
            finally
            {
                iniFile.Dispose();
            }

        }


        internal static void SaveGlobalSettings()
        {
            bool keyAlt;
            bool keyCtrl;
            bool keyShift;
            bool keyWin;

            MemIniFile iniFile;
            iniFile = new MemIniFile(GlobalConfig.KrentoSettingsFileName);
            try
            {
                iniFile.Load();
                iniFile.WriteString(Sections.General, Parameters.MouseButton, MouseHook.ToString());

                if (!string.IsNullOrEmpty(IconName))
                    iniFile.WriteString(Sections.General, Parameters.IconName, IconName);

                string shortCircleName = FileOperations.UnExpandPath(CircleName);
                iniFile.WriteString(Sections.General, Parameters.CircleName, shortCircleName);

                iniFile.WriteBool(Sections.Shortcut, Parameters.CircleSelector, CircleSelector);
                iniFile.WriteBool(Sections.General, Parameters.UseKeyboardActivation, UseKeyboardActivation);
                iniFile.WriteBool(Sections.General, Parameters.UseMouseActivation, UseMouseActivation);

                iniFile.WriteBool(Sections.General, Parameters.ShowTrayIcon, ShowTrayIcon);
                iniFile.WriteBool(Sections.General, Parameters.ShowPopupAlerts, ShowPopupAlerts);
                iniFile.WriteString(Sections.General, Parameters.Language, Language);

                keyWin = ((GlobalSettings.Modifiers & Keys.LWin) != 0);
                keyShift = ((GlobalSettings.Modifiers & Keys.Shift) != 0);
                keyAlt = ((GlobalSettings.Modifiers & Keys.Alt) != 0);
                keyCtrl = ((GlobalSettings.Modifiers & Keys.Control) != 0);

                iniFile.WriteBool(Sections.Shortcut, Parameters.Alt, keyAlt);
                iniFile.WriteBool(Sections.Shortcut, Parameters.Ctrl, keyCtrl);
                iniFile.WriteBool(Sections.Shortcut, Parameters.Win, keyWin);
                iniFile.WriteBool(Sections.Shortcut, Parameters.Shift, keyShift);

                iniFile.WriteString(Sections.Shortcut, Parameters.Key, Shortcut.ToString());

                keyShift = ((GlobalSettings.MouseModifiers & Keys.Shift) != 0);
                keyAlt = ((GlobalSettings.MouseModifiers & Keys.Alt) != 0);
                keyCtrl = ((GlobalSettings.MouseModifiers & Keys.Control) != 0);

                iniFile.WriteBool(Sections.Mouse, Parameters.ActivateCursor, ActivateCursor);
                iniFile.WriteBool(Sections.Mouse, Parameters.UseRightClick, UseRightClick);
                iniFile.WriteBool(Sections.Mouse, Parameters.DesktopClick, DesktopClick);

                iniFile.WriteBool(Sections.Mouse, Parameters.Alt, keyAlt);
                iniFile.WriteBool(Sections.Mouse, Parameters.Ctrl, keyCtrl);
                iniFile.WriteBool(Sections.Mouse, Parameters.Shift, keyShift);

                iniFile.WriteBool(Sections.General, Parameters.CheckUpdate, CheckUpdate);
                iniFile.WriteBool(Sections.General, Parameters.ShowSplashScreen, ShowSplashScreen);
                iniFile.WriteBool(Sections.General, Parameters.StartVisible, StartVisible);
                iniFile.WriteBool(Sections.General, Parameters.UseSound, UseSound);

                iniFile.WriteInteger(Sections.Help, Parameters.PortNumber, PortNumber);
                iniFile.WriteBool(Sections.Help, Parameters.ActivateServer, ActivateServer);

                iniFile.WriteBool(Sections.Manager, Parameters.HideOnClick, HideOnClick);
                iniFile.WriteInteger(Sections.Manager, Parameters.FadeDelay, FadeDelay);
                iniFile.WriteInteger(Sections.Manager, Parameters.WindowHeight, WindowHeight);
                iniFile.WriteInteger(Sections.Manager, Parameters.WindowWidth, WindowWidth);
                iniFile.WriteInteger(Sections.Manager, Parameters.StoneSize, StoneSize);
                iniFile.WriteInteger(Sections.Manager, Parameters.IconSize, IconSize);
                iniFile.WriteInteger(Sections.Manager, Parameters.Transparency, Transparency);
                iniFile.WriteInteger(Sections.Manager, Parameters.Raduis, Radius);
                iniFile.WriteBool(Sections.Manager, Parameters.RotateOnClick, RotateOnClick);
                iniFile.WriteBool(Sections.Manager, Parameters.IgnoreFullScreen, IgnoreFullScreen);
                iniFile.WriteBool(Sections.Manager, Parameters.ManagerButtons, ShowManagerButtons);
                iniFile.WriteInteger(Sections.Manager, Parameters.DefaultStonesNumber, DefaultStonesNumber);
                iniFile.WriteBool(Sections.Manager, Parameters.LiveReflection, LiveReflection);
                iniFile.WriteBool(Sections.Manager, Parameters.ShowStoneHint, ShowStoneHint);
                iniFile.WriteBool(Sections.Manager, Parameters.FlatRing, FlatRing);

                string shortMenuSkin = FileOperations.UnExpandPath(MenuSkin);
                if (!string.IsNullOrEmpty(shortMenuSkin))
                {
                    iniFile.WriteString(Sections.Menu, Parameters.Theme, shortMenuSkin);
                }
                else
                {
                    iniFile.DeleteKey(Sections.Menu, Parameters.Theme);
                }

                DefaultCircle = GlobalConfig.DefaultCircleName;
                string defaultCircleFile = FileOperations.UnExpandPath(DefaultCircle);
                if (!string.IsNullOrEmpty(defaultCircleFile))
                {
                    iniFile.WriteString(Sections.General, Parameters.DefaultCircle, defaultCircleFile);
                }
                else
                {
                    iniFile.DeleteKey(Sections.General, Parameters.DefaultCircle);
                }

                iniFile.Save();
            }
            finally
            {
                iniFile.Dispose();
            }
        }
    }
}

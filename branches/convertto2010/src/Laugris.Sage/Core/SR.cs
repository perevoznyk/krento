namespace Laugris.Sage
{


    public sealed partial class SR
    {

        public static string ActivateOnStart
        {
            get
            {
                return Keys.GetString(Keys.ActivateOnStart, "Show circle when Krento starts");
            }
        }

        public static string ActivateServer
        {
            get
            {
                return Keys.GetString(Keys.ActivateServer, "Activate webserver");
            }
        }

        public static string ActivateUsingKeyboard
        {
            get
            {
                return Keys.GetString(Keys.ActivateUsingKeyboard, "Activate Krento using keyboard");
            }
        }

        public static string ActivateUsingMouse
        {
            get
            {
                return Keys.GetString(Keys.ActivateUsingMouse, "Activate Krento using mouse");
            }
        }

        public static string AddStone
        {
            get
            {
                return Keys.GetString(Keys.AddStone, "Add Stone");
            }
        }

        public static string AdvancedSettings
        {
            get
            {
                return Keys.GetString(Keys.AdvancedSettings, "Advanced settings");
            }
        }

        public static string AppletAccessibilityOptions
        {
            get
            {
                return Keys.GetString(Keys.AppletAccessibilityOptions, "Accessibility Options");
            }
        }

        public static string AppletAddRemoveProgram
        {
            get
            {
                return Keys.GetString(Keys.AppletAddRemoveProgram, "Add/Remove Programs");
            }
        }

        public static string AppletAppearance
        {
            get
            {
                return Keys.GetString(Keys.AppletAppearance, "Appearance");
            }
        }

        public static string AppletDateAndTime
        {
            get
            {
                return Keys.GetString(Keys.AppletDateAndTime, "Date and Time");
            }
        }

        public static string AppletDisplayBackground
        {
            get
            {
                return Keys.GetString(Keys.AppletDisplayBackground, "Display Background");
            }
        }

        public static string AppletFonts
        {
            get
            {
                return Keys.GetString(Keys.AppletFonts, "Fonts");
            }
        }

        public static string AppletKeyboard
        {
            get
            {
                return Keys.GetString(Keys.AppletKeyboard, "Keyboard");
            }
        }

        public static string AppletMouse
        {
            get
            {
                return Keys.GetString(Keys.AppletMouse, "Mouse");
            }
        }

        public static string AppletNetworkConnections
        {
            get
            {
                return Keys.GetString(Keys.AppletNetworkConnections, "Network Connections");
            }
        }

        public static string AppletPrintersAndFaxes
        {
            get
            {
                return Keys.GetString(Keys.AppletPrintersAndFaxes, "Printers and Faxes");
            }
        }

        public static string AppletThemes
        {
            get
            {
                return Keys.GetString(Keys.AppletThemes, "Themes");
            }
        }

        public static string AppletUserAccounts
        {
            get
            {
                return Keys.GetString(Keys.AppletUserAccounts, "User Accounts");
            }
        }

        public static string Applications
        {
            get
            {
                return Keys.GetString(Keys.Applications, "Applications");
            }
        }

        public static string ApplicationsFilter
        {
            get
            {
                return Keys.GetString(Keys.ApplicationsFilter, "Applications|*.exe|All files|*.*");
            }
        }

        public static string AssociateRing
        {
            get
            {
                return Keys.GetString(Keys.AssociateRing, "Associate .circle files with Krento");
            }
        }

        public static string BackupData
        {
            get
            {
                return Keys.GetString(Keys.BackupData, "Backup personal data");
            }
        }

        public static string Blogger
        {
            get
            {
                return Keys.GetString(Keys.Blogger, "Blogger");
            }
        }

        public static string Browse
        {
            get
            {
                return Keys.GetString(Keys.Browse, "Browse...");
            }
        }

        public static string ButtonAlt
        {
            get
            {
                return Keys.GetString(Keys.ButtonAlt, "Alt button");
            }
        }

        public static string ButtonControl
        {
            get
            {
                return Keys.GetString(Keys.ButtonControl, "Control button");
            }
        }

        public static string ButtonShift
        {
            get
            {
                return Keys.GetString(Keys.ButtonShift, "Shift button");
            }
        }

        public static string Calculator
        {
            get
            {
                return Keys.GetString(Keys.Calculator, "Calculator");
            }
        }

        public static string Calendar
        {
            get
            {
                return Keys.GetString(Keys.Calendar, "Calendar");
            }
        }

        public static string Cancel
        {
            get
            {
                return Keys.GetString(Keys.Cancel, "Cancel");
            }
        }

        public static string CheckUpdateNow
        {
            get
            {
                return Keys.GetString(Keys.CheckUpdateNow, "Check now\nThis will connect to the Internet and check for available updates");
            }
        }

        public static string CheckUpdates
        {
            get
            {
                return Keys.GetString(Keys.CheckUpdates, "Automatic check for updates");
            }
        }

        public static string Circle
        {
            get
            {
                return Keys.GetString(Keys.Circle, "Circle");
            }
        }

        public static string CircleDefaultDescription
        {
            get
            {
                return Keys.GetString(Keys.CircleDefaultDescription, "Krento Applications Circle");
            }
        }

        public static string ClearCache
        {
            get
            {
                return Keys.GetString(Keys.ClearCache, "Clear Images Cache");
            }
        }

        public static string Close
        {
            get
            {
                return Keys.GetString(Keys.Close, "Close");
            }
        }

        public static string CommandPrompt
        {
            get
            {
                return Keys.GetString(Keys.CommandPrompt, "Command Prompt");
            }
        }

        public static string ControlPanel
        {
            get
            {
                return Keys.GetString(Keys.ControlPanel, "Control Panel");
            }
        }

        public static string CopyDocument(string fileName)
        {
            return Keys.GetString(Keys.CopyDocument, "File {0} was copied to My Documents folder" , new object[] {
                            fileName});
        }

        public static string CopyMusic(string fileName)
        {
            return Keys.GetString(Keys.CopyMusic, "File {0} was copied to My Music folder" , new object[] {
                            fileName});
        }

        public static string CopyPicture(string fileName)
        {
            return Keys.GetString(Keys.CopyPicture, "File {0} was copied to My Pictures folder" , new object[] {
                            fileName});
        }

        public static string CreateCircle
        {
            get
            {
                return Keys.GetString(Keys.CreateCircle, "Create new circle");
            }
        }

        public static string CreateStoneError
        {
            get
            {
                return Keys.GetString(Keys.CreateStoneError, "Create stone error");
            }
        }

        public static string CurrentDate
        {
            get
            {
                return Keys.GetString(Keys.CurrentDate, "Current date");
            }
        }

        public static string CurrentTime
        {
            get
            {
                return Keys.GetString(Keys.CurrentTime, "Current time");
            }
        }

        public static string Date
        {
            get
            {
                return Keys.GetString(Keys.Date, "Date");
            }
        }

        public static string DefaultRingName
        {
            get
            {
                return Keys.GetString(Keys.DefaultRingName, "Default");
            }
        }

        public static string DefaultSkin
        {
            get
            {
                return Keys.GetString(Keys.DefaultSkin, "Default Skin");
            }
        }

        public static string Delete
        {
            get
            {
                return Keys.GetString(Keys.Delete, "Delete");
            }
        }

        public static string Delicious
        {
            get
            {
                return Keys.GetString(Keys.Delicious, "Delicious");
            }
        }

        public static string Description
        {
            get
            {
                return Keys.GetString(Keys.Description, "Description");
            }
        }

        public static string DesktopClick
        {
            get
            {
                return Keys.GetString(Keys.DesktopClick, "Activate by double click on desktop");
            }
        }

        public static string DesktopRing
        {
            get
            {
                return Keys.GetString(Keys.DesktopRing, "Desktop");
            }
        }

        public static string DeviceManager
        {
            get
            {
                return Keys.GetString(Keys.DeviceManager, "Device Manager");
            }
        }

        public static string Disabled
        {
            get
            {
                return Keys.GetString(Keys.Disabled, "Disabled");
            }
        }

        public static string Documents
        {
            get
            {
                return Keys.GetString(Keys.Documents, "Documents");
            }
        }

        public static string DoNotAsk
        {
            get
            {
                return Keys.GetString(Keys.DoNotAsk, "Do not ask me again");
            }
        }

        public static string DoNotCheckUpdate
        {
            get
            {
                return Keys.GetString(Keys.DoNotCheckUpdate, "Cancel\nDo not check for updates now");
            }
        }

        public static string Error
        {
            get
            {
                return Keys.GetString(Keys.Error, "Error");
            }
        }

        public static string ErrorCreateNewGuid
        {
            get
            {
                return Keys.GetString(Keys.ErrorCreateNewGuid, "Error creating new Guid");
            }
        }

        public static string ErrorDeleteCurrentCircle
        {
            get
            {
                return Keys.GetString(Keys.ErrorDeleteCurrentCircle, "Can not delete the current circle");
            }
        }

        public static string ErrorDeleteDefaultRing
        {
            get
            {
                return Keys.GetString(Keys.ErrorDeleteDefaultRing, "Can not delete default circle");
            }
        }

        public static string ErrorDeleteSingleRing
        {
            get
            {
                return Keys.GetString(Keys.ErrorDeleteSingleRing, "Can not delete the last circle");
            }
        }

        public static string ErrorOverWriteCurrentCircle
        {
            get
            {
                return Keys.GetString(Keys.ErrorOverWriteCurrentCircle, "Can not overwrite the current circle");
            }
        }

        public static string Facebook
        {
            get
            {
                return Keys.GetString(Keys.Facebook, "Facebook");
            }
        }

        public static string FadeDelay
        {
            get
            {
                return Keys.GetString(Keys.FadeDelay, "Fade Delay");
            }
        }

        public static string FileConfigArgument
        {
            get
            {
                return Keys.GetString(Keys.FileConfigArgument, "Argument description");
            }
        }

        public static string FileConfigArgumentHint
        {
            get
            {
                return Keys.GetString(Keys.FileConfigArgumentHint, "If you inserted ## in the target field, you can\nprovide here the text of the prompt that will be\nused during the stone execution");
            }
        }

        public static string FileConfigBrowseHint
        {
            get
            {
                return Keys.GetString(Keys.FileConfigBrowseHint, "Browse your computer for the new target");
            }
        }

        public static string FileConfigCancelHint
        {
            get
            {
                return Keys.GetString(Keys.FileConfigCancelHint, "Discard all your changes");
            }
        }

        public static string FileConfigCommandLine
        {
            get
            {
                return Keys.GetString(Keys.FileConfigCommandLine, "Command line parameters");
            }
        }

        public static string FileConfigCommandLineHint
        {
            get
            {
                return Keys.GetString(Keys.FileConfigCommandLineHint, "Optional command line parameters");
            }
        }

        public static string FileConfigDefault
        {
            get
            {
                return Keys.GetString(Keys.FileConfigDefault, "Default");
            }
        }

        public static string FileConfigDefaultHint
        {
            get
            {
                return Keys.GetString(Keys.FileConfigDefaultHint, "Assign default icon for this stone");
            }
        }

        public static string FileConfigDescription
        {
            get
            {
                return Keys.GetString(Keys.FileConfigDescription, "Description");
            }
        }

        public static string FileConfigDescriptionHint
        {
            get
            {
                return Keys.GetString(Keys.FileConfigDescriptionHint, "Enter useful description of the target");
            }
        }

        public static string FileConfigOKHint
        {
            get
            {
                return Keys.GetString(Keys.FileConfigOKHint, "Save stone configuration");
            }
        }

        public static string FileConfigSelect
        {
            get
            {
                return Keys.GetString(Keys.FileConfigSelect, "Select...");
            }
        }

        public static string FileConfigSelectHint
        {
            get
            {
                return Keys.GetString(Keys.FileConfigSelectHint, "Select new icon for this stone");
            }
        }

        public static string FileConfigTarget
        {
            get
            {
                return Keys.GetString(Keys.FileConfigTarget, "Target");
            }
        }

        public static string FileConfigTargetHint
        {
            get
            {
                return Keys.GetString(Keys.FileConfigTargetHint, "Select target file name or web site address\nYou can use ## as a parameter that will be asked\nduring the stone execution");
            }
        }

        public static string FileConfigTitle
        {
            get
            {
                return Keys.GetString(Keys.FileConfigTitle, "Configuration");
            }
        }

        public static string FixedPosition
        {
            get
            {
                return Keys.GetString(Keys.FixedPosition, "Fixed position");
            }
        }

        public static string Flickr
        {
            get
            {
                return Keys.GetString(Keys.Flickr, "Flickr");
            }
        }

        public static string General
        {
            get
            {
                return Keys.GetString(Keys.General, "General");
            }
        }

        public static string GMail
        {
            get
            {
                return Keys.GetString(Keys.GMail, "GMail");
            }
        }

        public static string Google
        {
            get
            {
                return Keys.GetString(Keys.Google, "Google");
            }
        }

        public static string HideOnClick
        {
            get
            {
                return Keys.GetString(Keys.HideOnClick, "Hide on mouse click");
            }
        }

        public static string HideToy
        {
            get
            {
                return Keys.GetString(Keys.HideToy, "Hide Toy");
            }
        }

        public static string HotkeyCollision(string keyMessage)
        {
            return Keys.GetString(Keys.HotkeyCollision, "Well done!  You can successfully use Krento...but there is a small problem.\n\nKrento uses the hotkey {0} to give you quick keyboard access to the launch window.  Another application has already registered the {0} hotkey.  You can press {0} to see what application has registered the hotkey.\n\nTo fix this, reconfigure the other application to use a different hotkey or close the other application and restart Krento.  In the mean time, you can open the launch window by double clicking the desktop pulsar." , new object[] {
                            keyMessage});
        }

        public static string HotKeyMustIncludeModifier
        {
            get
            {
                return Keys.GetString(Keys.HotKeyMustIncludeModifier, "Hotkey must include a modifier key.");
            }
        }

        public static string IconSize
        {
            get
            {
                return Keys.GetString(Keys.IconSize, "Icons size");
            }
        }

        public static string ImageFilesFilter
        {
            get
            {
                return Keys.GetString(Keys.ImageFilesFilter, "Image Files (*.PNG;*.BMP;*.JPG;*.GIF)|*.PNG;*.BMP;*.JPG;*.GIF");
            }
        }

        public static string IndexPage
        {
            get
            {
                return Keys.GetString(Keys.IndexPage, "Index Page");
            }
        }

        public static string InstallKrentoSkin
        {
            get
            {
                return Keys.GetString(Keys.InstallKrentoSkin, "Install Krento Skin");
            }
        }

        public static string InstallMenuSkin
        {
            get
            {
                return Keys.GetString(Keys.InstallMenuSkin, "Install Menu Skin");
            }
        }

        public static string InstallStone
        {
            get
            {
                return Keys.GetString(Keys.InstallStone, "Install new stone");
            }
        }

        public static string InstallToy
        {
            get
            {
                return Keys.GetString(Keys.InstallToy, "Install new toy");
            }
        }

        public static string InternetExplorer
        {
            get
            {
                return Keys.GetString(Keys.InternetExplorer, "Internet Explorer");
            }
        }

        public static string InvalidCircleName
        {
            get
            {
                return Keys.GetString(Keys.InvalidCircleName, "The name of the new circle is not valid");
            }
        }

        public static string KeyboardActivation
        {
            get
            {
                return Keys.GetString(Keys.KeyboardActivation, "Keyboard Activation");
            }
        }

        public static string KeyRegistrationFailed
        {
            get
            {
                return Keys.GetString(Keys.KeyRegistrationFailed, "Key registration failed");
            }
        }

        public static string KrentoForum
        {
            get
            {
                return Keys.GetString(Keys.KrentoForum, "Krento Forum");
            }
        }

        public static string KrentoHookMissing
        {
            get
            {
                return Keys.GetString(Keys.KrentoHookMissing, "MoonRoad.dll file is missing or corrupted");
            }
        }

        public static string KrentoIsRunning
        {
            get
            {
                return Keys.GetString(Keys.KrentoIsRunning, "Krento is already running");
            }
        }

        public static string KrentoMenuSkinFilter
        {
            get
            {
                return Keys.GetString(Keys.KrentoMenuSkinFilter, "Krento Menu Skins (*.kmenu)|*.kmenu");
            }
        }

        public static string KrentoNews
        {
            get
            {
                return Keys.GetString(Keys.KrentoNews, "Krento News");
            }
        }

        public static string KrentoNewSkin
        {
            get
            {
                return Keys.GetString(Keys.KrentoNewSkin, "Change current skin");
            }
        }

        public static string KrentoNotification
        {
            get
            {
                return Keys.GetString(Keys.KrentoNotification, "Krento notification");
            }
        }

        public static string KrentoShortName
        {
            get
            {
                return Keys.GetString(Keys.KrentoShortName, "Krento");
            }
        }

        public static string KrentoSkinFilter
        {
            get
            {
                return Keys.GetString(Keys.KrentoSkinFilter, "Krento Skins (*.kskin)|*.kskin");
            }
        }

        public static string KrentoStartup
        {
            get
            {
                return Keys.GetString(Keys.KrentoStartup, "Krento startup");
            }
        }

        public static string KrentoTaskCircle
        {
            get
            {
                return Keys.GetString(Keys.KrentoTaskCircle, "Krento task circle (*.circle)|*.circle");
            }
        }

        public static string KrentoWelcome
        {
            get
            {
                return Keys.GetString(Keys.KrentoWelcome, "Welcome to Krento");
            }
        }

        public static string Language
        {
            get
            {
                return Keys.GetString(Keys.Language, "Language");
            }
        }

        public static string LiveReflection
        {
            get
            {
                return Keys.GetString(Keys.LiveReflection, "Live reflection of icons");
            }
        }

        public static string Load
        {
            get
            {
                return Keys.GetString(Keys.Load, "Load");
            }
        }

        public static string Logo
        {
            get
            {
                return Keys.GetString(Keys.Logo, "Logo");
            }
        }

        public static string Maintainance
        {
            get
            {
                return Keys.GetString(Keys.Maintainance, "Maintainance");
            }
        }

        public static string ManagerDefaultBackground
        {
            get
            {
                return Keys.GetString(Keys.ManagerDefaultBackground, "<Default background>");
            }
        }

        public static string ManagerHide
        {
            get
            {
                return Keys.GetString(Keys.ManagerHide, "Hide Krento Stones");
            }
        }

        public static string ManagerShow
        {
            get
            {
                return Keys.GetString(Keys.ManagerShow, "Show Krento Stones");
            }
        }

        public static string ManagerWindowText
        {
            get
            {
                return Keys.GetString(Keys.ManagerWindowText, "Krento Manager");
            }
        }

        public static string MenuSkins
        {
            get
            {
                return Keys.GetString(Keys.MenuSkins, "Menu Skins");
            }
        }

        public static string MissingFile
        {
            get
            {
                return Keys.GetString(Keys.MissingFile, "No program selected");
            }
        }

        public static string Modify
        {
            get
            {
                return Keys.GetString(Keys.Modify, "Modify");
            }
        }

        public static string MouseActionOneButton
        {
            get
            {
                return Keys.GetString(Keys.MouseActionOneButton, "Mouse Action 1 Button");
            }
        }

        public static string MouseActionTwoButton
        {
            get
            {
                return Keys.GetString(Keys.MouseActionTwoButton, "Mouse Action 2 Button");
            }
        }

        public static string MouseActivation
        {
            get
            {
                return Keys.GetString(Keys.MouseActivation, "Mouse Activation");
            }
        }

        public static string MouseCursorPosition
        {
            get
            {
                return Keys.GetString(Keys.MouseCursorPosition, "Mouse cursor position");
            }
        }

        public static string MouseWheelButton
        {
            get
            {
                return Keys.GetString(Keys.MouseWheelButton, "Mouse Wheel");
            }
        }

        public static string MouseWheelClick
        {
            get
            {
                return Keys.GetString(Keys.MouseWheelClick, "Mouse Wheel Click");
            }
        }

        public static string MouseXButton1
        {
            get
            {
                return Keys.GetString(Keys.MouseXButton1, "Action 1 Mouse Button");
            }
        }

        public static string MouseXButton2
        {
            get
            {
                return Keys.GetString(Keys.MouseXButton2, "Action 2 Mouse Button");
            }
        }

        public static string Music
        {
            get
            {
                return Keys.GetString(Keys.Music, "Music");
            }
        }

        public static string Name
        {
            get
            {
                return Keys.GetString(Keys.Name, "Name");
            }
        }

        public static string NewCircle
        {
            get
            {
                return Keys.GetString(Keys.NewCircle, "Load / Edit circle");
            }
        }

        public static string NewVersionAvailable(string versionNumber, string downloadSite)
        {
            return Keys.GetString(Keys.NewVersionAvailable, "New version of Krento {0} is available for download from {1}" , new object[] {
                            versionNumber,
                            downloadSite});
        }

        public static string NextCircle
        {
            get
            {
                return Keys.GetString(Keys.NextCircle, "Next circle");
            }
        }

        public static string Notepad
        {
            get
            {
                return Keys.GetString(Keys.Notepad, "Notepad");
            }
        }

        public static string OK
        {
            get
            {
                return Keys.GetString(Keys.OK, "OK");
            }
        }

        public static string OpenCacheFolder
        {
            get
            {
                return Keys.GetString(Keys.OpenCacheFolder, "Open Cache Folder");
            }
        }

        public static string OpenDataFolder
        {
            get
            {
                return Keys.GetString(Keys.OpenDataFolder, "Open Data Folder");
            }
        }

        public static string OpenSkinsFolder
        {
            get
            {
                return Keys.GetString(Keys.OpenSkinsFolder, "Open Skins Folder");
            }
        }

        public static string OpenStonesFolder
        {
            get
            {
                return Keys.GetString(Keys.OpenStonesFolder, "Open Stones Folder");
            }
        }

        public static string OrClick
        {
            get
            {
                return Keys.GetString(Keys.OrClick, "or click");
            }
        }

        public static string Paint
        {
            get
            {
                return Keys.GetString(Keys.Paint, "Paint");
            }
        }

        public static string Parameter
        {
            get
            {
                return Keys.GetString(Keys.Parameter, "Parameter");
            }
        }

        public static string Personalization
        {
            get
            {
                return Keys.GetString(Keys.Personalization, "Personalization");
            }
        }

        public static string Pictures
        {
            get
            {
                return Keys.GetString(Keys.Pictures, "Pictures");
            }
        }

        public static string PortNumber
        {
            get
            {
                return Keys.GetString(Keys.PortNumber, "Port Number");
            }
        }

        public static string PowerManagement
        {
            get
            {
                return Keys.GetString(Keys.PowerManagement, "Power Management");
            }
        }

        public static string PrevCircle
        {
            get
            {
                return Keys.GetString(Keys.PrevCircle, "Previous circle");
            }
        }

        public static string Programs
        {
            get
            {
                return Keys.GetString(Keys.Programs, "Programs");
            }
        }

        public static string PulsarAbout
        {
            get
            {
                return Keys.GetString(Keys.PulsarAbout, "About...");
            }
        }

        public static string PulsarClose
        {
            get
            {
                return Keys.GetString(Keys.PulsarClose, "Close Krento");
            }
        }

        public static string PulsarHelp
        {
            get
            {
                return Keys.GetString(Keys.PulsarHelp, "Help");
            }
        }

        public static string PulsarHide
        {
            get
            {
                return Keys.GetString(Keys.PulsarHide, "Hide Pulsar");
            }
        }

        public static string PulsarOptions
        {
            get
            {
                return Keys.GetString(Keys.PulsarOptions, "Options");
            }
        }

        public static string PulsarShow
        {
            get
            {
                return Keys.GetString(Keys.PulsarShow, "Show Pulsar");
            }
        }

        public static string Radius
        {
            get
            {
                return Keys.GetString(Keys.Radius, "Radius");
            }
        }

        public static string Reddit
        {
            get
            {
                return Keys.GetString(Keys.Reddit, "Reddit");
            }
        }

        public static string RemoveStone
        {
            get
            {
                return Keys.GetString(Keys.RemoveStone, "Remove Stone");
            }
        }

        public static string ResetToDefault
        {
            get
            {
                return Keys.GetString(Keys.ResetToDefault, "Reset to default");
            }
        }

        public static string Restart
        {
            get
            {
                return Keys.GetString(Keys.Restart, "Restart");
            }
        }

        public static string RightButtonActivation
        {
            get
            {
                return Keys.GetString(Keys.RightButtonActivation, "Activate by click and hold the right mouse button");
            }
        }

        public static string RingLocation
        {
            get
            {
                return Keys.GetString(Keys.RingLocation, "Circle Location");
            }
        }

        public static string RotateOnClick
        {
            get
            {
                return Keys.GetString(Keys.RotateOnClick, "Rotate on click");
            }
        }

        public static string RotationSpeed
        {
            get
            {
                return Keys.GetString(Keys.RotationSpeed, "Rotation Speed");
            }
        }

        public static string RunWithWindows
        {
            get
            {
                return Keys.GetString(Keys.RunWithWindows, "Launch Krento when Windows starts");
            }
        }

        public static string ScreenCenter
        {
            get
            {
                return Keys.GetString(Keys.ScreenCenter, "Screen center");
            }
        }

        public static string SearchPhrase
        {
            get
            {
                return Keys.GetString(Keys.SearchPhrase, "Enter search phrase");
            }
        }

        public static string SearchText
        {
            get
            {
                return Keys.GetString(Keys.SearchText, "Search text");
            }
        }

        public static string SelectCircle
        {
            get
            {
                return Keys.GetString(Keys.SelectCircle, "Select Circle");
            }
        }

        public static string SelectStoneType
        {
            get
            {
                return Keys.GetString(Keys.SelectStoneType, "Select stone type");
            }
        }

        public static string ServerRoot
        {
            get
            {
                return Keys.GetString(Keys.ServerRoot, "Root Folder");
            }
        }

        public static string Settings
        {
            get
            {
                return Keys.GetString(Keys.Settings, "Settings");
            }
        }

        public static string ShowManagerButtons
        {
            get
            {
                return Keys.GetString(Keys.ShowManagerButtons, "Show manager buttons");
            }
        }

        public static string ShowPopupAlerts
        {
            get
            {
                return Keys.GetString(Keys.ShowPopupAlerts, "Show Popup Alerts");
            }
        }

        public static string ShowSelector
        {
            get
            {
                return Keys.GetString(Keys.ShowSelector, "Use Windows + N hotkey for circle selection");
            }
        }

        public static string ShowSplashScreen
        {
            get
            {
                return Keys.GetString(Keys.ShowSplashScreen, "Show Splash Screen");
            }
        }

        public static string ShowStonesHint
        {
            get
            {
                return Keys.GetString(Keys.ShowStonesHint, "Show stones hint window");
            }
        }

        public static string ShowTrayIcon
        {
            get
            {
                return Keys.GetString(Keys.ShowTrayIcon, "Show Tray Icon");
            }
        }

        public static string SocialNetworks
        {
            get
            {
                return Keys.GetString(Keys.SocialNetworks, "Social Networks");
            }
        }

        public static string SplashScreen
        {
            get
            {
                return Keys.GetString(Keys.SplashScreen, "Splash Screen");
            }
        }

        public static string StoneAbout
        {
            get
            {
                return Keys.GetString(Keys.StoneAbout, "About");
            }
        }

        public static string StoneChangeType
        {
            get
            {
                return Keys.GetString(Keys.StoneChangeType, "Change stone type");
            }
        }

        public static string StoneConfigure
        {
            get
            {
                return Keys.GetString(Keys.StoneConfigure, "Configure this stone");
            }
        }

        public static string StoneDesktop
        {
            get
            {
                return Keys.GetString(Keys.StoneDesktop, "Show Desktop");
            }
        }

        public static string StoneFile
        {
            get
            {
                return Keys.GetString(Keys.StoneFile, "File or web site launcher");
            }
        }

        public static string StoneHibernate
        {
            get
            {
                return Keys.GetString(Keys.StoneHibernate, "Hibernate Computer");
            }
        }

        public static string StoneMyComputer
        {
            get
            {
                return Keys.GetString(Keys.StoneMyComputer, "My Computer");
            }
        }

        public static string StoneMyDocuments
        {
            get
            {
                return Keys.GetString(Keys.StoneMyDocuments, "My Documents");
            }
        }

        public static string StoneMyIP
        {
            get
            {
                return Keys.GetString(Keys.StoneMyIP, "My current IP address");
            }
        }

        public static string StoneMyMusic
        {
            get
            {
                return Keys.GetString(Keys.StoneMyMusic, "My Music");
            }
        }

        public static string StoneMyPictures
        {
            get
            {
                return Keys.GetString(Keys.StoneMyPictures, "My Pictures");
            }
        }

        public static string StoneRecycleBin
        {
            get
            {
                return Keys.GetString(Keys.StoneRecycleBin, "Recycle Bin");
            }
        }

        public static string StoneRestart
        {
            get
            {
                return Keys.GetString(Keys.StoneRestart, "Restart Computer");
            }
        }

        public static string StoneRing
        {
            get
            {
                return Keys.GetString(Keys.StoneRing, "Krento Applications Circle");
            }
        }

        public static string StonesFilter
        {
            get
            {
                return Keys.GetString(Keys.StonesFilter, "Krento Stones (*.stone)|*.stone");
            }
        }

        public static string StoneShutdown
        {
            get
            {
                return Keys.GetString(Keys.StoneShutdown, "Shutdown Computer");
            }
        }

        public static string StoneSize
        {
            get
            {
                return Keys.GetString(Keys.StoneSize, "Stone Size");
            }
        }

        public static string StonesNumber
        {
            get
            {
                return Keys.GetString(Keys.StonesNumber, "Default number of empty stones");
            }
        }

        public static string StoneSuspend
        {
            get
            {
                return Keys.GetString(Keys.StoneSuspend, "Stand By Computer");
            }
        }

        public static string TargetCircle
        {
            get
            {
                return Keys.GetString(Keys.TargetCircle, "Circle Name");
            }
        }

        public static string TaskManager
        {
            get
            {
                return Keys.GetString(Keys.TaskManager, "Task Manager");
            }
        }

        public static string Time
        {
            get
            {
                return Keys.GetString(Keys.Time, "Time");
            }
        }

        public static string Toys
        {
            get
            {
                return Keys.GetString(Keys.Toys, "Toys");
            }
        }

        public static string ToysFilter
        {
            get
            {
                return Keys.GetString(Keys.ToysFilter, "Toys Files (*.toy;*.docklet)|*.toy;*.docklet");
            }
        }

        public static string Trancparency
        {
            get
            {
                return Keys.GetString(Keys.Trancparency, "Transparency");
            }
        }

        public static string TrayIcon
        {
            get
            {
                return Keys.GetString(Keys.TrayIcon, "Tray Icon");
            }
        }

        public static string Tuning
        {
            get
            {
                return Keys.GetString(Keys.Tuning, "Tuning");
            }
        }

        public static string Twitter
        {
            get
            {
                return Keys.GetString(Keys.Twitter, "Twitter");
            }
        }

        public static string UnsupportedProcessWindowStyle
        {
            get
            {
                return Keys.GetString(Keys.UnsupportedProcessWindowStyle, "Unsupported Process Window Style value.");
            }
        }

        public static string UseCtrlTab
        {
            get
            {
                return Keys.GetString(Keys.UseCtrlTab, "Hint: Use Ctrl +Tab to switch between settings screens");
            }
        }

        public static string UseDefaultKey
        {
            get
            {
                return Keys.GetString(Keys.UseDefaultKey, "Use default (Win + S)");
            }
        }

        public static string UserFolders
        {
            get
            {
                return Keys.GetString(Keys.UserFolders, "User Folders");
            }
        }

        public static string UseSounds
        {
            get
            {
                return Keys.GetString(Keys.UseSounds, "Use sounds");
            }
        }

        public static string Video
        {
            get
            {
                return Keys.GetString(Keys.Video, "Video");
            }
        }

        public static string WarningRingExists
        {
            get
            {
                return Keys.GetString(Keys.WarningRingExists, "The circle with this name already exists.\nDo you want to overwrite it?");
            }
        }

        public static string WelcomeMessage(string keyMessage, string clickMessage, string mouseButton)
        {
            return Keys.GetString(Keys.WelcomeMessage, "Welcome to Krento!\nPress {0} keys {1} {2} \nto display Krento Stones Manager" , new object[] {
                            keyMessage,
                            clickMessage,
                            mouseButton});
        }

        public static string Wikipedia
        {
            get
            {
                return Keys.GetString(Keys.Wikipedia, "Wikipedia");
            }
        }

        public static string WindowHeight
        {
            get
            {
                return Keys.GetString(Keys.WindowHeight, "Window Height");
            }
        }

        public static string WindowText
        {
            get
            {
                return Keys.GetString(Keys.WindowText, "Krento Stone");
            }
        }

        public static string WindowWidth
        {
            get
            {
                return Keys.GetString(Keys.WindowWidth, "Window Width");
            }
        }

        public static string Wordpress
        {
            get
            {
                return Keys.GetString(Keys.Wordpress, "Wordpress");
            }
        }

        public static string WriteSettingsError
        {
            get
            {
                return Keys.GetString(Keys.WriteSettingsError, "Error writting configuration file to disk");
            }
        }

        public static string Yahoo
        {
            get
            {
                return Keys.GetString(Keys.Yahoo, "Yahoo");
            }
        }

        public static string YouTube
        {
            get
            {
                return Keys.GetString(Keys.YouTube, "YouTube");
            }
        }

        public sealed class Keys
        {

            public const string ActivateOnStart="ActivateOnStart";

            public const string ActivateServer="ActivateServer";

            public const string ActivateUsingKeyboard="ActivateUsingKeyboard";

            public const string ActivateUsingMouse="ActivateUsingMouse";

            public const string AddStone="AddStone";

            public const string AdvancedSettings="AdvancedSettings";

            public const string AppletAccessibilityOptions="AppletAccessibilityOptions";

            public const string AppletAddRemoveProgram="AppletAddRemoveProgram";

            public const string AppletAppearance="AppletAppearance";

            public const string AppletDateAndTime="AppletDateAndTime";

            public const string AppletDisplayBackground="AppletDisplayBackground";

            public const string AppletFonts="AppletFonts";

            public const string AppletKeyboard="AppletKeyboard";

            public const string AppletMouse="AppletMouse";

            public const string AppletNetworkConnections="AppletNetworkConnections";

            public const string AppletPrintersAndFaxes="AppletPrintersAndFaxes";

            public const string AppletThemes="AppletThemes";

            public const string AppletUserAccounts="AppletUserAccounts";

            public const string Applications="Applications";

            public const string ApplicationsFilter="ApplicationsFilter";

            public const string AssociateRing="AssociateRing";

            public const string BackupData="BackupData";

            public const string Blogger="Blogger";

            public const string Browse="Browse";

            public const string ButtonAlt="ButtonAlt";

            public const string ButtonControl="ButtonControl";

            public const string ButtonShift="ButtonShift";

            public const string Calculator="Calculator";

            public const string Calendar="Calendar";

            public const string Cancel="Cancel";

            public const string CheckUpdateNow="CheckUpdateNow";

            public const string CheckUpdates="CheckUpdates";

            public const string Circle="Circle";

            public const string CircleDefaultDescription="CircleDefaultDescription";

            public const string ClearCache="ClearCache";

            public const string Close="Close";

            public const string CommandPrompt="CommandPrompt";

            public const string ControlPanel="ControlPanel";

            public const string CopyDocument="CopyDocument";

            public const string CopyMusic="CopyMusic";

            public const string CopyPicture="CopyPicture";

            public const string CreateCircle="CreateCircle";

            public const string CreateStoneError="CreateStoneError";

            public const string CurrentDate="CurrentDate";

            public const string CurrentTime="CurrentTime";

            public const string Date="Date";

            public const string DefaultRingName="DefaultRingName";

            public const string DefaultSkin="DefaultSkin";

            public const string Delete="Delete";

            public const string Delicious="Delicious";

            public const string Description="Description";

            public const string DesktopClick="DesktopClick";

            public const string DesktopRing="DesktopRing";

            public const string DeviceManager="DeviceManager";

            public const string Disabled="Disabled";

            public const string Documents="Documents";

            public const string DoNotAsk="DoNotAsk";

            public const string DoNotCheckUpdate="DoNotCheckUpdate";

            public const string Error="Error";

            public const string ErrorCreateNewGuid="ErrorCreateNewGuid";

            public const string ErrorDeleteCurrentCircle="ErrorDeleteCurrentCircle";

            public const string ErrorDeleteDefaultRing="ErrorDeleteDefaultRing";

            public const string ErrorDeleteSingleRing="ErrorDeleteSingleRing";

            public const string ErrorOverWriteCurrentCircle="ErrorOverWriteCurrentCircle";

            public const string Facebook="Facebook";

            public const string FadeDelay="FadeDelay";

            public const string FileConfigArgument="FileConfigArgument";

            public const string FileConfigArgumentHint="FileConfigArgumentHint";

            public const string FileConfigBrowseHint="FileConfigBrowseHint";

            public const string FileConfigCancelHint="FileConfigCancelHint";

            public const string FileConfigCommandLine="FileConfigCommandLine";

            public const string FileConfigCommandLineHint="FileConfigCommandLineHint";

            public const string FileConfigDefault="FileConfigDefault";

            public const string FileConfigDefaultHint="FileConfigDefaultHint";

            public const string FileConfigDescription="FileConfigDescription";

            public const string FileConfigDescriptionHint="FileConfigDescriptionHint";

            public const string FileConfigOKHint="FileConfigOKHint";

            public const string FileConfigSelect="FileConfigSelect";

            public const string FileConfigSelectHint="FileConfigSelectHint";

            public const string FileConfigTarget="FileConfigTarget";

            public const string FileConfigTargetHint="FileConfigTargetHint";

            public const string FileConfigTitle="FileConfigTitle";

            public const string FixedPosition="FixedPosition";

            public const string Flickr="Flickr";

            public const string General="General";

            public const string GMail="GMail";

            public const string Google="Google";

            public const string HideOnClick="HideOnClick";

            public const string HideToy="HideToy";

            public const string HotkeyCollision="HotkeyCollision";

            public const string HotKeyMustIncludeModifier="HotKeyMustIncludeModifier";

            public const string IconSize="IconSize";

            public const string ImageFilesFilter="ImageFilesFilter";

            public const string IndexPage="IndexPage";

            public const string InstallKrentoSkin="InstallKrentoSkin";

            public const string InstallMenuSkin="InstallMenuSkin";

            public const string InstallStone="InstallStone";

            public const string InstallToy="InstallToy";

            public const string InternetExplorer="InternetExplorer";

            public const string InvalidCircleName="InvalidCircleName";

            public const string KeyboardActivation="KeyboardActivation";

            public const string KeyRegistrationFailed="KeyRegistrationFailed";

            public const string KrentoForum="KrentoForum";

            public const string KrentoHookMissing="KrentoHookMissing";

            public const string KrentoIsRunning="KrentoIsRunning";

            public const string KrentoMenuSkinFilter="KrentoMenuSkinFilter";

            public const string KrentoNews="KrentoNews";

            public const string KrentoNewSkin="KrentoNewSkin";

            public const string KrentoNotification="KrentoNotification";

            public const string KrentoShortName="KrentoShortName";

            public const string KrentoSkinFilter="KrentoSkinFilter";

            public const string KrentoStartup="KrentoStartup";

            public const string KrentoTaskCircle="KrentoTaskCircle";

            public const string KrentoWelcome="KrentoWelcome";

            public const string Language="Language";

            public const string LiveReflection="LiveReflection";

            public const string Load="Load";

            public const string Logo="Logo";

            public const string Maintainance="Maintainance";

            public const string ManagerDefaultBackground="ManagerDefaultBackground";

            public const string ManagerHide="ManagerHide";

            public const string ManagerShow="ManagerShow";

            public const string ManagerWindowText="ManagerWindowText";

            public const string MenuSkins="MenuSkins";

            public const string MissingFile="MissingFile";

            public const string Modify="Modify";

            public const string MouseActionOneButton="MouseActionOneButton";

            public const string MouseActionTwoButton="MouseActionTwoButton";

            public const string MouseActivation="MouseActivation";

            public const string MouseCursorPosition="MouseCursorPosition";

            public const string MouseWheelButton="MouseWheelButton";

            public const string MouseWheelClick="MouseWheelClick";

            public const string MouseXButton1="MouseXButton1";

            public const string MouseXButton2="MouseXButton2";

            public const string Music="Music";

            public const string Name="Name";

            public const string NewCircle="NewCircle";

            public const string NewVersionAvailable="NewVersionAvailable";

            public const string NextCircle="NextCircle";

            public const string Notepad="Notepad";

            public const string OK="OK";

            public const string OpenCacheFolder="OpenCacheFolder";

            public const string OpenDataFolder="OpenDataFolder";

            public const string OpenSkinsFolder="OpenSkinsFolder";

            public const string OpenStonesFolder="OpenStonesFolder";

            public const string OrClick="OrClick";

            public const string Paint="Paint";

            public const string Parameter="Parameter";

            public const string Personalization="Personalization";

            public const string Pictures="Pictures";

            public const string PortNumber="PortNumber";

            public const string PowerManagement="PowerManagement";

            public const string PrevCircle="PrevCircle";

            public const string Programs="Programs";

            public const string PulsarAbout="PulsarAbout";

            public const string PulsarClose="PulsarClose";

            public const string PulsarHelp="PulsarHelp";

            public const string PulsarHide="PulsarHide";

            public const string PulsarOptions="PulsarOptions";

            public const string PulsarShow="PulsarShow";

            public const string Radius="Radius";

            public const string Reddit="Reddit";

            public const string RemoveStone="RemoveStone";

            public const string ResetToDefault="ResetToDefault";

            public const string Restart="Restart";

            public const string RightButtonActivation="RightButtonActivation";

            public const string RingLocation="RingLocation";

            public const string RotateOnClick="RotateOnClick";

            public const string RotationSpeed="RotationSpeed";

            public const string RunWithWindows="RunWithWindows";

            public const string ScreenCenter="ScreenCenter";

            public const string SearchPhrase="SearchPhrase";

            public const string SearchText="SearchText";

            public const string SelectCircle="SelectCircle";

            public const string SelectStoneType="SelectStoneType";

            public const string ServerRoot="ServerRoot";

            public const string Settings="Settings";

            public const string ShowManagerButtons="ShowManagerButtons";

            public const string ShowPopupAlerts="ShowPopupAlerts";

            public const string ShowSelector="ShowSelector";

            public const string ShowSplashScreen="ShowSplashScreen";

            public const string ShowStonesHint="ShowStonesHint";

            public const string ShowTrayIcon="ShowTrayIcon";

            public const string SocialNetworks="SocialNetworks";

            public const string SplashScreen="SplashScreen";

            public const string StoneAbout="StoneAbout";

            public const string StoneChangeType="StoneChangeType";

            public const string StoneConfigure="StoneConfigure";

            public const string StoneDesktop="StoneDesktop";

            public const string StoneFile="StoneFile";

            public const string StoneHibernate="StoneHibernate";

            public const string StoneMyComputer="StoneMyComputer";

            public const string StoneMyDocuments="StoneMyDocuments";

            public const string StoneMyIP="StoneMyIP";

            public const string StoneMyMusic="StoneMyMusic";

            public const string StoneMyPictures="StoneMyPictures";

            public const string StoneRecycleBin="StoneRecycleBin";

            public const string StoneRestart="StoneRestart";

            public const string StoneRing="StoneRing";

            public const string StonesFilter="StonesFilter";

            public const string StoneShutdown="StoneShutdown";

            public const string StoneSize="StoneSize";

            public const string StonesNumber="StonesNumber";

            public const string StoneSuspend="StoneSuspend";

            public const string TargetCircle="TargetCircle";

            public const string TaskManager="TaskManager";

            public const string Time="Time";

            public const string Toys="Toys";

            public const string ToysFilter="ToysFilter";

            public const string Trancparency="Trancparency";

            public const string TrayIcon="TrayIcon";

            public const string Tuning="Tuning";

            public const string Twitter="Twitter";

            public const string UnsupportedProcessWindowStyle="UnsupportedProcessWindowStyle";

            public const string UseCtrlTab="UseCtrlTab";

            public const string UseDefaultKey="UseDefaultKey";

            public const string UserFolders="UserFolders";

            public const string UseSounds="UseSounds";

            public const string Video="Video";

            public const string WarningRingExists="WarningRingExists";

            public const string WelcomeMessage="WelcomeMessage";

            public const string Wikipedia="Wikipedia";

            public const string WindowHeight="WindowHeight";

            public const string WindowText="WindowText";

            public const string WindowWidth="WindowWidth";

            public const string Wordpress="Wordpress";

            public const string WriteSettingsError="WriteSettingsError";

            public const string Yahoo="Yahoo";

            public const string YouTube="YouTube";

            public static string GetString(string key)
            {
                return Laugris.Sage.Language.GetString(key);
            }


            public static string GetString(string key, string defaultValue)
            {
                return Laugris.Sage.Language.GetString(key, defaultValue);
            }

            
            public static string GetString(string key, string defaultValue, object[] args)
            {
                string msg;

                msg = Laugris.Sage.Language.GetString(key, defaultValue);
                msg = string.Format(Laugris.Sage.Language.Culture, msg, args);
                return msg;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace Laugris.Sage
{
    public static class GlobalConfig
    {
        private static string productName = Application.ProductName;
        private static string defaultCircleName;
        private static string indexPage = "index.html";
        private static string mainFolder;
        private static string applicationFolder;
        private static string applicationDrive;
        private static string serverRoot;
        private static string rollingStonesFolder;
        private static string stoneClasses;
        private static string dockletsFolder;
        private static string iconsFolder;
        private static string homeCircleName;
        private static string krentoSettingsFileName;
        private static string krentoConfigFileName;
        private static string keyMappingFileName;
        private static string siteImagesFileName;
        private static string appImagesFileName;
        private static string siteIconsFolder;
        private static string ringsIconsFolder;
        private static string appIconsFolder;
        private static string extIconsFolder;
        private static string languagesFolder;
        private static string menusFolder;
        private static string toysFolder;
        private static string downloadsFolder;
        private static string cacheFolder;
        private static string appShortcuts;
        private static string skinsFolder;
        private static string extensionsFolder;
        private static string backupFolder;

        static GlobalConfig()
        {
            AssignValues();
        }

        public static void AssignValues()
        {
            string assemblyLocation = Assembly.GetEntryAssembly().Location;

#if PORTABLE
                mainFolder = Path.GetDirectoryName(assemblyLocation);
#else
            mainFolder = UserSpecificDataFolder;
#endif
            applicationFolder = Path.GetDirectoryName(assemblyLocation);
            applicationDrive = FileOperations.ExcludeTrailingPathDelimiter(Path.GetPathRoot(MainFolder));
            serverRoot = FoldersInfo.ConcatenatePath(MainFolder, @"WebContent");
            rollingStonesFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Stones");
            stoneClasses = FoldersInfo.ConcatenatePath(MainFolder, @"Classes");
            iconsFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Icons");
            dockletsFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Docklets");
            homeCircleName = Path.Combine(RollingStonesFolder, @"Default.circle");
            krentoSettingsFileName = Path.Combine(MainFolder, "Krento.ini");
            krentoConfigFileName = Path.Combine(MainFolder, "Krento.config");
            keyMappingFileName = Path.Combine(MainFolder, "KeyMapping.ini");
            siteImagesFileName = Path.Combine(MainFolder, "WebImages.ini");
            appImagesFileName = Path.Combine(MainFolder, "AppImages.ini");
            siteIconsFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Icons\Sites");
            appIconsFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Icons\Applications");
            extIconsFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Icons\Extensions");
            languagesFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Languages");
            menusFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Menus");
            toysFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Toys");
            downloadsFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Downloads");
            cacheFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Cache");
            appShortcuts = FoldersInfo.ConcatenatePath(MainFolder, @"Shortcuts");
            skinsFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Skins");
            extensionsFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Extensions");
            ringsIconsFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Icons\Rings");

#if PORTABLE
            backupFolder = FoldersInfo.ConcatenatePath(MainFolder, @"Backup");
#else
            backupFolder = GlobalConfig.UserSpecificDocuments;
#endif
        }

        /// <summary>
        /// Global InterpolationMode used by all drawing methods of Krento
        /// </summary>
        private static InterpolationMode interpolationMode = InterpolationMode.HighQualityBicubic;

        /// <summary>
        /// Application is running in low memory and resources mode
        /// </summary>
        public static bool LowMemory { get; set; }

        /// <summary>
        /// Gets the global drawing interpolation mode.
        /// </summary>
        /// <value>The interpolation mode.</value>
        public static InterpolationMode InterpolationMode
        {
            get { return interpolationMode; }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public static string ProductName
        {
            get { return productName; }

        }

        /// <summary>
        /// Gets the backup folder.
        /// </summary>
        public static string BackupFolder
        {
            get { return backupFolder; }
        }

        /// <summary>
        /// Gets the name of the backup file.
        /// </summary>
        /// <value>
        /// The name of the backup file.
        /// </value>
        public static string BackupFileName
        {
            get
            {
                return Path.Combine(GlobalConfig.BackupFolder, "KrentoBackup" + DateTime.Now.Year.ToString(CultureInfo.InvariantCulture) +
                    DateTime.Now.Month.ToString(CultureInfo.InvariantCulture) + DateTime.Now.Day.ToString(CultureInfo.InvariantCulture) + ".zip");
            }
        }

        /// <summary>
        /// Gets the application folder.
        /// </summary>
        /// <value>The application folder.</value>
        public static string ApplicationFolder
        {
            get
            {
                return applicationFolder;
            }
        }

        /// <summary>
        /// Gets the application drive.
        /// </summary>
        public static string ApplicationDrive
        {
            get
            {
                return applicationDrive;
            }
        }

        /// <summary>
        /// Gets the name of the krento settings file. This is an user - related settings
        /// The name of the file is Krento.ini
        /// </summary>
        /// <value>The name of the krento settings file.</value>
        public static string KrentoSettingsFileName
        {
            get { return krentoSettingsFileName; }
        }

        /// <summary>
        /// Gets the name of the krento config file.
        /// </summary>
        /// <value>
        /// The name of the krento config file.
        /// </value>
        public static string KrentoConfigFileName
        {
            get { return krentoConfigFileName; }
        }

        /// <summary>
        /// Gets the name of the key mapping file.
        /// </summary>
        /// <value>The name of the key mapping file.</value>
        public static string KeyMappingFileName
        {
            get { return keyMappingFileName; }
        }

        /// <summary>
        /// Gets the name of the site images file.
        /// </summary>
        /// <value>The name of the site images file.</value>
        public static string SiteImagesFileName
        {
            get { return siteImagesFileName; }
        }

        /// <summary>
        /// Gets the name of the app images file.
        /// </summary>
        /// <value>The name of the app images file.</value>
        public static string AppImagesFileName
        {
            get { return appImagesFileName; }
        }

        /// <summary>
        /// Gets the server root.
        /// </summary>
        public static string ServerRoot
        {
            get { return serverRoot; }
        }

        /// <summary>
        /// Gets the index page.
        /// </summary>
        public static string IndexPage
        {
            get { return indexPage; }
        }

        /// <summary>
        /// Gets the rolling stones folder. This is a folder where Krento stores *.circle files.
        /// </summary>
        /// <value>The rolling stones folder.</value>
        public static string RollingStonesFolder
        {
            get { return rollingStonesFolder; }
        }

        /// <summary>
        /// Gets the stone classes folder. In this folder Krento stores assemblies with additional
        /// stones classes
        /// </summary>
        /// <value>The stone classes folder.</value>
        public static string StoneClasses
        {
            get { return stoneClasses; }
        }

        /// <summary>
        /// Gets the docklets folder.
        /// </summary>
        /// <value>The docklets folder.</value>
        public static string DockletsFolder
        {
            get { return dockletsFolder; }
        }

        /// <summary>
        /// Gets the icons folder.
        /// </summary>
        /// <value>The icons folder.</value>
        public static string IconsFolder
        {
            get { return iconsFolder; }
        }

        /// <summary>
        /// Gets the site icons folder.
        /// </summary>
        /// <value>The site icons folder.</value>
        public static string SiteIconsFolder
        {
            get { return siteIconsFolder; }
        }

        /// <summary>
        /// Gets the ring icons folder.
        /// </summary>
        public static string RingIconsFolder
        {
            get { return ringsIconsFolder; }
        }

        /// <summary>
        /// Gets the app icons folder.
        /// </summary>
        public static string AppIconsFolder
        {
            get { return appIconsFolder; }
        }

        public static string ExtIconsFolder
        {
            get { return extIconsFolder; }
        }

        /// <summary>
        /// Gets the languages folder.
        /// </summary>
        /// <value>The languages folder.</value>
        public static string LanguagesFolder
        {
            get { return languagesFolder; }
        }

        /// <summary>
        /// Gets the menu skins folder.
        /// </summary>
        /// <value>The menu skins folder.</value>
        public static string MenusFolder
        {
            get { return menusFolder; }
        }

        /// <summary>
        /// Gets the toys folder.
        /// </summary>
        /// <value>The toys folder.</value>
        public static string ToysFolder
        {
            get { return toysFolder; }
        }

        /// <summary>
        /// Gets the downloads folder.
        /// </summary>
        public static string DownloadsFolder
        {
            get { return downloadsFolder; }
        }

        /// <summary>
        /// Gets the rolling stones cache folder.
        /// </summary>
        /// <value>The rolling stones cache folder.</value>
        public static string RollingStonesCache
        {
            get { return cacheFolder; }
        }

        /// <summary>
        /// Gets the app shortcuts folder.
        /// </summary>
        /// <value>The app shortcuts folder.</value>
        public static string AppShortcuts
        {
            get { return appShortcuts; }
        }

        /// <summary>
        /// Gets the skins folder. In this folder Krento stores skins for Stones Manager
        /// </summary>
        /// <value>The skins folder.</value>
        public static string SkinsFolder
        {
            get { return skinsFolder; }
        }

        /// <summary>
        /// Gets the add in root folder.
        /// </summary>
        /// <value>The add in root folder.</value>
        public static string AddInRootFolder
        {
            get { return extensionsFolder; }
        }

        /// <summary>
        /// Gets the folder path relative to the main folder.
        /// </summary>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns></returns>
        public static string GetFolderPath(string folderName)
        {
            return FoldersInfo.ConcatenatePath(MainFolder, folderName);
        }

        /// <summary>
        /// Gets the default stones settings file name. This is the main rolling stones file (Default.circle)
        /// </summary>
        /// <value>The default stones settings file name.</value>
        public static string HomeCircleName
        {
            get { return homeCircleName; }
        }

        /// <summary>
        /// Gets or sets the default name of the circle.
        /// </summary>
        /// <value>
        /// The default name of the circle.
        /// </value>
        public static string DefaultCircleName
        {
            get
            {
                if (string.IsNullOrEmpty(defaultCircleName))
                    return HomeCircleName;
                else
                    return defaultCircleName;
            }
            set { defaultCircleName = value; }
        }


        /// <summary>
        /// Gets the main folder.
        /// </summary>
        public static string MainFolder
        {
            get { return mainFolder; }
        }

#if !PORTABLE
        /// <summary>
        /// Gets the user specific data folder. [User Name]\Krento
        /// </summary>
        /// <value>The user specific data folder.</value>
        public static string UserSpecificDataFolder
        {
            get { return FoldersInfo.GetDataPath(FoldersInfo.UserAppDataPath); }
        }

        /// <summary>
        /// Gets the user specific documents folder.
        /// </summary>
        /// <value>The user specific documents folder.</value>
        public static string UserSpecificDocuments
        {
            get { return FoldersInfo.GetDataPath(FoldersInfo.UserDocumentsPath); }
        }
#endif

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace Laugris.Sage
{
    public sealed class KrentoCoreSkin : IDisposable
    {
        private string skinFileName;

        private int leftMargin;
        private int topMargin;
        private int bottomMargin;
        private int rightMargin;
        private int outsideLeftMargin;
        private int outsideTopMargin;
        private int outsideBottomMargin;
        private int outsideRightMargin;

        private int stoneMarginLeft;
        private int stoneMarginRight;
        private int stoneMarginTop;
        private int stoneMarginBottom;
        private int stoneBorder;

        private Bitmap background;
        private Bitmap stone;

        private string fontName = "Tahoma";
        private int fontSize = 14;
        private Color foreColor = Color.White;
        private Rectangle indicatorRect = new Rectangle(0, 0, 100, 100);
        private int buttonSize = 60;

        private string indicatorImageName;

        private int textOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="KrentoCoreSkin"/> class.
        /// </summary>
        /// <param name="skinFileName">Name of the skin file.</param>
        public KrentoCoreSkin(string skinFileName)
        {
            this.skinFileName = FileOperations.StripFileName(skinFileName);
            HintBodyColor = Color.Black;
            HintBorderColor = Color.Gainsboro;
            HintOutlineColor = Color.Black;

            MenuBorderColor = Color.Gainsboro;
            MenuOutlineColor = Color.Black;
            MenuBodyColor = Color.Black;
            MenuNormalTextColor = Color.White;
            MenuDisabledTextColor = Color.Gray;
            MenuSelectedTextColor = Color.Black;
            MenuFontSize = 12;
            MenuFontName = "Tahoma";
            MenuHighlightColor = Color.Orange;

            ButtonStoneSize = 26;
            ButtonStoneSpace = 4;

            ManagerWidth = GlobalSettings.WindowWidth;
            ManagerHeight = GlobalSettings.WindowHeight;
            Radius = GlobalSettings.Radius;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="KrentoCoreSkin"/> is reclaimed by garbage collection.
        /// </summary>
        ~KrentoCoreSkin()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the skin caption.
        /// </summary>
        /// <param name="skinIniFile">The skin ini file.</param>
        /// <returns></returns>
        public static string GetSkinCaption(string skinIniFile)
        {
            string result = null;

            if (string.IsNullOrEmpty(skinIniFile))
                return null;

            string fullSkinIniFile = FileOperations.StripFileName(skinIniFile);

            if (FileOperations.FileExists(fullSkinIniFile))
            {
                string defaultCaption = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(fullSkinIniFile));
                result = NativeMethods.ReadString(fullSkinIniFile, "Info", "Name", defaultCaption);
                return result;
            }
            else
                return null;
        }

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName
        {
            get { return fontName; }
            set { fontName = value; }
        }

        public string ButtonHomeNormal { get; set; }
        public string ButtonHomeFocused { get; set; }
        public string ButtonHomePressed { get; set; }

        public string ButtonLauncherNormal { get; set; }
        public string ButtonLauncherFocused { get; set; }
        public string ButtonLauncherPressed { get; set; }

        public string ButtonSettingsNormal { get; set; }
        public string ButtonSettingsFocused { get; set; }
        public string ButtonSettingsPressed { get; set; }

        public string ButtonBackNormal { get; set; }
        public string ButtonBackFocused { get; set; }
        public string ButtonBackPressed { get; set; }

        public string ButtonForwardNormal { get; set; }
        public string ButtonForwardFocused { get; set; }
        public string ButtonForwardPressed { get; set; }

        public string ButtonStartMenuNormal { get; set; }
        public string ButtonStartMenuFocused { get; set; }
        public string ButtonStartMenuPressed { get; set; }

        public string ButtonPowerNormal { get; set; }
        public string ButtonPowerFocused { get; set; }
        public string ButtonPowerPressed { get; set; }

        public string ButtonStoneAbout { get; set; }
        public string ButtonStoneSelect { get; set; }
        public string ButtonStoneEdit { get; set; }
        public string ButtonStoneDelete { get; set; }

        public int ButtonStoneSize { get; set; }
        public int ButtonStoneSpace { get; set; }

        public Color MenuBorderColor { get; set; }
        public Color MenuOutlineColor { get; set; }
        public Color MenuBodyColor { get; set; }
        public Color MenuNormalTextColor { get; set; }
        public Color MenuDisabledTextColor { get; set; }
        public Color MenuSelectedTextColor { get; set; }
        public int MenuFontSize { get; set; }
        public string MenuFontName { get; set; }
        public Color MenuHighlightColor { get; set; }

        public int ManagerWidth { get; set; }
        public int ManagerHeight { get; set; }
        public int Radius { get; set; }

        public int ButtonSize
        {
            get { return buttonSize; }
            set { buttonSize = value; }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }

        public string MenuSkin { get; set; }

        /// <summary>
        /// Gets or sets the color of the text.
        /// </summary>
        /// <value>The color of the text.</value>
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        /// <summary>
        /// Gets the background image.
        /// </summary>
        /// <value>The background image.</value>
        public Bitmap Background
        {
            get { return background; }
        }

        /// <summary>
        /// Gets the stone background image.
        /// </summary>
        /// <value>The stone background image.</value>
        public Bitmap Stone
        {
            get { return stone; }
        }

        /// <summary>
        /// Gets or sets the stone background left margin
        /// </summary>
        /// <value>The stone background left margin.</value>
        public int StoneMarginLeft
        {
            get { return stoneMarginLeft; }
            set { stoneMarginLeft = value; }
        }

        public int StoneMarginRight
        {
            get { return stoneMarginRight; }
            set { stoneMarginRight = value; }
        }

        public int StoneMarginBottom
        {
            get { return stoneMarginBottom; }
            set { stoneMarginBottom = value; }
        }

        public int StoneMarginTop
        {
            get { return stoneMarginTop; }
            set { stoneMarginTop = value; }
        }

        public int LeftMargin
        {
            get { return leftMargin; }
            set { leftMargin = value; }
        }

        public int TopMargin
        {
            get { return topMargin; }
            set { topMargin = value; }
        }

        public int BottomMargin
        {
            get { return bottomMargin; }
            set { bottomMargin = value; }
        }

        public int RightMargin
        {
            get { return rightMargin; }
            set { rightMargin = value; }
        }


        public int OutsideLeftMargin
        {
            get { return outsideLeftMargin; }
            set { outsideLeftMargin = value; }
        }

        public int OutsideTopMargin
        {
            get { return outsideTopMargin; }
            set { outsideTopMargin = value; }
        }

        public int OutsideBottomMargin
        {
            get { return outsideBottomMargin; }
            set { outsideBottomMargin = value; }
        }

        public int OutsideRightMargin
        {
            get { return outsideRightMargin; }
            set { outsideRightMargin = value; }
        }

        public int StoneBorder
        {
            get { return stoneBorder; }
            set { stoneBorder = value; }
        }


        public string IndicatorImageName
        {
            get { return indicatorImageName; }
            set { indicatorImageName = value; }
        }

        public int TextOffset
        {
            get { return textOffset; }
            set { textOffset = value; }
        }


        public int IndicatorLeft { get; set; }
        public int IndicatorBottom { get; set; }

        public Color HintOutlineColor { get; set; }
        public Color HintBorderColor { get; set; }
        public Color HintBodyColor { get; set; }

        public bool LoadFromFile()
        {
            string iniFileName = "";
            string stoneName;
            string backgroundName;
            string skinFolder = "";

            if (!File.Exists(skinFileName))
                return false;
            else
            {
                iniFileName = skinFileName;
                skinFolder = Path.GetDirectoryName(skinFileName);
            }


            MemIniFile ini = new MemIniFile(iniFileName);
            try
            {
                ini.Load();

                MenuSkin = ini.ReadString("Menu", "Theme", null);

                ColorConverter cc = new ColorConverter();
                RectangleConverter rc = new RectangleConverter();
                fontName = ini.ReadString("Info", "FontName", "Tahoma");
                fontSize = ini.ReadInteger("Info", "FontSize", 14);
                try
                {
                    foreColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Info", "Color", "White"));
                }
                catch
                {
                    foreColor = Color.White;
                }

                try
                {
                    HintOutlineColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Hint", "OutlineColor", "Black"));
                }
                catch
                {
                    HintOutlineColor = Color.Black;
                }

                try
                {
                    HintBorderColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Hint", "BorderColor", "Gainsboro"));
                }
                catch
                {
                    HintBorderColor = Color.Gainsboro;
                }

                try
                {
                    HintBodyColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Hint", "BodyColor", "Black"));
                }
                catch
                {
                    HintBodyColor = Color.Black;
                }

                ButtonStoneSize = ini.ReadInteger("Hint", "ButtonSize", 26);
                ButtonStoneSpace = ini.ReadInteger("Hint", "ButtonSpace", 4);

                try
                {
                    MenuBorderColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "BorderColor", "Gainsboro"));
                }
                catch
                {
                    MenuBorderColor = Color.Gainsboro;
                }

                try
                {
                    MenuOutlineColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "OutlineColor", "Black"));
                }
                catch
                {
                    MenuOutlineColor = Color.Black;
                }

                try
                {
                    MenuBodyColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "BodyColor", "Black"));
                }
                catch
                {
                    MenuBodyColor = Color.Black;
                }

                try
                {
                    MenuNormalTextColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "NormalTextColor", "White"));
                }
                catch
                {
                    MenuNormalTextColor = Color.White;
                }

                try
                {
                    MenuDisabledTextColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "DisabledTextColor", "Gray"));
                }
                catch
                {
                    MenuDisabledTextColor = Color.Gray;
                }

                try
                {
                    MenuSelectedTextColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "SelectedTextColor", "Black"));
                }
                catch
                {
                    MenuSelectedTextColor = Color.Black;
                }

                MenuFontSize = ini.ReadInteger("Menu", "FontSize", 12);

                MenuFontName = ini.ReadString("Menu", "FontName", "Tahoma");

                try
                {
                    MenuHighlightColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "HighlightColor", "Orange"));
                }
                catch
                {
                    MenuHighlightColor = Color.Orange;
                }


                ButtonStoneAbout = ini.ReadString("Hint", "ButtonStoneAbout", string.Empty);
                if (!string.IsNullOrEmpty(ButtonStoneAbout))
                {
                    ButtonStoneAbout = Path.Combine(skinFolder, ButtonStoneAbout);
                }


                ButtonStoneSelect = ini.ReadString("Hint", "ButtonStoneSelect", string.Empty);
                if (!string.IsNullOrEmpty(ButtonStoneSelect))
                {
                    ButtonStoneSelect = Path.Combine(skinFolder, ButtonStoneSelect);
                }


                ButtonStoneEdit = ini.ReadString("Hint", "ButtonStoneEdit", string.Empty);
                if (!string.IsNullOrEmpty(ButtonStoneEdit))
                {
                    ButtonStoneEdit = Path.Combine(skinFolder, ButtonStoneEdit);
                }

                ButtonStoneDelete = ini.ReadString("Hint", "ButtonStoneDelete", string.Empty);
                if (!string.IsNullOrEmpty(ButtonStoneDelete))
                {
                    ButtonStoneDelete = Path.Combine(skinFolder, ButtonStoneDelete);
                }


                ButtonBackNormal = ini.ReadString("ButtonBack", "Normal", string.Empty);
                if (!string.IsNullOrEmpty(ButtonBackNormal))
                {
                    ButtonBackNormal = Path.Combine(skinFolder, ButtonBackNormal);
                }

                ButtonBackFocused = ini.ReadString("ButtonBack", "Focused", string.Empty);
                if (!string.IsNullOrEmpty(ButtonBackFocused))
                {
                    ButtonBackFocused = Path.Combine(skinFolder, ButtonBackFocused);
                }

                ButtonBackPressed = ini.ReadString("ButtonBack", "Pressed", string.Empty);
                if (!string.IsNullOrEmpty(ButtonBackPressed))
                {
                    ButtonBackPressed = Path.Combine(skinFolder, ButtonBackPressed);
                }


                ButtonStartMenuNormal = ini.ReadString("ButtonStartMenu", "Normal", string.Empty);
                if (!string.IsNullOrEmpty(ButtonStartMenuNormal))
                {
                    ButtonStartMenuNormal = Path.Combine(skinFolder, ButtonStartMenuNormal);
                }

                ButtonStartMenuFocused = ini.ReadString("ButtonStartMenu", "Focused", string.Empty);
                if (!string.IsNullOrEmpty(ButtonStartMenuFocused))
                {
                    ButtonStartMenuFocused = Path.Combine(skinFolder, ButtonStartMenuFocused);
                }

                ButtonStartMenuPressed = ini.ReadString("ButtonStartMenu", "Pressed", string.Empty);
                if (!string.IsNullOrEmpty(ButtonStartMenuPressed))
                {
                    ButtonStartMenuPressed = Path.Combine(skinFolder, ButtonStartMenuPressed);
                }

                ButtonForwardNormal = ini.ReadString("ButtonForward", "Normal", string.Empty);
                if (!string.IsNullOrEmpty(ButtonForwardNormal))
                {
                    ButtonForwardNormal = Path.Combine(skinFolder, ButtonForwardNormal);
                }
                else

                    ButtonForwardFocused = ini.ReadString("ButtonForward", "Focused", string.Empty);
                if (!string.IsNullOrEmpty(ButtonForwardFocused))
                {
                    ButtonForwardFocused = Path.Combine(skinFolder, ButtonForwardFocused);
                }

                ButtonForwardPressed = ini.ReadString("ButtonForward", "Pressed", string.Empty);
                if (!string.IsNullOrEmpty(ButtonForwardPressed))
                {
                    ButtonForwardPressed = Path.Combine(skinFolder, ButtonForwardPressed);
                }

                ButtonPowerNormal = ini.ReadString("ButtonPower", "Normal", string.Empty);
                if (!string.IsNullOrEmpty(ButtonPowerNormal))
                {
                    ButtonPowerNormal = Path.Combine(skinFolder, ButtonPowerNormal);
                }

                ButtonPowerFocused = ini.ReadString("ButtonPower", "Focused", string.Empty);
                if (!string.IsNullOrEmpty(ButtonPowerFocused))
                {
                    ButtonPowerFocused = Path.Combine(skinFolder, ButtonPowerFocused);
                }

                ButtonPowerPressed = ini.ReadString("ButtonPower", "Pressed", string.Empty);
                if (!string.IsNullOrEmpty(ButtonPowerPressed))
                {
                    ButtonPowerPressed = Path.Combine(skinFolder, ButtonPowerPressed);
                }

                ButtonHomeNormal = ini.ReadString("ButtonHome", "Normal", string.Empty);
                if (!string.IsNullOrEmpty(ButtonHomeNormal))
                {
                    ButtonHomeNormal = Path.Combine(skinFolder, ButtonHomeNormal);
                }

                ButtonHomeFocused = ini.ReadString("ButtonHome", "Focused", string.Empty);
                if (!string.IsNullOrEmpty(ButtonHomeFocused))
                {
                    ButtonHomeFocused = Path.Combine(skinFolder, ButtonHomeFocused);
                }

                ButtonHomePressed = ini.ReadString("ButtonHome", "Pressed", string.Empty);
                if (!string.IsNullOrEmpty(ButtonHomePressed))
                {
                    ButtonHomePressed = Path.Combine(skinFolder, ButtonHomePressed);
                }

                ButtonLauncherNormal = ini.ReadString("ButtonLauncher", "Normal", string.Empty);
                if (!string.IsNullOrEmpty(ButtonLauncherNormal))
                {
                    ButtonLauncherNormal = Path.Combine(skinFolder, ButtonLauncherNormal);
                }

                ButtonLauncherFocused = ini.ReadString("ButtonLauncher", "Focused", string.Empty);
                if (!string.IsNullOrEmpty(ButtonLauncherFocused))
                {
                    ButtonLauncherFocused = Path.Combine(skinFolder, ButtonLauncherFocused);
                }

                ButtonLauncherPressed = ini.ReadString("ButtonLauncher", "Pressed", string.Empty);
                if (!string.IsNullOrEmpty(ButtonLauncherPressed))
                {
                    ButtonLauncherPressed = Path.Combine(skinFolder, ButtonLauncherPressed);
                }

                ButtonSettingsNormal = ini.ReadString("ButtonSettings", "Normal", string.Empty);
                if (!string.IsNullOrEmpty(ButtonSettingsNormal))
                {
                    ButtonSettingsNormal = Path.Combine(skinFolder, ButtonSettingsNormal);
                }

                ButtonSettingsFocused = ini.ReadString("ButtonSettings", "Focused", string.Empty);
                if (!string.IsNullOrEmpty(ButtonSettingsFocused))
                {
                    ButtonSettingsFocused = Path.Combine(skinFolder, ButtonSettingsFocused);
                }

                ButtonSettingsPressed = ini.ReadString("ButtonSettings", "Pressed", string.Empty);
                if (!string.IsNullOrEmpty(ButtonSettingsPressed))
                {
                    ButtonSettingsPressed = Path.Combine(skinFolder, ButtonSettingsPressed);
                }

                ManagerWidth = ini.ReadInteger("Background", "ManagerWidth", GlobalSettings.WindowWidth);
                ManagerHeight = ini.ReadInteger("Background", "ManagerHeight", GlobalSettings.WindowHeight);
                Radius = ini.ReadInteger("Background", "Radius", GlobalSettings.Radius);

                string sectionName = "Background";

                if (ini.SectionExists("Background"))
                    sectionName = "Background";
                else
                    if (ini.SectionExists("Tile"))
                        sectionName = "Tile";
                    else
                        if (ini.SectionExists("BackgroundTop"))
                            sectionName = "BackgroundTop";
                        else
                            if (ini.SectionExists("BackgroundBottom"))
                                sectionName = "BackgroundBottom";

                if (ini.ValueExists(sectionName, "LeftMargin"))
                    leftMargin = ini.ReadInteger(sectionName, "LeftMargin", 0);
                else
                    leftMargin = ini.ReadInteger(sectionName, "LeftWidth", 0);

                textOffset = ini.ReadInteger("Background", "TextOffset", leftMargin);

                if (ini.ValueExists(sectionName, "RightMargin"))
                    rightMargin = ini.ReadInteger(sectionName, "RightMargin", 0);
                else
                    rightMargin = ini.ReadInteger(sectionName, "RightWidth", 0);

                if (ini.ValueExists(sectionName, "TopMargin"))
                    topMargin = ini.ReadInteger(sectionName, "TopMargin", 0);
                else
                    topMargin = ini.ReadInteger(sectionName, "TopHeight", 0);

                if (ini.ValueExists(sectionName, "BottomMargin"))
                    bottomMargin = ini.ReadInteger(sectionName, "BottomMargin", 0);
                else
                    bottomMargin = ini.ReadInteger(sectionName, "BottomHeight", 0);

                if (ini.ValueExists(sectionName, "Outside-LeftMargin"))
                    outsideLeftMargin = ini.ReadInteger(sectionName, "Outside-LeftMargin", 0);
                else
                    outsideLeftMargin = ini.ReadInteger(sectionName, "OutsideBorderLeft", 0);

                if (ini.ValueExists(sectionName, "Outside-TopMargin"))
                    outsideTopMargin = ini.ReadInteger(sectionName, "Outside-TopMargin", 0);
                else
                    outsideTopMargin = ini.ReadInteger(sectionName, "OutsideBorderTop", 0);

                if (ini.ValueExists(sectionName, "Outside-BottomMargin"))
                    outsideBottomMargin = ini.ReadInteger(sectionName, "Outside-BottomMargin", 0);
                else
                    outsideBottomMargin = ini.ReadInteger(sectionName, "OutsideBorderBottom", 0);

                if (ini.ValueExists(sectionName, "Outside-RightMargin"))
                    outsideRightMargin = ini.ReadInteger(sectionName, "Outside-RightMargin", 0);
                else
                    outsideRightMargin = ini.ReadInteger(sectionName, "OutsideBorderRight", 0);

                backgroundName = ini.ReadString(sectionName, "Image", string.Empty);

                ButtonSize = ini.ReadInteger("Background", "ButtonSize", 60);


                //read stone info
                indicatorRect = (Rectangle)rc.ConvertFromInvariantString(ini.ReadString("Stone", "Indicator-Rect", "0, 0, 100, 100"));
                IndicatorLeft = ini.ReadInteger("Stone", "Indicator-Left", 0);
                IndicatorBottom = ini.ReadInteger("Stone", "Indicator-Bottom", 0);

                indicatorImageName = ini.ReadString("Stone", "Indicator", string.Empty);
                if (!string.IsNullOrEmpty(indicatorImageName))
                {
                    indicatorImageName = Path.Combine(skinFolder, indicatorImageName);
                }


                if (ini.SectionExists("Stone"))
                {
                    stoneMarginTop = ini.ReadInteger("Stone", "OutsideBorderTop", 0);
                    stoneMarginBottom = ini.ReadInteger("Stone", "OutsideBorderBottom", 0);
                    stoneMarginLeft = ini.ReadInteger("Stone", "OutsideBorderLeft", 0);
                    stoneMarginRight = ini.ReadInteger("Stone", "OutsideBorderRight", 0);
                    stoneBorder = ini.ReadInteger("Stone", "Border", 0);
                    stoneName = ini.ReadString("Stone", "Image", backgroundName);
                }
                else
                {
                    stoneMarginTop = outsideTopMargin;
                    stoneMarginBottom = outsideBottomMargin;
                    stoneMarginLeft = outsideLeftMargin;
                    stoneMarginRight = outsideRightMargin;
                    stoneName = backgroundName;
                    stoneBorder = 0;
                }

                if (string.IsNullOrEmpty(backgroundName))
                {
                    return false;
                }
                else
                {
                    if (background != null)
                    {
                        background.Dispose();
                        background = null;
                    }
                    try
                    {
                        background = FastBitmap.FromFile(Path.Combine(skinFolder, backgroundName));
                        if (background == null)
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                }

                if (string.IsNullOrEmpty(stoneName))
                {
                    return false;
                }
                else
                {
                    if (stone != null)
                    {
                        stone.Dispose();
                        stone = null;
                    }
                    try
                    {
                        stone = FastBitmap.FromFile(Path.Combine(skinFolder, stoneName));
                        if (stone == null)
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            finally
            {
                ini.Dispose();
            }
            return true;
        }


        public int TotalLeft
        {
            get { return LeftMargin + OutsideLeftMargin; }
        }

        public int TotalRight
        {
            get { return RightMargin + OutsideRightMargin; }
        }

        public int TotalTop
        {
            get { return TopMargin + OutsideTopMargin; }
        }

        public int TotalBottom
        {
            get { return BottomMargin + OutsideBottomMargin; }
        }

        public int IndicatorWidth
        {
            get { return indicatorRect.Width; }
        }

        public int IndicatorHeight
        {
            get { return indicatorRect.Height; }
        }

        public void DisposeBackground()
        {
            if (background != null)
            {
                background.Dispose();
                background = null;
            }

            if (stone != null)
            {
                stone.Dispose();
                stone = null;
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeBackground();
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

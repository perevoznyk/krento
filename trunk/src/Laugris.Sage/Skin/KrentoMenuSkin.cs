using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace Laugris.Sage
{
    public sealed class KrentoMenuSkin : IDisposable
    {
        private string skinFileName;
        private Bitmap background;
        private Bitmap highlight;
        private string caption;
        private int leftMargin;
        private int topMargin;
        private int bottomMargin;
        private int textLeftMargin;
        private int textRightMargin;
        private int textOffset = 4;
        private int outerBorderLeft = 21;
        private int outerBorderTop = 21;
        private int scrollDividerSize = 6;
        private int itemHeight = 20;
        private string fontName = "Tahoma";
        private int fontSize = 12;
        private int imageSize = 16;
        private bool springBottom = false;
        private Color foreColor = Color.White;
        private Color selectedColor = Color.FromArgb(238, 175, 238, 238);
        private Color disabledColor = Color.Gray;

        public static string GetSkinCaption(string skinIniFile)
        {
            string result = null;

            if (string.IsNullOrEmpty(skinIniFile))
                return null;

            string fullSkinIniFile = FileOperations.StripFileName(skinIniFile);

            if (FileOperations.FileExists(fullSkinIniFile))
            {
                string defaultCaption = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(fullSkinIniFile));
                result = NativeMethods.ReadString(fullSkinIniFile, "Krento", "Caption", defaultCaption);
                return result;
            }
            else
                return null;
        }

        public KrentoMenuSkin(string skinFileName)
        {
            this.skinFileName = FileOperations.StripFileName(skinFileName);
        }

        ~KrentoMenuSkin()
        {
            Dispose(false);
        }

        public string FileName
        {
            get { return this.skinFileName; }
        }

        public string Caption
        {
            get { return caption; }
            set { caption = value; }
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

        public int OuterBorderLeft
        {
            get { return outerBorderLeft; }
            set { outerBorderLeft = value; }
        }

        public int OuterBorderTop
        {
            get { return outerBorderTop; }
            set { outerBorderTop = value; }
        }

        public Bitmap Background
        {
            get { return background; }
        }

        public Bitmap Highlight
        {
            get { return highlight; }
        }

        public string FontName
        {
            get { return fontName; }
            set { fontName = value; }
        }

        public int FontSize
        {
            get { return fontSize; }
            set { fontSize = value; }
        }

        public bool SpringBottom
        {
            get { return springBottom; }
            set { springBottom = value; }
        }

        /// <summary>
        /// The text color of the menu item
        /// </summary>
        /// <value>Text text color of the menu item</value>
        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        /// <summary>
        /// Gets or sets the color of the selected menu item.
        /// </summary>
        /// <value>The color of the selected menu item.</value>
        public Color SelectedColor
        {
            get { return selectedColor; }
            set { selectedColor = value; }
        }

        public Color DisabledColor
        {
            get { return disabledColor; }
            set { disabledColor = value; }
        }

        /// <summary>
        /// Gets or sets the size of the image.
        /// </summary>
        /// <value>The size of the image.</value>
        public int ImageSize
        {
            get { return imageSize; }
            set { imageSize = value; }
        }

        public int TextTopMargin { get; set; }

        public int TextBottomMargin { get; set; }

        public bool LoadFromFile()
        {
            string iniFileName;

            if (!File.Exists(skinFileName))
                return false;

            iniFileName = skinFileName;


            MemIniFile ini = new MemIniFile(iniFileName);
            try
            {
                ini.Load();
                ColorConverter cc = new ColorConverter();
                imageSize = ini.ReadInteger("Menu", "ImageSize", 16);
                itemHeight = ini.ReadInteger("Menu", "ItemHeight", 20);
                leftMargin = ini.ReadInteger("Menu", "LeftMargin", 38);
                topMargin = ini.ReadInteger("Menu", "TopMargin", 28);
                TextTopMargin = ini.ReadInteger("Menu", "TextTopMargin", 28);
                TextBottomMargin = ini.ReadInteger("Menu", "TextBottomMargin", 28);
                bottomMargin = ini.ReadInteger("Menu", "BottomMargin", topMargin);
                textLeftMargin = ini.ReadInteger("Menu", "TextLeftMargin", 28);
                textRightMargin = ini.ReadInteger("Menu", "TextRightMargin", 24);
                textOffset = ini.ReadInteger("Menu", "TextOffset", 4);
                outerBorderLeft = ini.ReadInteger("Menu", "OuterBorderLeft", 21);
                outerBorderTop = ini.ReadInteger("Menu", "OuterBorderTop", 21);
                scrollDividerSize = ini.ReadInteger("Menu", "DividerSize", 6);
                springBottom = ini.ReadBool("Menu", "SpringBottom", false);

                fontName = ini.ReadString("Krento", "FontName", "Tahoma");
                fontSize = ini.ReadInteger("Krento", "FontSize", 12);
                caption = ini.ReadString("Krento", "Caption", Path.GetFileNameWithoutExtension(Path.GetDirectoryName(skinFileName)));

                foreColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "ForeColor", "White"));
                selectedColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "SelectedColor",
                    "238, 175, 238, 238"));
                disabledColor = (Color)cc.ConvertFromInvariantString(ini.ReadString("Menu", "DisabledColor", "Gray"));


                string backgroundName = ini.ReadString("Menu", "Image", string.Empty);

                if (!string.IsNullOrEmpty(backgroundName))
                {
                    if (background != null)
                    {
                        background.Dispose();
                        background = null;
                    }
                    try
                    {
                        string skinFolder = Path.GetDirectoryName(skinFileName);
                        background = FastBitmap.FromFile(Path.Combine(skinFolder, backgroundName));
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                    return false;


                string highlightName = ini.ReadString("Menu", "Highlight", string.Empty);

                if (!string.IsNullOrEmpty(highlightName))
                {
                    if (highlight != null)
                    {
                        highlight.Dispose();
                        highlight = null;
                    }
                    try
                    {
                        string skinFolder = Path.GetDirectoryName(skinFileName);
                        highlight = FastBitmap.FromFile(Path.Combine(skinFolder, highlightName));
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                    return false;

            }
            finally
            {
                ini.Dispose();
            }

            return true;
        }


        public int ItemHeight
        {
            get { return itemHeight; }
            set { itemHeight = value; }
        }

        /// <summary>
        /// Gets or sets the text highlight bar left margin.
        /// </summary>
        /// <value>The text highlight bar left margin.</value>
        public int TextLeftMargin
        {
            get { return textLeftMargin; }
            set { textLeftMargin = value; }
        }

        /// <summary>
        /// Gets or sets the text offset from the left side of the highlight bar.
        /// </summary>
        /// <value>The text offset from the left side of the highlight bar.</value>
        public int TextOffset
        {
            get { return textOffset; }
            set { textOffset = value; }
        }

        public int TextRightMargin
        {
            get { return textRightMargin; }
            set { textRightMargin = value; }
        }

        public int ScrollDividerSize
        {
            get { return scrollDividerSize; }
            set { scrollDividerSize = value; }
        }


        #region IDisposable Members

        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeBackground();
            }
        }

        public void DisposeBackground()
        {
            if (background != null)
            {
                background.Dispose();
                background = null;
            }

            if (highlight != null)
            {
                highlight.Dispose();
                highlight = null;
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

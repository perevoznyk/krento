using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Drawing;
using System.Windows.Forms;
using Laugris.Sage.Launcher;
using System.IO;

namespace Krento.RollingStones
{
    /// <summary>
    /// The Windows start menu items search dialog
    /// </summary>
    public sealed class StartMenuDialog : FolderView
    {
        private string searchText;
        private readonly Catalog catalog;
        private List<CatalogItem> result;
        private readonly Bitmap searchField;
        private Bitmap textField;
        private Font searchFont;
        private int searchTextHeight;
        private StringFormat searchFormat;

        public StartMenuDialog(IntPtr parentWindow)
            : base(parentWindow)
        {
            //ItemSize = 104;
            Distance = 20;
            searchFormat = new StringFormat();
            searchFormat.Alignment = StringAlignment.Near;

            searchFont = new Font("Tahoma", 12, FontStyle.Bold, GraphicsUnit.Pixel);
            searchTextHeight = (int)Canvas.MeasureString("Wg", searchFont, ItemSize + Distance / 2, searchFormat).Height + 4;

            searchFormat.LineAlignment = StringAlignment.Center;
            searchFormat.Trimming = StringTrimming.EllipsisCharacter;
            searchFormat.FormatFlags = StringFormatFlags.LineLimit;

            searchField = NativeThemeManager.LoadBitmap("TextField.png");
            HeaderText = SR.Applications;
            
            this.Font = new Font("Tahoma", 10);
            
            ExtraItemsOffset = searchTextHeight + 14;
            searchText = string.Empty;
            catalog = new Catalog { Limit = 15 };
            result = new List<CatalogItem>();
            catalog.Rebuild();
            ScanTime = DateTime.Now;
        }

        public bool Executed { get; set; }

        public DateTime ScanTime;

        public override bool Execute(int leftCorner, int topCorner)
        {
            bool answer = base.Execute(leftCorner, topCorner);
            Executed = true;
            return answer;
        }

        internal override void LoadBackground()
        {
            base.LoadBackground();
            if (textField != null)
            {
                textField.Dispose();
                textField = null;
            }

            textField = BitmapPainter.BuildImageFromSkin(searchField, Width - Distance * 2, searchTextHeight + 4, new SkinOffset(12, 12, 12, 12));
        }

        public void RebuildCatalog()
        {
            catalog.Rebuild();
            ScanTime = DateTime.Now;
        }

        public void HistoryAdd()
        {
            if (catalog.History.ContainsKey(searchText))
                catalog.History.Remove(searchText);
            if ((SelectedItem > -1) && (SelectedItem < Items.Count))
                catalog.History.Add(searchText, result[SelectedItem]);
        }

        protected override void PaintWindowIcon()
        {
            FolderItem item = null;
            if ((SelectedItem > -1) && (SelectedItem < Items.Count))
                item = Items[SelectedItem];
            if ((item != null) && (item.Logo != null))
                DrawLogoImage(item.Logo);
            else
                base.PaintWindowIcon();
        }

        protected override void PaintHeaderText()
        {
            FolderItem item = null;
            if ((SelectedItem > -1) && (SelectedItem < Items.Count))
                item = Items[SelectedItem];
            if ((item != null) && (item.Logo != null))
            {
                Canvas.DrawString(item.Description, searchFont, Brushes.White, new RectangleF(Distance + LogoSize + 4, Distance, Width - Distance * 2 - LogoSize * 2 - 8, LogoSize), searchFormat);
            }
            else
                base.PaintHeaderText();
        }

        protected override void PaintWindowBackground()
        {
            base.PaintWindowBackground();
            Canvas.DrawImage(textField, new RectangleF(Distance, Distance + HeaderSize + 10, Width - Distance * 2, searchTextHeight + 4), new RectangleF(0, 0, textField.Width, textField.Height), GraphicsUnit.Pixel);
            if (!string.IsNullOrEmpty(searchText))
                Canvas.DrawString(searchText, searchFont, Brushes.Black, new RectangleF(Distance + 12, Distance + HeaderSize + 12, Width - Distance * 2 - 24, searchTextHeight), searchFormat);
            else
                Canvas.DrawString(SR.SearchText, searchFont, Brushes.DarkGray, new RectangleF(Distance + 12, Distance + HeaderSize + 12, Width - Distance * 2 - 24, searchTextHeight), searchFormat);
        }

        protected override void RepaintItems()
        {
            Canvas.SetClip(new Rectangle(0, 0, Width, Distance + HeaderSize));
            try
            {
                PaintItems();
            }
            finally
            {
                Canvas.ResetClip();
            }

            base.RepaintItems();
        }

        protected override void Dispose(bool disposing)
        {
            searchFormat.Dispose();

            if (textField != null)
            {
                textField.Dispose();
                textField = null;
            }

            catalog.Dispose();
            searchField.Dispose();
            base.Dispose(disposing);
        }

        protected override void BuildItems()
        {
            DisposeItems();

            int totalItems = 0;

            for (int i = 0; i < result.Count; i++)
            {
                string fullTarget = result[i].FullPath;

                totalItems++;

                if (totalItems > Limit)
                {
                    totalItems = Limit;
                    break;
                }

                FolderItem item = new FolderItem(fullTarget) { Logo = (Bitmap)result[i].Icon.Clone(), Name = result[i].ShortName };
                item.Description = result[i].Description;
                Items.Add(item);
            }
        }



        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; }
        }

        public void Research()
        {
            result.Clear();
            catalog.Search(searchText, result);
            BuildItems();
            CalcRowsAndColumns();
            CalcPositions();
            PaintItems();

        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Keys keyCode = e.KeyCode;
            if (e.Modifiers == Keys.None)
            {
                if ((keyCode == Keys.Delete) || (keyCode == Keys.Back))
                {
                    if (!string.IsNullOrEmpty(searchText))
                        searchText = searchText.Remove(searchText.Length - 1);
                    Research();
                }
            }
            if (e.Control && char.IsLetter((char)((ushort)keyCode)))
            {
                switch (keyCode)
                {
                    case Keys.V:
                        if (Clipboard.ContainsText())
                        {
                            searchText = Clipboard.GetText();
                            Research();
                        }
                        break;
                    case Keys.C:
                        if ((Items.Count > 0) && (SelectedItem > -1))
                        {
                            FileOperations.CopyFilesListToClipboard(new string[1] { Items[SelectedItem].FileName });
                        }
                        break;
                    default:
                        break;
                }
            }
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!Char.IsControl(e.KeyChar))
            {
                searchText = searchText + e.KeyChar;
                PaintItems();
                result.Clear();
                catalog.Search(searchText, result);
                BuildItems();
                CalcRowsAndColumns();
                CalcPositions();
                PaintItems();
            }
            base.OnKeyPress(e);
        }


    }
}

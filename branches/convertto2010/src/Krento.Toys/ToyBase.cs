using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using Laugris.Sage;

namespace Krento.Toys
{
    /// <summary>
    /// Base class for Krento Toys
    /// </summary>
    public class ToyBase : Window
    {
        private KrentoMenu popupMenu;

        public event EventHandler Load;
        public event EventHandler Unload;
        public event EventHandler<IniFileAccessArgs> LoadSettings;
        public event EventHandler<IniFileAccessArgs> SaveSettings;

        private string currentFolder;
        private bool toyEnabled;
        private string settingFileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToyBase"/> class.
        /// </summary>
        /// <param name="currentFolder">The current folder.</param>
        public ToyBase(string currentFolder)
            : base()
        {
            KrentoMenuItem item;

            Text = "Krento Toy";
            Name = "KrentoToy";
            toyEnabled = true;
            this.currentFolder = currentFolder;
            settingFileName = Path.Combine(currentFolder, "config.ini");
            this.BottomMostWindow = true;
            popupMenu = new KrentoMenu();

            item = popupMenu.AddItem();
            item.Caption = SR.StoneAbout;
            item.Name = "About";
            item.Execute += new EventHandler(AboutToy_Execute);

            item = popupMenu.AddItem();
            item.Caption = SR.HideToy;
            item.Name = "HideToy";
            item.Execute += new EventHandler(HideToy_Execute);

            item = popupMenu.AddItem();
            item.Caption = SR.Delete;
            item.Name = "Delete";
            item.Execute += new EventHandler(DeleteToy_Execute);

        }

        public IntPtr LaugrisHandle { get; set; }

        void HideToy_Execute(object sender, EventArgs e)
        {
            NativeMethods.PostMessage(LaugrisHandle, NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        private void DeleteToy_Execute(object sender, EventArgs e)
        {
            toyEnabled = false;
            NativeMethods.PostMessage(LaugrisHandle, NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }


        private void AboutToy_Execute(object sender, EventArgs e)
        {
            ToyAboutBox aboutBox = new ToyAboutBox(this.settingFileName);
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

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="ToyBase"/> is reclaimed by garbage collection.
        /// </summary>
        ~ToyBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the toy is enabled.
        /// When toy is not enabled it will be not loaded
        /// </summary>
        /// <value><c>true</c> if toy is enabled; otherwise, <c>false</c>.</value>
        public bool ToyEnabled
        {
            get { return toyEnabled; }
            set { toyEnabled = value; }
        }

        public string CurrentFolder
        {
            get { return currentFolder; }
        }


        /// <summary>
        /// Raises the <see cref="E:Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnLoad(EventArgs e)
        {
            if (Load != null)
                Load(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Unload"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnUnload(EventArgs e)
        {
            if (Unload != null)
            {
                Unload(this, e);
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SYSCOMMAND)
            {
                if (m.WParam == NativeMethods.SC_CLOSE)
                    return;
            }
            base.WndProc(ref m);
        }

        public string SettingsFileName
        {
            get { return settingFileName; }
        }


        protected virtual void OnSaveSettings(IniFileAccessArgs e)
        {
            EventHandler<IniFileAccessArgs> handler = SaveSettings;
            if (handler != null)
                handler(this, e);
        }


        protected virtual void OnLoadSettings(IniFileAccessArgs e)
        {
            EventHandler<IniFileAccessArgs> handler = LoadSettings;
            if (handler != null)
                handler(this, e);
        }
        
        protected virtual void SaveConfiguration()
        {
            try
            {
                MemIniFile iniFile = new MemIniFile(settingFileName);
                iniFile.Load();
                iniFile.WriteInteger("Settings", "Left", Left);
                iniFile.WriteInteger("Settings", "Top", Top);
                iniFile.WriteBool("Toy", "Enabled", ToyEnabled);
                OnSaveSettings(new IniFileAccessArgs(iniFile));
                iniFile.Save();
                iniFile.Dispose();
            }
            catch
            {
            }
        }

        protected virtual void ReadConfiguration()
        {
            try
            {
                Random rnd = new Random();
                MemIniFile iniFile = new MemIniFile(settingFileName, true);
                iniFile.Load();
                Left = iniFile.ReadInteger("Settings", "Left", rnd.Next(0, PrimaryScreen.Bounds.Width - 96));
                Top = iniFile.ReadInteger("Settings", "Top", rnd.Next(0, (int)((PrimaryScreen.Bounds.Height - 96)/2)));
                OnLoadSettings(new IniFileAccessArgs(iniFile));
                iniFile.Dispose();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Loads the image.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns></returns>
        public Image LoadImage(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            string imageFile;

            if (Path.IsPathRooted(fileName))
                imageFile = fileName;
            else
                imageFile = Path.Combine(currentFolder, fileName);

            try
            {
                Image tmp = Image.FromFile(imageFile);
                tmp = BitmapPainter.ResizeBitmap(tmp, tmp.Width, tmp.Height, true);
                return tmp;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Loads the image and resizes it
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        /// <returns></returns>
        public Image LoadImage(string fileName, int newWidth, int newHeight)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            string imageFile;

            if (Path.IsPathRooted(fileName))
                imageFile = fileName;
            else
                imageFile = Path.Combine(currentFolder, fileName);

            try
            {
                Image tmp = Image.FromFile(imageFile);
                tmp = BitmapPainter.ResizeBitmap(tmp, newWidth, newHeight, true);
                return tmp;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// Loads the image and resizes it proportionally
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        /// <param name="fillColor">Color to fill extra area</param>
        /// <returns></returns>
        public Image LoadImage(string fileName, int newWidth, int newHeight, Color fillColor)
        {
            if (string.IsNullOrEmpty(fileName))
                return null;

            string imageFile;

            if (Path.IsPathRooted(fileName))
                imageFile = fileName;
            else
                imageFile = Path.Combine(currentFolder, fileName);

            try
            {
                Image tmp = Image.FromFile(imageFile);

                int sourceWidth = tmp.Width;
                int sourceHeight = tmp.Height;

                float nPercent = 0;
                float nPercentW = 0;
                float nPercentH = 0;

                nPercentW = ((float)newWidth / (float)sourceWidth);
                nPercentH = ((float)newHeight / (float)sourceHeight);

                if (nPercentH < nPercentW)
                    nPercent = nPercentH;
                else
                    nPercent = nPercentW;

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                tmp = BitmapPainter.ResizeBitmap(tmp, destWidth, destHeight, true);

                Bitmap result = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppPArgb);
                using (Graphics gr = Graphics.FromImage(result))
                {
                    gr.InterpolationMode = GlobalConfig.InterpolationMode;
                    using (SolidBrush br = new SolidBrush(fillColor))
                    {
                        gr.FillRectangle(br, new Rectangle(0, 0, newWidth, newHeight));
                    }
                    gr.DrawImage(tmp, (int)((newWidth - destWidth) / 2), (int)((newHeight - destHeight) / 2), destWidth, destHeight);
                }

                tmp.Dispose();
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Rotates the image.
        /// </summary>
        /// <param name="bmp">The orifinal image.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="disposeOld">if set to <c>true</c> dispose old image.</param>
        /// <returns></returns>
        public Image RotateImage(Image bmp, float angle, bool disposeOld)
        {
            if (bmp == null)
                return null;

            Bitmap result = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.InterpolationMode = GlobalConfig.InterpolationMode;
                g.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);
                g.DrawImage(bmp, bmp.Width / 2 - bmp.Height / 2, bmp.Height / 2 - bmp.Width / 2, bmp.Height, bmp.Width);
            }
            if (disposeOld)
            {
                bmp.Dispose();
                bmp = null;
            }
            return result;
        }

        public virtual void Startup()
        {
            ReadConfiguration();
            OnLoad(EventArgs.Empty);
            Show(false);
        }

        public virtual void Shutdown()
        {
            SaveConfiguration();
            OnUnload(EventArgs.Empty);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right)
            {
                if (popupMenu != null)
                {
                    popupMenu.PopupAt(ClientToScreenPoint(e.Location));
                }
                else
                    base.OnMouseClick(e);
            }
            else
                base.OnMouseClick(e);
        }


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ToyBase"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public override bool Visible
        {
            get { return base.Visible; }
            set
            {
                if (base.Visible != value)
                {
                    base.Visible = value;
                    if (Alpha == 0)
                        RestoreAlpha();
                    if (this.popupMenu != null)
                        this.popupMenu.CloseUp();
                }
            }
        }

        /// <summary>
        /// Gets or sets the popup menu.
        /// </summary>
        /// <value>The popup menu.</value>
        public KrentoMenu PopupMenu
        {
            get { return popupMenu; }
            set { popupMenu = value; }
        }

        #region IDisposable Members

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (popupMenu != null)
                    {
                        popupMenu.Dispose();
                        popupMenu = null;
                    }
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion

        /// <summary>
        /// Plays the sound from wave file
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void PlaySound(string fileName)
        {
            try
            {
                string soundfile;

                if (string.IsNullOrEmpty(fileName))
                    return;


                if (Path.IsPathRooted(fileName))
                    soundfile = fileName;
                else
                    soundfile = Path.Combine(currentFolder, fileName);

                NativeMethods.MakeSoundFromFile(fileName);
            }
            catch
            {
                //Problem with playing the sound
            }
        }

    }



}

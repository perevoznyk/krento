//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Laugris.Sage
{
    [ToolboxItem(false)]
    public partial class CustomDockPanel : BufferedPanel
    {
        #region Private fields
        private CustomDockManager dockManager;
        private DockSettings settings;
        private CustomDockPainter painter;
        private int updateCount;
        private bool hotTrack;
        private bool keepNativeBitmap;
        #endregion

        #region Events
        public event EventHandler NewSettings;
        public event EventHandler<DockItemEventArgs> SelectItem;
        public event EventHandler<DockItemEventArgs> EnterItem;
        #endregion

        public CustomDockPanel()
        {
            InitializeComponent();
            settings = CreateDockSettings();
            painter = CreateDockPainter();
            dockManager = CreateDockManager();
        }

        public bool KeepNativeBitmap
        {
            get { return keepNativeBitmap; }
            set { keepNativeBitmap = value; }
        }

        protected virtual void OnSelectItem(DockItemEventArgs e)
        {
            if (SelectItem != null)
                SelectItem(this, e);
        }

        private  void DoSelectItem(DockItem item, MouseButtons button)
        {
            DockItemEventArgs e = new DockItemEventArgs(item, button);
            OnSelectItem(e);
        }

        protected virtual void OnEnterItem(DockItemEventArgs e)
        {
            if (EnterItem != null)
                EnterItem(this, e);
        }

        private void DoEnterItem(DockItem item)
        {
            DockItemEventArgs e = new DockItemEventArgs(item, MouseButtons.None);
            OnEnterItem(e);
        }

        private void SettingsChanged(object sender, EventArgs e)
        {
            Invalidate();
            if (NewSettings != null)
                NewSettings(this, e);
        }

        protected virtual CustomDockManager CreateDockManager()
        {
            return null;
        }

        protected virtual CustomDockPainter CreateDockPainter()
        {
            return new DockPainter();
        }

        protected virtual DockSettings CreateDockSettings()
        {
            DockSettings localSettings = new DockSettings();
            localSettings.Changed += new EventHandler(SettingsChanged);
            return localSettings;
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            if (dockManager != null)
                dockManager.Painter.Font = this.Font;
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            if (dockManager != null)
                dockManager.Painter.ForeColor = this.ForeColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (dockManager != null)
            {
                dockManager.Paint(e.Graphics);
            }
        }

        [Browsable(false)]
        public CustomDockManager DockManager
        {
            get { return this.dockManager; }
        }


        public DockSettings Settings
        {
            get { return settings; }
        }

        public CustomDockPainter Painter
        {
            get { return painter; }
        }

        public bool HotTrack
        {
            get { return hotTrack; }
            set { hotTrack = value; }
        }


        public ImageList ImageList
        {
            get { return dockManager.ImageList; }
            set
            {
                if (dockManager != null)
                {
                    dockManager.ImageList = value;
                }
            }
        }


        protected virtual Bitmap ProcessImage(Image image)
        {
            Bitmap result;
            Bitmap originalResult;
            int mirrorSize;
            float denomination = 1.0f;

            if (image == null)
                return null;

            if (dockManager.UseDenomination)
            {
                denomination = (float)((float)image.Width / settings.IconSize);
                result = BitmapPainter.ConvertToRealColors(image, false);
            }
            else
            {
                if ((image.Width != settings.IconSize) || (image.Height != settings.IconSize))
                    result = BitmapPainter.ResizeBitmap(image, settings.IconSize, settings.IconSize);
                else
                    result = BitmapPainter.ConvertToRealColors(image, false);
            }

            if (settings.ReflectionDepth > 0)
            {
                if (dockManager.UseDenomination)
                    mirrorSize = (int)(settings.ReflectionDepth * denomination);
                else
                    mirrorSize = settings.ReflectionDepth;
                originalResult = result;
                result = (Bitmap)BitmapPainter.CreateReflectionImage(result, mirrorSize);
                originalResult.Dispose();
            }

            return result;
        }

        public virtual Bitmap IconFromFileName(string fileName)
        {
            if (dockManager != null)
            {
                Bitmap result = dockManager.IconFromFileName(fileName);
                if (result != null)
                result = ProcessImage(result);
                return result;
            }
            else
                return null;
        }


        public virtual Bitmap IconFromImageList(int index)
        {
            if (dockManager != null)
            {
                Bitmap result = dockManager.IconFromImageList(index);
                if (result != null)
                    result = ProcessImage(result);
                return result;
            }
            else
                return null;

        }

        public virtual void ReloadImages()
        {
            foreach (DockItem item in dockManager.Items)
            {
                if (!string.IsNullOrEmpty(item.ImageName))
                    item.Icon = this.IconFromFileName(item.ImageName);
                else
                    if (item.ImageIndex >= 0)
                        item.Icon = this.IconFromImageList(item.ImageIndex);
                    
            }
        }

        public DockItem AddItem(string caption, string fileName)
        {
            DockItem item;
            if (dockManager != null)
            {
                item = new DockItem();
                item.KeepNativeBitmap = this.KeepNativeBitmap;
                item.ImageName = fileName;
                item.Caption = caption;
                item.Icon = this.IconFromFileName(fileName);
                dockManager.Items.Add(item);
                UpdateItems();
                return item;
            }
            else
                return null;
        }

        public DockItem AddItem(string caption, int imageIndex)
        {
            DockItem item;
            if (dockManager != null)
            {
                item = new DockItem();
                item.KeepNativeBitmap = this.KeepNativeBitmap;
                item.Caption = caption;
                item.ImageIndex = imageIndex;
                item.ImageList = dockManager.ImageList;
                item.Icon = this.IconFromImageList(imageIndex); 
                dockManager.Items.Add(item);
                UpdateItems();
                return item;
            }
            else
                return null;

        }

        public DockItem AddItem(string caption, Image icon)
        {
            DockItem item;
            if (dockManager != null)
            {
                item = new DockItem();
                item.KeepNativeBitmap = this.KeepNativeBitmap;
                item.Caption = caption;
                item.Icon = ProcessImage((Bitmap)icon);
                dockManager.Items.Add(item);
                UpdateItems();
                return item;
            }
            else
                return null;
        }

        public void RemoveItem(int index)
        {
            if (dockManager != null)
            {
                dockManager.Items.RemoveAt(index);
                UpdateItems();
            }
        }

        public void RemoveItem(DockItem item)
        {
            if (dockManager != null)
            {
                dockManager.Items.Remove(item);
                UpdateItems();
            }
        }

        public void Clear()
        {
            if (dockManager != null)
            {
                dockManager.Items.Clear();
                UpdateItems();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseMove"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (dockManager != null)
            {
                if (dockManager.Items.Count > 0)
                {
                    dockManager.DoMouseMove(e.X, e.Y);
                    Invalidate();
                    if (hotTrack)
                    {
                        DockItem item = dockManager.GetItemAt(e.X, e.Y);
                        if (item != null)
                        {
                            DoEnterItem(item);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseLeave"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (dockManager != null)
            {
                dockManager.DoMouseLeave();
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseEnter"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (this.CanFocus)
                this.Focus();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.SizeChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (dockManager != null)
            {
                dockManager.Width = this.Width;
                dockManager.Height = this.Height;
                dockManager.DoSizeChanged();
                Invalidate();
            }
        }

        public void UpdateItems()
        {
            if (updateCount != 0)
                return;

            if (dockManager != null)
            {
                dockManager.DoSizeChanged();
                Invalidate();
            }
        }

        public void BeginUpdate()
        {
            updateCount++;
        }

        public void EndUpdate()
        {
            updateCount--;
            if (updateCount < 0)
                updateCount = 0;
            if (updateCount == 0)
            {
                UpdateItems();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseClick"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (dockManager != null)
            {
                DockItem item = dockManager.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    DoSelectItem(item, e.Button);
                }
            }
        }

    }
}

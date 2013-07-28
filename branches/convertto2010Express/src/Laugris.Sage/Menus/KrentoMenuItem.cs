//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Laugris.Sage
{
    /// <summary>
    /// Popup menu item class
    /// </summary>
    public class KrentoMenuItem : IDisposable, INotifyPropertyChanged, IKrentoMenuItem
    {
        private static KeysConverter keysConverter;

        private KrentoMenuTag tag;
        private string caption;
        private string data;
        private bool enabled = true;
        private string hint;
        private Keys shortCut;
        private string shortCutName;
        private Image image;
        private string name;
        

        public event EventHandler Execute;

        ~KrentoMenuItem()
        {
            Dispose(false);
        }

        protected virtual void OnExecute(EventArgs e)
        {
            if (!enabled)
                return;

            if (Execute != null)
            {
                Execute(this, e);
            }
        }

        internal void DoExecute()
        {
            if (enabled)
                OnExecute(EventArgs.Empty);
        }

        public void Run()
        {
            if (enabled)
                OnExecute(EventArgs.Empty);
        }

        public string ShortCutName
        {
            get { return shortCutName; }
        }

        internal bool EventAssigned
        {
            get
            {
                return (Execute != null);
            }
        }

        public static KeysConverter KeysConverter
        {
            get
            {
                if (keysConverter == null)
                    keysConverter = new KeysConverter();
                return keysConverter;
            }
        }

        public Keys ShortCut
        {
            get { return shortCut; }
            set
            {
                shortCut = value;
                if (shortCut == Keys.None)
                    shortCutName = String.Empty;
                else
                {
                    shortCutName = KeysConverter.ConvertToString(shortCut);
                }
                this.OnPropertyChanged("ShortCut");
            }
        }

        public Image Image
        {
            get { return image; }
            set
            {
                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }
                image = value;
                this.OnPropertyChanged("Image");
            }
        }

        public string Data
        {
            get { return data; }
            set
            {
                data = value;
                this.OnPropertyChanged("Data");
            }
        }

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="KrentoMenuItem"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                this.OnPropertyChanged("Enabled");
            }
        }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get { return caption; }
            set
            {
                caption = value;
                this.OnPropertyChanged("Caption");
            }
        }

        public string Hint
        {
            get { return hint; }
            set
            {
                hint = value;
                this.OnPropertyChanged("Hint");
            }
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public KrentoMenuTag Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                this.OnPropertyChanged("Tag");
            }
        }



        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }
                Execute = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}

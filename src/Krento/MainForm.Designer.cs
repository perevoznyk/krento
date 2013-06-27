using System.Drawing;
using Laugris.Sage;
namespace Krento
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                dropFileHandlers.Clear();

                if (hotKeys != null)
                    try
                    {
                        hotKeys.Dispose();
                    }
                    finally
                    {
                        hotKeys = null;
                    }

                if (ringKeys != null)
                    try
                    {
                        ringKeys.Dispose();
                    }
                    finally
                    {
                        ringKeys = null;
                    }

                if (mouseHook != null)
                    try
                    {
                        mouseHook.Dispose();
                    }
                    finally
                    {
                        mouseHook = null;
                    }
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.timerIntercept = new System.Windows.Forms.Timer(this.components);
            this.notifier = new Laugris.Sage.Notifier(this.components);
            this.SuspendLayout();
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipTitle = "Krento";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "Krento";
            this.notifyIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseMove);
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // timerIntercept
            // 
            this.timerIntercept.Interval = 500;
            this.timerIntercept.Tick += new System.EventHandler(this.timerIntercept_Tick);
            // 
            // notifier
            // 
            this.notifier.AdjustedHeight = 0;
            this.notifier.AdjustedWidth = 0;
            this.notifier.BackColor = System.Drawing.SystemColors.Window;
            this.notifier.BorderDarkColor = System.Drawing.Color.Black;
            this.notifier.BorderLightColor = System.Drawing.Color.LightGray;
            this.notifier.Caption = "Welcome to Krento";
            this.notifier.CaptionColor = System.Drawing.Color.WhiteSmoke;
            this.notifier.FlatBorder = false;
            this.notifier.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notifier.Glyph = null;
            this.notifier.Text = "Welcome to Krento!\r\nPress Win + S keys or click mouse wheel \r\nto display Krento S" +
                "tones Manager";
            this.notifier.TextColor = System.Drawing.Color.Snow;
            this.notifier.Url = "http://users.telenet.be/serhiy.perevoznyk";
            this.notifier.Deactivate += new System.EventHandler(this.notifier_Deactivate);
            // 
            // MainForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 342);
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.ResumeLayout(false);

        }

        #endregion

        private Laugris.Sage.Notifier notifier;
        public System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Timer timerIntercept;
    }
}


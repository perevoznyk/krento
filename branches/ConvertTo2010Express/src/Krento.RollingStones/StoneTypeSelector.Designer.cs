namespace Krento.RollingStones
{
    partial class StoneTypeSelector
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
                components.Dispose();
            }
            if (buttonBg != null)
            {
                buttonBg.Dispose();
                buttonBg = null;
            }

            for (int i = 0; i < icons.Count; i++)
                icons[i].Dispose();
            icons.Clear();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoneTypeSelector));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.StonesSelector = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(216, 267);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 267);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // StonesSelector
            // 
            this.StonesSelector.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.StonesSelector.FormattingEnabled = true;
            this.StonesSelector.ItemHeight = 40;
            this.StonesSelector.Location = new System.Drawing.Point(12, 12);
            this.StonesSelector.Name = "StonesSelector";
            this.StonesSelector.Size = new System.Drawing.Size(360, 244);
            this.StonesSelector.TabIndex = 3;
            this.StonesSelector.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.StonesSelector_DrawItem);
            this.StonesSelector.DoubleClick += new System.EventHandler(this.StonesSelector_DoubleClick);
            // 
            // StoneTypeSelector
            // 
            this.AcceptButton = this.btnOK;
            this.AccessibleRole = System.Windows.Forms.AccessibleRole.Dialog;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(384, 299);
            this.Controls.Add(this.StonesSelector);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StoneTypeSelector";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select stone type";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.StoneTypeSelector_Load);
            this.Shown += new System.EventHandler(this.StoneTypeSelector_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.ListBox StonesSelector;
    }
}
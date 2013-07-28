using System.Drawing;

namespace Krento.RollingStones
{
    partial class RingSettingsDialog
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
            try
            {
                if (imgLogo.Image != null)
                {
                    imgLogo.Image.Dispose();
                    imgLogo.Image = null;
                }

                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Dispose();
                    pictureBox1.Image = null;
                }

                if (this.Icon != null)
                {
                    this.Icon.Dispose();
                    this.Icon = null;
                }

                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                if (ring != null)
                {
                    ring.Dispose();
                    ring = null;
                }

                if (openIconDialog != null)
                {
                    openIconDialog.Dispose();
                    openIconDialog = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RingSettingsDialog));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btlLoad = new System.Windows.Forms.Button();
            this.openIconDialog = new System.Windows.Forms.OpenFileDialog();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.edtDefaultRing = new System.Windows.Forms.CheckBox();
            this.lblName = new System.Windows.Forms.Label();
            this.edtDescription = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.edtName = new System.Windows.Forms.TextBox();
            this.imgLogo = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(396, 212);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(477, 212);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btlLoad
            // 
            this.btlLoad.Location = new System.Drawing.Point(28, 162);
            this.btlLoad.Name = "btlLoad";
            this.btlLoad.Size = new System.Drawing.Size(103, 23);
            this.btlLoad.TabIndex = 0;
            this.btlLoad.Text = "Load";
            this.btlLoad.UseVisualStyleBackColor = true;
            this.btlLoad.Click += new System.EventHandler(this.btlLoad_Click);
            // 
            // openIconDialog
            // 
            this.openIconDialog.Filter = "Image Files (*.PNG;*.BMP;*.JPG;*.GIF;*.ICO)|*.PNG;*.BMP;*.JPG;*.GIF;*.ICO";
            this.openIconDialog.Title = "Select Logo image";
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(5, 201);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(546, 2);
            this.label5.TabIndex = 14;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.edtDefaultRing);
            this.groupBox1.Controls.Add(this.lblName);
            this.groupBox1.Controls.Add(this.edtDescription);
            this.groupBox1.Controls.Add(this.lblDescription);
            this.groupBox1.Controls.Add(this.edtName);
            this.groupBox1.Location = new System.Drawing.Point(161, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(394, 180);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // edtDefaultRing
            // 
            this.edtDefaultRing.AutoSize = true;
            this.edtDefaultRing.Location = new System.Drawing.Point(15, 153);
            this.edtDefaultRing.Name = "edtDefaultRing";
            this.edtDefaultRing.Size = new System.Drawing.Size(80, 17);
            this.edtDefaultRing.TabIndex = 2;
            this.edtDefaultRing.Text = "Default ring";
            this.edtDefaultRing.UseVisualStyleBackColor = true;
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(12, 12);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 9;
            this.lblName.Text = "Name";
            // 
            // edtDescription
            // 
            this.edtDescription.Location = new System.Drawing.Point(15, 67);
            this.edtDescription.Multiline = true;
            this.edtDescription.Name = "edtDescription";
            this.edtDescription.Size = new System.Drawing.Size(363, 80);
            this.edtDescription.TabIndex = 1;
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(12, 51);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 8;
            this.lblDescription.Text = "Description";
            // 
            // edtName
            // 
            this.edtName.Location = new System.Drawing.Point(15, 28);
            this.edtName.Name = "edtName";
            this.edtName.Size = new System.Drawing.Size(363, 20);
            this.edtName.TabIndex = 0;
            this.edtName.TextChanged += new System.EventHandler(this.edtName_TextChanged_1);
            // 
            // imgLogo
            // 
            this.imgLogo.BackColor = System.Drawing.Color.White;
            this.imgLogo.Location = new System.Drawing.Point(31, 32);
            this.imgLogo.Name = "imgLogo";
            this.imgLogo.Size = new System.Drawing.Size(100, 100);
            this.imgLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgLogo.TabIndex = 16;
            this.imgLogo.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(5, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 150);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 17;
            this.pictureBox1.TabStop = false;
            // 
            // RingSettingsDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(564, 247);
            this.Controls.Add(this.imgLogo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btlLoad);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RingSettingsDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.RingSettingsDialog_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btlLoad;
        private System.Windows.Forms.OpenFileDialog openIconDialog;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox edtDescription;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox edtName;
        private System.Windows.Forms.CheckBox edtDefaultRing;
        protected internal System.Windows.Forms.PictureBox imgLogo;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
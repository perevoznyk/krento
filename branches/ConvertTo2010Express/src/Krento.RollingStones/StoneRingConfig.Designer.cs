namespace Krento.RollingStones
{
    partial class StoneRingConfig
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

            if (openIconDialog != null)
            {
                openIconDialog.Dispose();
                openIconDialog = null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoneRingConfig));
            this.imgLogo = new System.Windows.Forms.PictureBox();
            this.btnSelectImage = new System.Windows.Forms.Button();
            this.edtTarget = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.edtDescription = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.openIconDialog = new System.Windows.Forms.OpenFileDialog();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // imgLogo
            // 
            this.imgLogo.BackColor = System.Drawing.Color.White;
            this.imgLogo.Location = new System.Drawing.Point(51, 28);
            this.imgLogo.Name = "imgLogo";
            this.imgLogo.Size = new System.Drawing.Size(100, 100);
            this.imgLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgLogo.TabIndex = 0;
            this.imgLogo.TabStop = false;
            // 
            // btnSelectImage
            // 
            this.btnSelectImage.Location = new System.Drawing.Point(7, 150);
            this.btnSelectImage.Name = "btnSelectImage";
            this.btnSelectImage.Size = new System.Drawing.Size(100, 23);
            this.btnSelectImage.TabIndex = 5;
            this.btnSelectImage.Text = "Select...";
            this.toolTip.SetToolTip(this.btnSelectImage, "Select new icon for this stone");
            this.btnSelectImage.UseVisualStyleBackColor = true;
            this.btnSelectImage.Click += new System.EventHandler(this.btnSelectImage_Click);
            // 
            // edtTarget
            // 
            this.edtTarget.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.edtTarget.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllSystemSources;
            this.edtTarget.Location = new System.Drawing.Point(195, 23);
            this.edtTarget.Name = "edtTarget";
            this.edtTarget.ReadOnly = true;
            this.edtTarget.Size = new System.Drawing.Size(276, 20);
            this.edtTarget.TabIndex = 0;
            this.toolTip.SetToolTip(this.edtTarget, "Select target file name or web site address\r\nYou can use ## as a parameter that w" +
                    "ill be asked\r\nduring the stone execution");
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(477, 20);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.toolTip.SetToolTip(this.btnBrowse, "Browse your computer for the new target");
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(192, 7);
            this.lblTarget.Name = "lblTarget";
            this.lblTarget.Size = new System.Drawing.Size(38, 13);
            this.lblTarget.TabIndex = 4;
            this.lblTarget.Text = "Target";
            this.toolTip.SetToolTip(this.lblTarget, "Select target file name or web site address\r\nYou can use ## as a parameter that w" +
                    "ill be asked\r\nduring the stone execution\r\n");
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(192, 50);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // edtDescription
            // 
            this.edtDescription.Location = new System.Drawing.Point(195, 66);
            this.edtDescription.Name = "edtDescription";
            this.edtDescription.Size = new System.Drawing.Size(357, 20);
            this.edtDescription.TabIndex = 2;
            this.toolTip.SetToolTip(this.edtDescription, "Enter useful description of the target");
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(396, 201);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.toolTip.SetToolTip(this.btnOK, "Save stone configuration");
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(477, 201);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.toolTip.SetToolTip(this.btnCancel, "Discard all your changes");
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(7, 186);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(527, 2);
            this.label5.TabIndex = 13;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(113, 150);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 23);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "Default";
            this.toolTip.SetToolTip(this.btnReset, "Assign default icon for this stone");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // openIconDialog
            // 
            this.openIconDialog.Filter = "Image Files (*.PNG;*.BMP;*.JPG;*.GIF;*.ICO)|*.PNG;*.BMP;*.JPG;*.GIF;*.ICO";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(25, 2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 150);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // StoneRingConfig
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(563, 237);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.edtDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblTarget);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.edtTarget);
            this.Controls.Add(this.btnSelectImage);
            this.Controls.Add(this.imgLogo);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "StoneRingConfig";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.StoneRingConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog openIconDialog;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip toolTip;
        protected internal System.Windows.Forms.TextBox edtTarget;
        protected internal System.Windows.Forms.Button btnBrowse;
        protected internal System.Windows.Forms.Label lblTarget;
        protected internal System.Windows.Forms.PictureBox imgLogo;
        protected internal System.Windows.Forms.Button btnSelectImage;
        protected internal System.Windows.Forms.Label lblDescription;
        protected internal System.Windows.Forms.TextBox edtDescription;
        protected internal System.Windows.Forms.Button btnOK;
        protected internal System.Windows.Forms.Button btnCancel;
        protected internal System.Windows.Forms.Button btnReset;
    }
}
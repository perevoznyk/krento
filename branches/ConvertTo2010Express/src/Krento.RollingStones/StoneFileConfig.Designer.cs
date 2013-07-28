namespace Krento.RollingStones
{
    partial class StoneFileConfig
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

            if (openFileDialog != null)
            {
                openFileDialog.Dispose();
                openFileDialog = null;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StoneFileConfig));
            this.imgLogo = new System.Windows.Forms.PictureBox();
            this.btnSelectImage = new System.Windows.Forms.Button();
            this.edtTarget = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.lblTarget = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.edtDescription = new System.Windows.Forms.TextBox();
            this.lblParameters = new System.Windows.Forms.Label();
            this.edtParameters = new System.Windows.Forms.TextBox();
            this.lblArgument = new System.Windows.Forms.Label();
            this.edtArgument = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.openIconDialog = new System.Windows.Forms.OpenFileDialog();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // imgLogo
            // 
            this.imgLogo.BackColor = System.Drawing.Color.White;
            this.imgLogo.Location = new System.Drawing.Point(63, 26);
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
            this.btnSelectImage.TabIndex = 6;
            this.btnSelectImage.Text = "Select...";
            this.toolTip.SetToolTip(this.btnSelectImage, "Select new icon for this stone");
            this.btnSelectImage.UseVisualStyleBackColor = true;
            this.btnSelectImage.Click += new System.EventHandler(this.btnSelectImage_Click);
            // 
            // edtTarget
            // 
            this.edtTarget.Location = new System.Drawing.Point(232, 24);
            this.edtTarget.Name = "edtTarget";
            this.edtTarget.Size = new System.Drawing.Size(357, 20);
            this.edtTarget.TabIndex = 0;
            this.toolTip.SetToolTip(this.edtTarget, "Select target file name or web site address\r\nYou can use ## as a parameter that w" +
                    "ill be asked\r\nduring the stone execution");
            this.edtTarget.TextChanged += new System.EventHandler(this.edtTarget_TextChanged);
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(232, 50);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(100, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse...";
            this.toolTip.SetToolTip(this.btnBrowse, "Browse your computer for the new target");
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // lblTarget
            // 
            this.lblTarget.AutoSize = true;
            this.lblTarget.Location = new System.Drawing.Point(229, 8);
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
            this.lblDescription.Location = new System.Drawing.Point(229, 92);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(60, 13);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description";
            // 
            // edtDescription
            // 
            this.edtDescription.Location = new System.Drawing.Point(232, 108);
            this.edtDescription.Name = "edtDescription";
            this.edtDescription.Size = new System.Drawing.Size(357, 20);
            this.edtDescription.TabIndex = 3;
            this.toolTip.SetToolTip(this.edtDescription, "Enter useful description of the target");
            // 
            // lblParameters
            // 
            this.lblParameters.AutoSize = true;
            this.lblParameters.Location = new System.Drawing.Point(229, 135);
            this.lblParameters.Name = "lblParameters";
            this.lblParameters.Size = new System.Drawing.Size(128, 13);
            this.lblParameters.TabIndex = 7;
            this.lblParameters.Text = "Command line parameters";
            // 
            // edtParameters
            // 
            this.edtParameters.Location = new System.Drawing.Point(232, 151);
            this.edtParameters.Name = "edtParameters";
            this.edtParameters.Size = new System.Drawing.Size(357, 20);
            this.edtParameters.TabIndex = 4;
            this.toolTip.SetToolTip(this.edtParameters, "Optional command line parameters");
            // 
            // lblArgument
            // 
            this.lblArgument.AutoSize = true;
            this.lblArgument.Location = new System.Drawing.Point(229, 178);
            this.lblArgument.Name = "lblArgument";
            this.lblArgument.Size = new System.Drawing.Size(106, 13);
            this.lblArgument.TabIndex = 9;
            this.lblArgument.Text = "Argument description";
            // 
            // edtArgument
            // 
            this.edtArgument.Location = new System.Drawing.Point(232, 194);
            this.edtArgument.Name = "edtArgument";
            this.edtArgument.Size = new System.Drawing.Size(357, 20);
            this.edtArgument.TabIndex = 5;
            this.toolTip.SetToolTip(this.edtArgument, "If you inserted ## in the target field, you can\r\nprovide here the text of the pro" +
                    "mpt that will be\r\nused during the stone execution");
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(433, 243);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.toolTip.SetToolTip(this.btnOK, "Save stone configuration");
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(514, 243);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.toolTip.SetToolTip(this.btnCancel, "Discard all your changes");
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label5.Location = new System.Drawing.Point(10, 227);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(580, 2);
            this.label5.TabIndex = 13;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(113, 150);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 23);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "Default";
            this.toolTip.SetToolTip(this.btnReset, "Assign default icon for this stone");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "exe";
            this.openFileDialog.Filter = "Applications|*.exe|All files|*.*";
            this.openFileDialog.Title = "Select stone target";
            // 
            // openIconDialog
            // 
            this.openIconDialog.Filter = "Image Files (*.PNG;*.BMP;*.JPG;*.GIF;*.ICO)|*.PNG;*.BMP;*.JPG;*.GIF;*.ICO";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(37, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(150, 150);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            // 
            // btnFolder
            // 
            this.btnFolder.Location = new System.Drawing.Point(338, 50);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(100, 23);
            this.btnFolder.TabIndex = 2;
            this.btnFolder.Text = "Select...";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(232, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(357, 2);
            this.label1.TabIndex = 16;
            // 
            // StoneFileConfig
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(603, 277);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnFolder);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.edtArgument);
            this.Controls.Add(this.lblArgument);
            this.Controls.Add(this.edtParameters);
            this.Controls.Add(this.lblParameters);
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
            this.Name = "StoneFileConfig";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.Load += new System.EventHandler(this.StoneFileConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgLogo;
        private System.Windows.Forms.Button btnSelectImage;
        private System.Windows.Forms.TextBox edtTarget;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Label lblTarget;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.TextBox edtDescription;
        private System.Windows.Forms.Label lblParameters;
        private System.Windows.Forms.TextBox edtParameters;
        private System.Windows.Forms.Label lblArgument;
        private System.Windows.Forms.TextBox edtArgument;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.OpenFileDialog openIconDialog;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}
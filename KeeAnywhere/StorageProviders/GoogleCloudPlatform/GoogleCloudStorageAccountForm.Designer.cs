namespace KeeAnywhere.StorageProviders.GoogleCloudPlatform
{
    partial class GoogleCloudStorageAccountForm
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            this.m_btnOK = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_btnTest = new System.Windows.Forms.Button();
            this.m_lblTestResult = new System.Windows.Forms.Label();
            this.m_dlgOpenKeyFile = new System.Windows.Forms.OpenFileDialog();
            this.m_txtKeyFileLocation = new System.Windows.Forms.TextBox();
            this.m_txtProjectId = new System.Windows.Forms.TextBox();
            this.m_lblAccessKey = new System.Windows.Forms.Label();
            this.m_lblProjectId = new System.Windows.Forms.Label();
            this.m_btnBrowse = new System.Windows.Forms.Button();
            this.m_grpCredentials = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.m_grpCredentials.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(453, 60);
            this.m_bannerImage.TabIndex = 5;
            this.m_bannerImage.TabStop = false;
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnOK.Location = new System.Drawing.Point(285, 206);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(75, 23);
            this.m_btnOK.TabIndex = 7;
            this.m_btnOK.Text = "OK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            this.m_btnOK.Click += new System.EventHandler(this.OnOk);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(366, 206);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 8;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_btnTest
            // 
            this.m_btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnTest.Location = new System.Drawing.Point(12, 206);
            this.m_btnTest.Name = "m_btnTest";
            this.m_btnTest.Size = new System.Drawing.Size(75, 23);
            this.m_btnTest.TabIndex = 6;
            this.m_btnTest.Text = "&Test";
            this.m_btnTest.UseVisualStyleBackColor = true;
            this.m_btnTest.Click += new System.EventHandler(this.OnTest);
            // 
            // m_lblTestResult
            // 
            this.m_lblTestResult.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lblTestResult.Location = new System.Drawing.Point(12, 156);
            this.m_lblTestResult.Name = "m_lblTestResult";
            this.m_lblTestResult.Size = new System.Drawing.Size(429, 45);
            this.m_lblTestResult.TabIndex = 9;
            this.m_lblTestResult.Text = "TestResult";
            this.m_lblTestResult.Visible = false;
            // 
            // m_txtKeyFileLocation
            // 
            this.m_txtKeyFileLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtKeyFileLocation.Location = new System.Drawing.Point(162, 19);
            this.m_txtKeyFileLocation.Name = "m_txtKeyFileLocation";
            this.m_txtKeyFileLocation.Size = new System.Drawing.Size(225, 20);
            this.m_txtKeyFileLocation.TabIndex = 1;
            this.m_txtKeyFileLocation.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_txtProjectId
            // 
            this.m_txtProjectId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtProjectId.Location = new System.Drawing.Point(162, 45);
            this.m_txtProjectId.Name = "m_txtProjectId";
            this.m_txtProjectId.Size = new System.Drawing.Size(225, 20);
            this.m_txtProjectId.TabIndex = 1;
            this.m_txtProjectId.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_lblAccessKey
            // 
            this.m_lblAccessKey.AutoSize = true;
            this.m_lblAccessKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblAccessKey.Location = new System.Drawing.Point(6, 22);
            this.m_lblAccessKey.Name = "m_lblAccessKey";
            this.m_lblAccessKey.Size = new System.Drawing.Size(150, 13);
            this.m_lblAccessKey.TabIndex = 0;
            this.m_lblAccessKey.Text = "Service Account Key File";
            // 
            // m_lblProjectId
            // 
            this.m_lblProjectId.AutoSize = true;
            this.m_lblProjectId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblProjectId.Location = new System.Drawing.Point(6, 48);
            this.m_lblProjectId.Name = "m_lblProjectId";
            this.m_lblProjectId.Size = new System.Drawing.Size(64, 13);
            this.m_lblProjectId.TabIndex = 0;
            this.m_lblProjectId.Text = "Project ID";
            // 
            // m_btnBrowse
            // 
            this.m_btnBrowse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_btnBrowse.Location = new System.Drawing.Point(393, 19);
            this.m_btnBrowse.Name = "m_btnBrowse";
            this.m_btnBrowse.Size = new System.Drawing.Size(30, 20);
            this.m_btnBrowse.TabIndex = 2;
            this.m_btnBrowse.Text = "...";
            this.m_btnBrowse.UseVisualStyleBackColor = true;
            this.m_btnBrowse.Click += new System.EventHandler(this.m_btnBrowse_Click);
            // 
            // m_grpCredentials
            // 
            this.m_grpCredentials.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_grpCredentials.Controls.Add(this.m_btnBrowse);
            this.m_grpCredentials.Controls.Add(this.m_lblProjectId);
            this.m_grpCredentials.Controls.Add(this.m_lblAccessKey);
            this.m_grpCredentials.Controls.Add(this.m_txtProjectId);
            this.m_grpCredentials.Controls.Add(this.m_txtKeyFileLocation);
            this.m_grpCredentials.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_grpCredentials.Location = new System.Drawing.Point(12, 66);
            this.m_grpCredentials.Name = "m_grpCredentials";
            this.m_grpCredentials.Size = new System.Drawing.Size(429, 77);
            this.m_grpCredentials.TabIndex = 19;
            this.m_grpCredentials.TabStop = false;
            this.m_grpCredentials.Text = "Credentials";
            // 
            // GoogleCloudStorageAccountForm
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(453, 241);
            this.Controls.Add(this.m_grpCredentials);
            this.Controls.Add(this.m_lblTestResult);
            this.Controls.Add(this.m_btnTest);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GoogleCloudStorageAccountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Google Cloud Storage Account Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.m_grpCredentials.ResumeLayout(false);
            this.m_grpCredentials.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Button m_btnTest;
        private System.Windows.Forms.Label m_lblTestResult;
        private System.Windows.Forms.OpenFileDialog m_dlgOpenKeyFile;
        private System.Windows.Forms.TextBox m_txtKeyFileLocation;
        private System.Windows.Forms.TextBox m_txtProjectId;
        private System.Windows.Forms.Label m_lblAccessKey;
        private System.Windows.Forms.Label m_lblProjectId;
        private System.Windows.Forms.Button m_btnBrowse;
        private System.Windows.Forms.GroupBox m_grpCredentials;
    }
}
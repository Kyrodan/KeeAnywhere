namespace KeeAnywhere.StorageProviders.AmazonS3
{
    partial class AmazonS3AccountForm
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
            this.m_lblAccessKey = new System.Windows.Forms.Label();
            this.m_txtAccessKey = new System.Windows.Forms.TextBox();
            this.m_lblSecretKey = new System.Windows.Forms.Label();
            this.m_txtSecretKey = new System.Windows.Forms.TextBox();
            this.m_lblRegion = new System.Windows.Forms.Label();
            this.m_cmbRegion = new System.Windows.Forms.ComboBox();
            this.m_btnTest = new System.Windows.Forms.Button();
            this.m_lblTestResult = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(354, 60);
            this.m_bannerImage.TabIndex = 5;
            this.m_bannerImage.TabStop = false;
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnOK.Location = new System.Drawing.Point(186, 170);
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
            this.m_btnCancel.Location = new System.Drawing.Point(267, 170);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 8;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_lblAccessKey
            // 
            this.m_lblAccessKey.AutoSize = true;
            this.m_lblAccessKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblAccessKey.Location = new System.Drawing.Point(12, 69);
            this.m_lblAccessKey.Name = "m_lblAccessKey";
            this.m_lblAccessKey.Size = new System.Drawing.Size(73, 13);
            this.m_lblAccessKey.TabIndex = 0;
            this.m_lblAccessKey.Text = "Access Key";
            // 
            // m_txtAccessKey
            // 
            this.m_txtAccessKey.Location = new System.Drawing.Point(135, 66);
            this.m_txtAccessKey.Name = "m_txtAccessKey";
            this.m_txtAccessKey.Size = new System.Drawing.Size(207, 20);
            this.m_txtAccessKey.TabIndex = 1;
            this.m_txtAccessKey.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_lblSecretKey
            // 
            this.m_lblSecretKey.AutoSize = true;
            this.m_lblSecretKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblSecretKey.Location = new System.Drawing.Point(12, 95);
            this.m_lblSecretKey.Name = "m_lblSecretKey";
            this.m_lblSecretKey.Size = new System.Drawing.Size(69, 13);
            this.m_lblSecretKey.TabIndex = 2;
            this.m_lblSecretKey.Text = "Secret Key";
            // 
            // m_txtSecretKey
            // 
            this.m_txtSecretKey.Location = new System.Drawing.Point(135, 92);
            this.m_txtSecretKey.Name = "m_txtSecretKey";
            this.m_txtSecretKey.Size = new System.Drawing.Size(207, 20);
            this.m_txtSecretKey.TabIndex = 3;
            this.m_txtSecretKey.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_lblRegion
            // 
            this.m_lblRegion.AutoSize = true;
            this.m_lblRegion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblRegion.Location = new System.Drawing.Point(12, 122);
            this.m_lblRegion.Name = "m_lblRegion";
            this.m_lblRegion.Size = new System.Drawing.Size(47, 13);
            this.m_lblRegion.TabIndex = 4;
            this.m_lblRegion.Text = "Region";
            // 
            // m_cmbRegion
            // 
            this.m_cmbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbRegion.FormattingEnabled = true;
            this.m_cmbRegion.Location = new System.Drawing.Point(135, 119);
            this.m_cmbRegion.Name = "m_cmbRegion";
            this.m_cmbRegion.Size = new System.Drawing.Size(207, 21);
            this.m_cmbRegion.TabIndex = 5;
            this.m_cmbRegion.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_btnTest
            // 
            this.m_btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnTest.Location = new System.Drawing.Point(12, 170);
            this.m_btnTest.Name = "m_btnTest";
            this.m_btnTest.Size = new System.Drawing.Size(75, 23);
            this.m_btnTest.TabIndex = 6;
            this.m_btnTest.Text = "&Test";
            this.m_btnTest.UseVisualStyleBackColor = true;
            this.m_btnTest.Click += new System.EventHandler(this.OnTest);
            // 
            // m_lblTestResult
            // 
            this.m_lblTestResult.Location = new System.Drawing.Point(12, 143);
            this.m_lblTestResult.Name = "m_lblTestResult";
            this.m_lblTestResult.Size = new System.Drawing.Size(330, 24);
            this.m_lblTestResult.TabIndex = 9;
            this.m_lblTestResult.Text = "TestResult";
            this.m_lblTestResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_lblTestResult.Visible = false;
            // 
            // AmazonS3AccountForm
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(354, 205);
            this.Controls.Add(this.m_lblTestResult);
            this.Controls.Add(this.m_btnTest);
            this.Controls.Add(this.m_cmbRegion);
            this.Controls.Add(this.m_lblRegion);
            this.Controls.Add(this.m_txtSecretKey);
            this.Controls.Add(this.m_lblSecretKey);
            this.Controls.Add(this.m_txtAccessKey);
            this.Controls.Add(this.m_lblAccessKey);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AmazonS3AccountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Amazon S3 Account Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_lblAccessKey;
        private System.Windows.Forms.TextBox m_txtAccessKey;
        private System.Windows.Forms.Label m_lblSecretKey;
        private System.Windows.Forms.TextBox m_txtSecretKey;
        private System.Windows.Forms.Label m_lblRegion;
        private System.Windows.Forms.ComboBox m_cmbRegion;
        private System.Windows.Forms.Button m_btnTest;
        private System.Windows.Forms.Label m_lblTestResult;
    }
}
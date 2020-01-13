namespace KeeAnywhere.StorageProviders.Azure
{
    partial class AzureAccountForm
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
            this.m_lblAccountName = new System.Windows.Forms.Label();
            this.m_txtAccountName = new System.Windows.Forms.TextBox();
            this.m_lblAccessToken = new System.Windows.Forms.Label();
            this.m_txtAccessToken = new System.Windows.Forms.TextBox();
            this.m_btnTest = new System.Windows.Forms.Button();
            this.m_lblTestResult = new System.Windows.Forms.Label();
            this.m_txtItemName = new System.Windows.Forms.TextBox();
            this.m_lblItemName = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Dock = System.Windows.Forms.DockStyle.Top;
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(944, 143);
            this.m_bannerImage.TabIndex = 5;
            this.m_bannerImage.TabStop = false;
            // 
            // m_btnOK
            // 
            this.m_btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnOK.Location = new System.Drawing.Point(496, 380);
            this.m_btnOK.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.m_btnOK.Name = "m_btnOK";
            this.m_btnOK.Size = new System.Drawing.Size(200, 55);
            this.m_btnOK.TabIndex = 7;
            this.m_btnOK.Text = "OK";
            this.m_btnOK.UseVisualStyleBackColor = true;
            this.m_btnOK.Click += new System.EventHandler(this.OnOk);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(712, 380);
            this.m_btnCancel.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(200, 55);
            this.m_btnCancel.TabIndex = 8;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_lblAccountName
            // 
            this.m_lblAccountName.AutoSize = true;
            this.m_lblAccountName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblAccountName.Location = new System.Drawing.Point(39, 165);
            this.m_lblAccountName.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.m_lblAccountName.Name = "m_lblAccountName";
            this.m_lblAccountName.Size = new System.Drawing.Size(212, 32);
            this.m_lblAccountName.TabIndex = 0;
            this.m_lblAccountName.Text = "Account Name";
            // 
            // m_txtAccountName
            // 
            this.m_txtAccountName.Location = new System.Drawing.Point(367, 157);
            this.m_txtAccountName.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.m_txtAccountName.Name = "m_txtAccountName";
            this.m_txtAccountName.Size = new System.Drawing.Size(545, 38);
            this.m_txtAccountName.TabIndex = 1;
            this.m_txtAccountName.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_lblAccessToken
            // 
            this.m_lblAccessToken.AutoSize = true;
            this.m_lblAccessToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblAccessToken.Location = new System.Drawing.Point(39, 217);
            this.m_lblAccessToken.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.m_lblAccessToken.Name = "m_lblAccessToken";
            this.m_lblAccessToken.Size = new System.Drawing.Size(204, 32);
            this.m_lblAccessToken.TabIndex = 2;
            this.m_lblAccessToken.Text = "Access Token";
            // 
            // m_txtAccessToken
            // 
            this.m_txtAccessToken.Location = new System.Drawing.Point(367, 209);
            this.m_txtAccessToken.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.m_txtAccessToken.Name = "m_txtAccessToken";
            this.m_txtAccessToken.Size = new System.Drawing.Size(545, 38);
            this.m_txtAccessToken.TabIndex = 3;
            this.m_txtAccessToken.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_btnTest
            // 
            this.m_btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnTest.Location = new System.Drawing.Point(32, 380);
            this.m_btnTest.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.m_btnTest.Name = "m_btnTest";
            this.m_btnTest.Size = new System.Drawing.Size(200, 55);
            this.m_btnTest.TabIndex = 6;
            this.m_btnTest.Text = "&Test";
            this.m_btnTest.UseVisualStyleBackColor = true;
            this.m_btnTest.Click += new System.EventHandler(this.OnTest);
            // 
            // m_lblTestResult
            // 
            this.m_lblTestResult.Location = new System.Drawing.Point(32, 306);
            this.m_lblTestResult.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.m_lblTestResult.Name = "m_lblTestResult";
            this.m_lblTestResult.Size = new System.Drawing.Size(880, 57);
            this.m_lblTestResult.TabIndex = 9;
            this.m_lblTestResult.Text = "TestResult";
            this.m_lblTestResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_lblTestResult.Visible = false;
            // 
            // m_txtItemName
            // 
            this.m_txtItemName.Location = new System.Drawing.Point(367, 261);
            this.m_txtItemName.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.m_txtItemName.Name = "m_txtItemName";
            this.m_txtItemName.Size = new System.Drawing.Size(545, 38);
            this.m_txtItemName.TabIndex = 11;
            // 
            // m_lblItemName
            // 
            this.m_lblItemName.AutoSize = true;
            this.m_lblItemName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblItemName.Location = new System.Drawing.Point(39, 269);
            this.m_lblItemName.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.m_lblItemName.Name = "m_lblItemName";
            this.m_lblItemName.Size = new System.Drawing.Size(94, 32);
            this.m_lblItemName.TabIndex = 10;
            this.m_lblItemName.Text = "Name";
            // 
            // AzureAccountForm
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(944, 464);
            this.Controls.Add(this.m_txtItemName);
            this.Controls.Add(this.m_lblItemName);
            this.Controls.Add(this.m_lblTestResult);
            this.Controls.Add(this.m_btnTest);
            this.Controls.Add(this.m_txtAccessToken);
            this.Controls.Add(this.m_lblAccessToken);
            this.Controls.Add(this.m_txtAccountName);
            this.Controls.Add(this.m_lblAccountName);
            this.Controls.Add(this.m_btnOK);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_bannerImage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AzureAccountForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Azure Storage Account Configuration";
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
        private System.Windows.Forms.Label m_lblAccountName;
        private System.Windows.Forms.TextBox m_txtAccountName;
        private System.Windows.Forms.Label m_lblAccessToken;
        private System.Windows.Forms.TextBox m_txtAccessToken;
        private System.Windows.Forms.Button m_btnTest;
        private System.Windows.Forms.Label m_lblTestResult;
        private System.Windows.Forms.TextBox m_txtItemName;
        private System.Windows.Forms.Label m_lblItemName;
    }
}
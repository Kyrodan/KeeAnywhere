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
            this.m_cmbRegion = new System.Windows.Forms.ComboBox();
            this.m_btnTest = new System.Windows.Forms.Button();
            this.m_lblTestResult = new System.Windows.Forms.Label();
            this.m_txtSessionToken = new System.Windows.Forms.TextBox();
            this.m_chkUseSessionToken = new System.Windows.Forms.CheckBox();
            this.m_grpEndpoint = new System.Windows.Forms.GroupBox();
            this.m_authRegion = new System.Windows.Forms.TextBox();
            this.m_lblAuthRegion = new System.Windows.Forms.Label();
            this.m_cmbEndpointUrl = new System.Windows.Forms.ComboBox();
            this.m_rbTypeOther = new System.Windows.Forms.RadioButton();
            this.m_rbTypeAmazon = new System.Windows.Forms.RadioButton();
            this.m_grpCredentials = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.m_grpEndpoint.SuspendLayout();
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
            this.m_btnOK.Location = new System.Drawing.Point(285, 349);
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
            this.m_btnCancel.Location = new System.Drawing.Point(366, 349);
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
            this.m_lblAccessKey.Location = new System.Drawing.Point(6, 22);
            this.m_lblAccessKey.Name = "m_lblAccessKey";
            this.m_lblAccessKey.Size = new System.Drawing.Size(73, 13);
            this.m_lblAccessKey.TabIndex = 0;
            this.m_lblAccessKey.Text = "Access Key";
            // 
            // m_txtAccessKey
            // 
            this.m_txtAccessKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtAccessKey.Location = new System.Drawing.Point(158, 19);
            this.m_txtAccessKey.Name = "m_txtAccessKey";
            this.m_txtAccessKey.Size = new System.Drawing.Size(265, 20);
            this.m_txtAccessKey.TabIndex = 1;
            this.m_txtAccessKey.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_lblSecretKey
            // 
            this.m_lblSecretKey.AutoSize = true;
            this.m_lblSecretKey.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblSecretKey.Location = new System.Drawing.Point(6, 48);
            this.m_lblSecretKey.Name = "m_lblSecretKey";
            this.m_lblSecretKey.Size = new System.Drawing.Size(69, 13);
            this.m_lblSecretKey.TabIndex = 2;
            this.m_lblSecretKey.Text = "Secret Key";
            // 
            // m_txtSecretKey
            // 
            this.m_txtSecretKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtSecretKey.Location = new System.Drawing.Point(158, 45);
            this.m_txtSecretKey.Name = "m_txtSecretKey";
            this.m_txtSecretKey.Size = new System.Drawing.Size(265, 20);
            this.m_txtSecretKey.TabIndex = 3;
            this.m_txtSecretKey.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_cmbRegion
            // 
            this.m_cmbRegion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmbRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cmbRegion.FormattingEnabled = true;
            this.m_cmbRegion.Location = new System.Drawing.Point(158, 18);
            this.m_cmbRegion.Name = "m_cmbRegion";
            this.m_cmbRegion.Size = new System.Drawing.Size(265, 21);
            this.m_cmbRegion.TabIndex = 5;
            this.m_cmbRegion.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_btnTest
            // 
            this.m_btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_btnTest.Location = new System.Drawing.Point(12, 349);
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
            this.m_lblTestResult.Location = new System.Drawing.Point(12, 299);
            this.m_lblTestResult.Name = "m_lblTestResult";
            this.m_lblTestResult.Size = new System.Drawing.Size(429, 45);
            this.m_lblTestResult.TabIndex = 9;
            this.m_lblTestResult.Text = "TestResult";
            this.m_lblTestResult.Visible = false;
            // 
            // m_txtSessionToken
            // 
            this.m_txtSessionToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtSessionToken.Enabled = false;
            this.m_txtSessionToken.Location = new System.Drawing.Point(158, 71);
            this.m_txtSessionToken.Name = "m_txtSessionToken";
            this.m_txtSessionToken.Size = new System.Drawing.Size(265, 20);
            this.m_txtSessionToken.TabIndex = 12;
            this.m_txtSessionToken.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_chkUseSessionToken
            // 
            this.m_chkUseSessionToken.AutoSize = true;
            this.m_chkUseSessionToken.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_chkUseSessionToken.Location = new System.Drawing.Point(5, 74);
            this.m_chkUseSessionToken.Margin = new System.Windows.Forms.Padding(2);
            this.m_chkUseSessionToken.Name = "m_chkUseSessionToken";
            this.m_chkUseSessionToken.Size = new System.Drawing.Size(136, 17);
            this.m_chkUseSessionToken.TabIndex = 14;
            this.m_chkUseSessionToken.Text = "Use Session Token";
            this.m_chkUseSessionToken.UseVisualStyleBackColor = false;
            this.m_chkUseSessionToken.CheckedChanged += new System.EventHandler(this.OnUseSessionTokenChanged);
            // 
            // m_grpEndpoint
            // 
            this.m_grpEndpoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_grpEndpoint.Controls.Add(this.m_authRegion);
            this.m_grpEndpoint.Controls.Add(this.m_lblAuthRegion);
            this.m_grpEndpoint.Controls.Add(this.m_cmbEndpointUrl);
            this.m_grpEndpoint.Controls.Add(this.m_rbTypeOther);
            this.m_grpEndpoint.Controls.Add(this.m_rbTypeAmazon);
            this.m_grpEndpoint.Controls.Add(this.m_cmbRegion);
            this.m_grpEndpoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_grpEndpoint.Location = new System.Drawing.Point(12, 172);
            this.m_grpEndpoint.Name = "m_grpEndpoint";
            this.m_grpEndpoint.Size = new System.Drawing.Size(429, 124);
            this.m_grpEndpoint.TabIndex = 18;
            this.m_grpEndpoint.TabStop = false;
            this.m_grpEndpoint.Text = "Endpoint";
            // 
            // m_authRegion
            // 
            this.m_authRegion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_authRegion.Enabled = false;
            this.m_authRegion.Location = new System.Drawing.Point(158, 72);
            this.m_authRegion.Name = "m_authRegion";
            this.m_authRegion.Size = new System.Drawing.Size(265, 20);
            this.m_authRegion.TabIndex = 15;
            this.m_authRegion.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_lblAuthRegion
            // 
            this.m_lblAuthRegion.AutoSize = true;
            this.m_lblAuthRegion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_lblAuthRegion.Location = new System.Drawing.Point(23, 75);
            this.m_lblAuthRegion.Name = "m_lblAuthRegion";
            this.m_lblAuthRegion.Size = new System.Drawing.Size(129, 26);
            this.m_lblAuthRegion.TabIndex = 15;
            this.m_lblAuthRegion.Text = "Override Auth Region\r\n(optional)";
            // 
            // m_cmbEndpointUrl
            // 
            this.m_cmbEndpointUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cmbEndpointUrl.FormattingEnabled = true;
            this.m_cmbEndpointUrl.Items.AddRange(new object[] {
            "https://s3.yourregion.backblazeb2.com",
            "https://s3-de-central.profitbricks.com",
            "https://s3.hidrive.strato.com",
            "https://s3.us-east-1.wasabisys.com",
            "https://s3.us-east-2.wasabisys.com",
            "https://s3.us-central-1.wasabisys.com",
            "https://s3.us-west-1.wasabisys.com",
            "https://s3.eu-central-1.wasabisys.com",
            "https://s3.ap-northeast-1.wasabisys.com"});
            this.m_cmbEndpointUrl.Location = new System.Drawing.Point(158, 45);
            this.m_cmbEndpointUrl.Name = "m_cmbEndpointUrl";
            this.m_cmbEndpointUrl.Size = new System.Drawing.Size(265, 21);
            this.m_cmbEndpointUrl.TabIndex = 6;
            this.m_cmbEndpointUrl.TextChanged += new System.EventHandler(this.OnTextChanged);
            // 
            // m_rbTypeOther
            // 
            this.m_rbTypeOther.AutoSize = true;
            this.m_rbTypeOther.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_rbTypeOther.Location = new System.Drawing.Point(9, 46);
            this.m_rbTypeOther.Name = "m_rbTypeOther";
            this.m_rbTypeOther.Size = new System.Drawing.Size(85, 17);
            this.m_rbTypeOther.TabIndex = 1;
            this.m_rbTypeOther.TabStop = true;
            this.m_rbTypeOther.Text = "Other URL";
            this.m_rbTypeOther.UseVisualStyleBackColor = true;
            this.m_rbTypeOther.CheckedChanged += new System.EventHandler(this.OnTypeChanged);
            // 
            // m_rbTypeAmazon
            // 
            this.m_rbTypeAmazon.AutoSize = true;
            this.m_rbTypeAmazon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_rbTypeAmazon.Location = new System.Drawing.Point(9, 18);
            this.m_rbTypeAmazon.Name = "m_rbTypeAmazon";
            this.m_rbTypeAmazon.Size = new System.Drawing.Size(113, 17);
            this.m_rbTypeAmazon.TabIndex = 0;
            this.m_rbTypeAmazon.TabStop = true;
            this.m_rbTypeAmazon.Text = "Amazon Region";
            this.m_rbTypeAmazon.UseVisualStyleBackColor = true;
            this.m_rbTypeAmazon.CheckedChanged += new System.EventHandler(this.OnTypeChanged);
            // 
            // m_grpCredentials
            // 
            this.m_grpCredentials.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_grpCredentials.Controls.Add(this.m_lblAccessKey);
            this.m_grpCredentials.Controls.Add(this.m_txtAccessKey);
            this.m_grpCredentials.Controls.Add(this.m_chkUseSessionToken);
            this.m_grpCredentials.Controls.Add(this.m_lblSecretKey);
            this.m_grpCredentials.Controls.Add(this.m_txtSessionToken);
            this.m_grpCredentials.Controls.Add(this.m_txtSecretKey);
            this.m_grpCredentials.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_grpCredentials.Location = new System.Drawing.Point(12, 66);
            this.m_grpCredentials.Name = "m_grpCredentials";
            this.m_grpCredentials.Size = new System.Drawing.Size(429, 100);
            this.m_grpCredentials.TabIndex = 19;
            this.m_grpCredentials.TabStop = false;
            this.m_grpCredentials.Text = "Credentials";
            // 
            // AmazonS3AccountForm
            // 
            this.AcceptButton = this.m_btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(453, 384);
            this.Controls.Add(this.m_grpCredentials);
            this.Controls.Add(this.m_grpEndpoint);
            this.Controls.Add(this.m_lblTestResult);
            this.Controls.Add(this.m_btnTest);
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
            this.m_grpEndpoint.ResumeLayout(false);
            this.m_grpEndpoint.PerformLayout();
            this.m_grpCredentials.ResumeLayout(false);
            this.m_grpCredentials.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.Button m_btnOK;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_lblAccessKey;
        private System.Windows.Forms.TextBox m_txtAccessKey;
        private System.Windows.Forms.Label m_lblSecretKey;
        private System.Windows.Forms.TextBox m_txtSecretKey;
        private System.Windows.Forms.ComboBox m_cmbRegion;
        private System.Windows.Forms.Button m_btnTest;
        private System.Windows.Forms.Label m_lblTestResult;
        private System.Windows.Forms.TextBox m_txtSessionToken;
        private System.Windows.Forms.CheckBox m_chkUseSessionToken;
        private System.Windows.Forms.GroupBox m_grpEndpoint;
        private System.Windows.Forms.RadioButton m_rbTypeOther;
        private System.Windows.Forms.RadioButton m_rbTypeAmazon;
        private System.Windows.Forms.ComboBox m_cmbEndpointUrl;
        private System.Windows.Forms.GroupBox m_grpCredentials;
        private System.Windows.Forms.TextBox m_authRegion;
        private System.Windows.Forms.Label m_lblAuthRegion;
    }
}
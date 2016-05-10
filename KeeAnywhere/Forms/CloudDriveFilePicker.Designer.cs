namespace KeeAnywhere.Forms
{
    partial class CloudDriveFilePicker
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
            this.components = new System.ComponentModel.Container();
            this.m_btnOk = new System.Windows.Forms.Button();
            this.m_btnCancel = new System.Windows.Forms.Button();
            this.m_lblAccount = new System.Windows.Forms.Label();
            this.m_lvDetails = new System.Windows.Forms.ListView();
            this.m_ilFiletypeIcons = new System.Windows.Forms.ImageList(this.components);
            this.m_lblFilename = new System.Windows.Forms.Label();
            this.m_txtFilename = new System.Windows.Forms.TextBox();
            this.m_cbFilter = new System.Windows.Forms.ComboBox();
            this.m_bannerImage = new System.Windows.Forms.PictureBox();
            this.m_txtUrl = new System.Windows.Forms.TextBox();
            this.m_lblUrl = new System.Windows.Forms.Label();
            this.m_ilProviderIcons = new System.Windows.Forms.ImageList(this.components);
            this.m_cbAccounts = new KeeAnywhere.Forms.ImagedComboBox.ImageComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // m_btnOk
            // 
            this.m_btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_btnOk.Location = new System.Drawing.Point(420, 521);
            this.m_btnOk.Name = "m_btnOk";
            this.m_btnOk.Size = new System.Drawing.Size(75, 23);
            this.m_btnOk.TabIndex = 8;
            this.m_btnOk.Text = "OK";
            this.m_btnOk.UseVisualStyleBackColor = true;
            this.m_btnOk.Click += new System.EventHandler(this.OnOkClick);
            // 
            // m_btnCancel
            // 
            this.m_btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.m_btnCancel.Location = new System.Drawing.Point(501, 521);
            this.m_btnCancel.Name = "m_btnCancel";
            this.m_btnCancel.Size = new System.Drawing.Size(75, 23);
            this.m_btnCancel.TabIndex = 9;
            this.m_btnCancel.Text = "Cancel";
            this.m_btnCancel.UseVisualStyleBackColor = true;
            // 
            // m_lblAccount
            // 
            this.m_lblAccount.AutoSize = true;
            this.m_lblAccount.Location = new System.Drawing.Point(12, 69);
            this.m_lblAccount.Name = "m_lblAccount";
            this.m_lblAccount.Size = new System.Drawing.Size(50, 13);
            this.m_lblAccount.TabIndex = 10;
            this.m_lblAccount.Text = "Account:";
            // 
            // m_lvDetails
            // 
            this.m_lvDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_lvDetails.FullRowSelect = true;
            this.m_lvDetails.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.m_lvDetails.Location = new System.Drawing.Point(15, 93);
            this.m_lvDetails.MultiSelect = false;
            this.m_lvDetails.Name = "m_lvDetails";
            this.m_lvDetails.Size = new System.Drawing.Size(561, 383);
            this.m_lvDetails.SmallImageList = this.m_ilFiletypeIcons;
            this.m_lvDetails.TabIndex = 12;
            this.m_lvDetails.UseCompatibleStateImageBehavior = false;
            this.m_lvDetails.View = System.Windows.Forms.View.Details;
            this.m_lvDetails.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.OnItemSelectionChanged);
            this.m_lvDetails.DoubleClick += new System.EventHandler(this.OnItemDoubleClick);
            // 
            // m_ilFiletypeIcons
            // 
            this.m_ilFiletypeIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.m_ilFiletypeIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.m_ilFiletypeIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // m_lblFilename
            // 
            this.m_lblFilename.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_lblFilename.AutoSize = true;
            this.m_lblFilename.Location = new System.Drawing.Point(12, 485);
            this.m_lblFilename.Name = "m_lblFilename";
            this.m_lblFilename.Size = new System.Drawing.Size(52, 13);
            this.m_lblFilename.TabIndex = 13;
            this.m_lblFilename.Text = "Filename:";
            // 
            // m_txtFilename
            // 
            this.m_txtFilename.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtFilename.Location = new System.Drawing.Point(68, 482);
            this.m_txtFilename.Name = "m_txtFilename";
            this.m_txtFilename.Size = new System.Drawing.Size(346, 20);
            this.m_txtFilename.TabIndex = 14;
            this.m_txtFilename.TextChanged += new System.EventHandler(this.OnFilenameChanged);
            // 
            // m_cbFilter
            // 
            this.m_cbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_cbFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cbFilter.FormattingEnabled = true;
            this.m_cbFilter.Location = new System.Drawing.Point(420, 482);
            this.m_cbFilter.Name = "m_cbFilter";
            this.m_cbFilter.Size = new System.Drawing.Size(156, 21);
            this.m_cbFilter.TabIndex = 15;
            this.m_cbFilter.SelectedIndexChanged += new System.EventHandler(this.OnFilterChanged);
            // 
            // m_bannerImage
            // 
            this.m_bannerImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_bannerImage.Location = new System.Drawing.Point(0, 0);
            this.m_bannerImage.Name = "m_bannerImage";
            this.m_bannerImage.Size = new System.Drawing.Size(588, 60);
            this.m_bannerImage.TabIndex = 16;
            this.m_bannerImage.TabStop = false;
            // 
            // m_txtUrl
            // 
            this.m_txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_txtUrl.Location = new System.Drawing.Point(68, 521);
            this.m_txtUrl.Name = "m_txtUrl";
            this.m_txtUrl.ReadOnly = true;
            this.m_txtUrl.Size = new System.Drawing.Size(346, 20);
            this.m_txtUrl.TabIndex = 17;
            // 
            // m_lblUrl
            // 
            this.m_lblUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_lblUrl.AutoSize = true;
            this.m_lblUrl.Location = new System.Drawing.Point(12, 524);
            this.m_lblUrl.Name = "m_lblUrl";
            this.m_lblUrl.Size = new System.Drawing.Size(32, 13);
            this.m_lblUrl.TabIndex = 18;
            this.m_lblUrl.Text = "URL:";
            // 
            // m_ilProviderIcons
            // 
            this.m_ilProviderIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.m_ilProviderIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.m_ilProviderIcons.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // m_cbAccounts
            // 
            this.m_cbAccounts.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.m_cbAccounts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.m_cbAccounts.FormattingEnabled = true;
            this.m_cbAccounts.ImageList = this.m_ilProviderIcons;
            this.m_cbAccounts.Indent = 20;
            this.m_cbAccounts.ItemHeight = 16;
            this.m_cbAccounts.Location = new System.Drawing.Point(68, 66);
            this.m_cbAccounts.Name = "m_cbAccounts";
            this.m_cbAccounts.Size = new System.Drawing.Size(261, 22);
            this.m_cbAccounts.TabIndex = 19;
            this.m_cbAccounts.SelectedValueChanged += new System.EventHandler(this.OnAccountChanged);
            // 
            // CloudDriveFilePicker
            // 
            this.AcceptButton = this.m_btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.m_btnCancel;
            this.ClientSize = new System.Drawing.Size(588, 556);
            this.Controls.Add(this.m_cbAccounts);
            this.Controls.Add(this.m_lblUrl);
            this.Controls.Add(this.m_txtUrl);
            this.Controls.Add(this.m_cbFilter);
            this.Controls.Add(this.m_txtFilename);
            this.Controls.Add(this.m_lblFilename);
            this.Controls.Add(this.m_lvDetails);
            this.Controls.Add(this.m_lblAccount);
            this.Controls.Add(this.m_btnOk);
            this.Controls.Add(this.m_btnCancel);
            this.Controls.Add(this.m_bannerImage);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CloudDriveFilePicker";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cloud Drive Picker";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnFormClosed);
            this.Load += new System.EventHandler(this.OnFormLoad);
            this.Resize += new System.EventHandler(this.OnFormResize);
            ((System.ComponentModel.ISupportInitialize)(this.m_bannerImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button m_btnOk;
        private System.Windows.Forms.Button m_btnCancel;
        private System.Windows.Forms.Label m_lblAccount;
        private System.Windows.Forms.ListView m_lvDetails;
        private System.Windows.Forms.Label m_lblFilename;
        private System.Windows.Forms.TextBox m_txtFilename;
        private System.Windows.Forms.ComboBox m_cbFilter;
        private System.Windows.Forms.PictureBox m_bannerImage;
        private System.Windows.Forms.ImageList m_ilFiletypeIcons;
        private System.Windows.Forms.TextBox m_txtUrl;
        private System.Windows.Forms.Label m_lblUrl;
        private ImagedComboBox.ImageComboBox m_cbAccounts;
        private System.Windows.Forms.ImageList m_ilProviderIcons;
    }
}
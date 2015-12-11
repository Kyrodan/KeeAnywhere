using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeeAnywhere.Configuration;
using KeeAnywhere.StorageProviders;
using KeePass.UI;
using KeePassLib.Utility;

namespace KeeAnywhere.Forms
{
    public partial class CloudDriveFilePicker : Form
    {
        public enum Mode
        {
            Unknown,
            Open,
            Save,
        }

        private IStorageProvider m_provider;
        private readonly Dictionary<StorageProviderItem, ItemInfo> m_cache = new Dictionary<StorageProviderItem, ItemInfo>();
        private readonly Stack<StorageProviderItem> m_stack = new Stack<StorageProviderItem>();
        private ConfigurationService m_configService;
        private bool m_isInit;
        private StorageService m_storageService;
        private Cursor m_savedCursor;
        private StorageProviderItem m_selectedItem;
        private Mode m_mode;

        public CloudDriveFilePicker()
        {
            InitializeComponent();
        }

        public void InitEx(ConfigurationService configService, StorageService storageService, Mode mode)
        {
            if (configService == null) throw new ArgumentNullException("configService");
            if (storageService == null) throw new ArgumentNullException("storageService");
            if (mode == Mode.Unknown) throw new ArgumentException("mode");

            m_configService = configService;
            m_storageService = storageService;
            m_mode = mode;
        }

        public string ResultUri
        {
            get {
                return GetResultUri();
            }
        }

        private string GetResultUri()
        {
            var path = GetPath();
            var account = ((AccountConfiguration) m_cbAccount.SelectedItem);

            var s = StorageUri.BuildUriString(account.Type, account.Name, path);

            return s;
        }

        private string GetPath()
        {
            var a = m_stack.Select(_ => _.Name).Reverse().ToList();

            var filename = m_txtFilename.Text;
            if (!string.IsNullOrEmpty(filename))
                a.Add(filename);

            var path = string.Join("/", a);
            return path;
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            GlobalWindowManager.AddWindow(this);

            Icon = PluginResources.Icon_OneDrive_16x16;

            UpdateBanner();

            m_isInit = true;

            m_cbAccount.BeginUpdate();
            m_cbAccount.DataSource = m_configService.Accounts;
            m_cbAccount.DisplayMember = "DisplayName";
            m_cbAccount.ValueMember = "Id";
            m_cbAccount.EndUpdate();

            m_ilIcons.Images.Add(PluginResources.Folder_16x16);

            m_cbFilter.Items.Add("KeePass KDBX Files (*.kdbx)");
            m_cbFilter.Items.Add("All Files (*.*)");
            m_cbFilter.SelectedIndex = 0;

            m_lvDetails.Columns.Add("Name");
            m_lvDetails.Columns.Add("Id");
            m_lvDetails.Columns.Add("Type");
            m_lvDetails.Columns.Add("Changed Date");

            UIUtil.ResizeColumns(m_lvDetails, new int[] {
				4, 1, 1, 1 }, true);

            m_isInit = false;

            if (m_cbAccount.Items.Count > 0)
            {
                m_cbAccount.SelectedIndex = -1;
                m_cbAccount.SelectedIndex = 0;
            }
        }

        private void UpdateBanner()
        {
            switch (m_mode)
            {
                case Mode.Open:
                    this.Text = "Open from Cloud Drive";
                    BannerFactory.CreateBannerEx(this, m_bannerImage,
                        PluginResources.OneDrive_48x48, "Open from Cloud Drive",
                        "Here you can pick your database to open from a Cloud Drive.");
                    break;
                case Mode.Save:
                    this.Text = "Save to cloud drive";
                    BannerFactory.CreateBannerEx(this, m_bannerImage,
                        PluginResources.OneDrive_48x48, "Save to Cloud Drive",
                        "Here you can pick a location to save to a Cloud Drive.");
                    break;
                //default:
                //    throw new NotImplementedException();
            }

        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            GlobalWindowManager.RemoveWindow(this);
        }

        private async void OnAccountChanged(object sender, EventArgs e)
        {
            if (m_isInit) return;

            var account = m_cbAccount.SelectedItem as AccountConfiguration;
            if (account == null) return;

            SetWaitState(true);
            var provider = m_storageService.GetProviderByAccount(account);
            await SetProvider(provider);

            SetWaitState(false);
        }

        private void SetWaitState(bool isWait)
        {
            if (isWait && m_savedCursor != null) return;

            m_cbAccount.Enabled = !isWait;
            m_lvDetails.Enabled = !isWait;
            m_btnOpen.Enabled = !isWait;

            if (isWait)
            {
                m_savedCursor = Cursor;
                Cursor = Cursors.WaitCursor;
            }
            else
            {
                Cursor = m_savedCursor;
                m_savedCursor = null;
            }
        }

        private async Task SetSelectedItem(StorageProviderItem item)
        {
            if (m_selectedItem == item || item.Type != StorageProviderItemType.Folder) return;

            m_selectedItem = item;

            await UpdateListView();
            m_txtUrl.Text = GetResultUri();
        }

        private async Task UpdateListView()
        {
            if (m_selectedItem == null) return;

            m_lvDetails.BeginUpdate();
            m_lvDetails.Items.Clear();

            var info = await GetItemInfo(m_selectedItem);

            if (info.Parent != null)
            {
                var lvi = m_lvDetails.Items.Add("..");
                lvi.Tag = info.Parent;
                lvi.ImageIndex = 0;
                lvi.SubItems.Add(info.Parent.Id);
                lvi.SubItems.Add("Folder");
                lvi.SubItems.Add(string.Empty);
            }

            foreach (var child in info.Children)
            {
                var ext = Path.GetExtension(child.Name);
                if (m_cbFilter.SelectedIndex == 0 && child.Type == StorageProviderItemType.File && !string.IsNullOrEmpty(ext) && ext.ToLower() != ".kdbx")
                    continue;

                var lvi = m_lvDetails.Items.Add(child.Name);
                lvi.Tag = child;

                lvi.SubItems.Add(child.Id);

                switch (child.Type)
                {
                    case StorageProviderItemType.Folder:
                        lvi.ImageIndex = 0;
                        lvi.SubItems.Add("Folder");
                        break;
                    case StorageProviderItemType.File:
                        lvi.ImageIndex = GetIconIndex(child.Name);
                        lvi.SubItems.Add("File");
                        break;
                    default:
                        lvi.SubItems.Add("Unknown");
                        break;
                }

                lvi.SubItems.Add(child.LastModifiedDateTime.ToString());
            }

            m_lvDetails.EndUpdate();
        }

        private int GetIconIndex(string filename)
        {
            var extension = Path.GetExtension(filename);

            if (!m_ilIcons.Images.ContainsKey(extension))
            {
                var image = IconHelper.IconFromExtension(extension, IconHelper.SystemIconSize.Small);
                if (image == null) return 0;
                
                m_ilIcons.Images.Add(extension, image);
            }

            return m_ilIcons.Images.IndexOfKey(extension);
        }

        private async Task SetProvider(IStorageProvider provider)
        {
            if (m_provider == provider) return;

            m_provider = provider;
            m_cache.Clear();
            m_stack.Clear();

            if (m_provider == null)
            {
                await SetSelectedItem(null);
            }
            else
            {
                var root = await m_provider.GetRootItem();
                await SetSelectedItem(root);
            }
        }

        private async void OnItemDoubleClick(object sender, EventArgs e)
        {
            if (m_lvDetails.FocusedItem == null) return;

            var item = m_lvDetails.FocusedItem.Tag as StorageProviderItem;
            if (item == null) return;

            switch (item.Type)
            {
                case StorageProviderItemType.Folder:
                    SetWaitState(true);

                    if (m_lvDetails.FocusedItem.Text == @"..")
                        m_stack.Pop();
                    else
                        m_stack.Push(item);

                    await SetSelectedItem(item);
                    SetWaitState(false);
                    break;
                case StorageProviderItemType.File:
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
            }
        }

        private async Task<ItemInfo> GetItemInfo(StorageProviderItem item)
        {
            if (m_cache.ContainsKey(item))
                return m_cache[item];

            var info = new ItemInfo();
            var result = await m_provider.GetChildrenByParentItem(item);
            info.Children = result.OrderByDescending(_ => _.Type).ThenBy(_ => _.Name).ToArray();

            if (item.ParentReferenceId != null)
            {
                var parent = m_cache.Keys.SingleOrDefault(_ => _.Id == item.ParentReferenceId);
                if (parent != null)
                    info.Parent = parent;
                else
                    throw new NotImplementedException();
                //await m_provider.GetItem(item.ParentReference.Id);
            }

            m_cache.Add(item, info);

            return info;
        }

        private void OnFormResize(object sender, EventArgs e)
        {
            UpdateBanner();
        }

        private class ItemInfo
        {
            public IEnumerable<StorageProviderItem> Children;
            public StorageProviderItem Parent;
        }

        private async void OnOpenClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.None;
            if (string.IsNullOrEmpty(m_txtFilename.Text)) return;

            var filename = m_txtFilename.Text;
            if (string.IsNullOrEmpty(filename))
                return;

            var itemInfo = await GetItemInfo(m_selectedItem);
            var subItem = itemInfo.Children.SingleOrDefault(_ => _.Name == filename);

            switch (m_mode)
            {
                case Mode.Open:
                    if (subItem == null)
                        MessageService.ShowWarning("File/Folder does not exist.");
                    else 
                    {
                        switch (subItem.Type)
                        {
                            case StorageProviderItemType.File:
                                DialogResult = DialogResult.OK;
                                break;

                            case StorageProviderItemType.Folder:
                                SetWaitState(true);

                                m_txtFilename.Text = null;
                                m_stack.Push(subItem);
                                await SetSelectedItem(subItem);

                                SetWaitState(false);
                                break;
                        }
                    }

                    break;

                case Mode.Save:
                    if (subItem == null)
                    {
                        DialogResult = DialogResult.OK;
                    }
                    else 
                    {
                        switch (subItem.Type)
                        {
                            case StorageProviderItemType.File:
                                var result = MessageService.AskYesNo("The file already exists.\nWould you like to override?",
                                    "Overwrite file");

                                if (result)
                                    DialogResult = DialogResult.OK;
                                break;
                            case StorageProviderItemType.Folder:
                                SetWaitState(true);

                                m_txtFilename.Text = null;
                                m_stack.Push(subItem);
                                await SetSelectedItem(subItem);

                                SetWaitState(false);
                                break;
                        }
                    }

                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private void OnItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.Item == null) return;

            var item = e.Item.Tag as StorageProviderItem;

            if (item != null && item.Type == StorageProviderItemType.File)
                m_txtFilename.Text = item.Name;

            m_txtUrl.Text = GetResultUri();

        }

        private async void OnFilterChanged(object sender, EventArgs e)
        {
            await UpdateListView();
        }

        private void OnFilenameChanged(object sender, EventArgs e)
        {
            m_txtUrl.Text = GetResultUri();
        }
    }
}
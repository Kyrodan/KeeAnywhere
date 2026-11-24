namespace KeeAnywhere
{
    /// <summary>
    ///     OAuth client ids/secrets of the storage providers.
    /// </summary>
    /// <remarks>
    ///     ApiKeys.cs is NOT under version control (.gitignore). It is created from
    ///     ApiKeys.Template.cs on first build (dummy values). For release builds put the
    ///     real keys into ApiKeys.cs - never into ApiKeys.Template.cs.
    /// </remarks>
    internal static class ApiKeys
    {
        // Box
        internal const string BoxClientId = "dummy";
        internal const string BoxClientSecret = "dummy";

        // Dropbox
        internal const string DropboxFullAccessClientId = "dummy";
        internal const string DropboxFullAccessClientSecret = "dummy";
        internal const string DropboxAppFolderOnlyClientId = "dummy";
        internal const string DropboxAppFolderOnlyClientSecret = "dummy";

        // Google Drive
        internal const string GoogleDriveClientId = "dummy";
        internal const string GoogleDriveClientSecret = "dummy";

        // HiDrive
        internal const string HiDriveClientId = "dummy";
        internal const string HiDriveClientSecret = "dummy";

        // OneDrive
        internal const string OneDriveClientId = "dummy";
    }
}

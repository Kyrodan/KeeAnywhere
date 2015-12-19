namespace KeeAnywhere.Configuration
{
    /// <summary>
    /// Defines the possible storage locations for the account credentials
    /// </summary>
    public enum AccountStorageLocation : short
    {
        /// <summary>
        /// Saves the accounts plain text in KeePass.config.xml (located in %APPDATA%\KeePassDatabase)
        /// </summary>
        KeePassConfig = 0,

        /// <summary>
        /// Saves the accounts encrypted in the Windows Credential Manager
        /// </summary>
        WindowsCredentialManager = 1,
    }
}
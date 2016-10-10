using System;

namespace KeeAnywhere.Offline
{
    public interface ICacheSupervisor
    {
        string CacheFolder { get; }

        bool IsSaving { get; }

        void CouldNotSaveToRemote(Uri requestedUri, Exception ex);

        void CouldNotOpenFromRemote(Uri requestedUri, Exception ex);

        void OpenWithConflict(Uri requestedUri);

        void UpdatedRemoteOnLoad(Uri requestedUri);

    }
}
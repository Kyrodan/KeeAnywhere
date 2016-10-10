using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KeeAnywhere.StorageProviders;
using KeePassLib.Cryptography;
using KeePassLib.Utility;

namespace KeeAnywhere.Offline
{
    public class CacheProvider : IStorageProvider
    {
        private readonly IStorageProvider _baseProvider;
        private readonly Uri _requestedUri;
        private readonly ICacheSupervisor _cacheSupervisor;
        private string _cachedFilename;

        public CacheProvider(IStorageProvider baseProvider, Uri requestedUri, ICacheSupervisor cacheSupervisor)
        {
            if (baseProvider == null) throw new ArgumentNullException("baseProvider");
            if (requestedUri == null) throw new ArgumentNullException("requestedUri");
            if (cacheSupervisor == null) throw new ArgumentNullException("cacheSupervisor");
            _baseProvider = baseProvider;
            _requestedUri = requestedUri;
            _cacheSupervisor = cacheSupervisor;
        }

        public string CachedFilename
        {
            get
            {
                if (_cachedFilename == null)
                {
                    var hashBytes = GetMD5Hash(_requestedUri.ToString());
                    var hash = MemUtil.ByteArrayToHexString(hashBytes);
                    _cachedFilename = Path.Combine(_cacheSupervisor.CacheFolder, Path.ChangeExtension(hash, "kdbx"));
                }

                return _cachedFilename;
            }
        }

        public string BaseHashFilename
        {
            get { return Path.ChangeExtension(this.CachedFilename, "basehash"); }
        }

        public string LocalHashFilename
        {
            get { return Path.ChangeExtension(this.CachedFilename, "localhash"); }
        }


        public bool IsCached()
        {
            return File.Exists(this.CachedFilename)
                   && File.Exists(this.BaseHashFilename)
                   && File.Exists(this.LocalHashFilename);
        }

        public bool IsCachedFileChanged()
        {
            var localHash = GetLocalHash();
            var baseHash = GetBaseHash();

            return localHash != baseHash;
        }

        private string GetBaseHash()
        {
            return File.ReadAllText(this.BaseHashFilename);
        }

        private string GetLocalHash()
        {
            return File.ReadAllText(this.LocalHashFilename);
        }


        public async Task<Stream> Load(string path)
        {
            try
            {
                var remoteDetails = await LoadFromRemote(path);

                if (_cacheSupervisor.IsSaving) return remoteDetails.Stream;

                if (this.IsCached()) // If file was cached before?
                {
                    var baseHash = GetBaseHash();
                    var localHash = GetLocalHash();

                    if (localHash != baseHash) // Cached file has been changed since last remote update?
                    {
                        if (baseHash != remoteDetails.Hash) // Remote file has been changed since last update?
                        {
                            // Conflict 
                            _cacheSupervisor.OpenWithConflict(_requestedUri);
                        }
                        else
                        {
                            // No conflict: try updating remote
                            try
                            {
                                await UpdateRemoteFromCache(path);
                                _cacheSupervisor.UpdatedRemoteOnLoad(_requestedUri);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        return File.OpenRead(this.CachedFilename);
                    }


                    /*
                         * 1. Read remote
                         * 2. If remote readable: Is remote changed?
                         *    a. yes => conflict, show message
                         *    b. no => try update remote with cached
                         * 3. open cached
                        */
                }

                await UpdateCacheFromRemote(remoteDetails);
                return remoteDetails.Stream;
            }
            catch (Exception ex) // Could not load file from Network: open cached file, if present
            {
                if (!this.IsCached())
                    throw;

                if (!_cacheSupervisor.IsSaving)
                {
                    _cacheSupervisor.CouldNotOpenFromRemote(_requestedUri, ex);
                }

                return File.OpenRead(this.CachedFilename);
            }
        }

        private async Task UpdateRemoteFromCache(string path)
        {
            var cachedStream = File.OpenRead(this.CachedFilename);
            await this.Save(cachedStream, path);
            var localHash = GetLocalHash();
            File.WriteAllText(this.BaseHashFilename, localHash);
        }

        private async Task UpdateCacheFromRemote(FileDetails remoteDetails)
        {
            await UpdateCache(remoteDetails.Stream, remoteDetails.Hash);
            File.WriteAllText(this.BaseHashFilename, remoteDetails.Hash);
        }

        private async Task UpdateCache(Stream stream, string hash)
        {
            using (var cachedStream = File.Create(this.CachedFilename))
            {
                await stream.CopyToAsync(cachedStream);
                cachedStream.Close();
                stream.Position = 0;
            }
            File.WriteAllText(this.LocalHashFilename, hash);
        }

        private async Task<FileDetails> LoadFromRemote(string path)
        {
            Stream dataStream = new MemoryStream();
            string hash = null;

            using (var remoteStream = await _baseProvider.Load(path))
            {
                // Copy remote to temporary stream (because remote stream may not be seekable)
                var tempStream = new MemoryStream();
                await remoteStream.CopyToAsync(tempStream);
                tempStream.Position = 0;

                // Get Hash of remote stream
                hash = HashStream(tempStream, dataStream);

                remoteStream.Close();
            }

            return new FileDetails(dataStream, hash);
        }

        private static string HashStream(Stream sourceStream, Stream destStream)
        {
            string hash;

            using (var hashingRemoteStream = new HashingStreamEx(sourceStream, false, new SHA256Managed()))
            {
                hashingRemoteStream.CopyTo(destStream);
                hashingRemoteStream.Close();
                hash = MemUtil.ByteArrayToHexString(hashingRemoteStream.Hash);
                destStream.Position = 0;
            }

            return hash;
        }

        public async Task Save(Stream stream, string path)
        {
            var cachedStream = new MemoryStream();
            var hash = HashStream(stream, cachedStream);
            await UpdateCache(cachedStream, hash);

            try // Try to save to remote
            {
                await _baseProvider.Save(cachedStream, path);
                File.WriteAllText(this.BaseHashFilename, hash);
            }
            catch (Exception ex)
            {
                _cacheSupervisor.CouldNotSaveToRemote(_requestedUri, ex);
            }
        }


        public Task<StorageProviderItem> GetRootItem()
        {
            return _baseProvider.GetRootItem();
        }

        public Task<IEnumerable<StorageProviderItem>> GetChildrenByParentItem(StorageProviderItem parent)
        {
            return _baseProvider.GetChildrenByParentItem(parent);
        }

        public bool IsFilenameValid(string filename)
        {
            return _baseProvider.IsFilenameValid(filename);
        }

       
        private static byte[] GetMD5Hash(string s)
        {
            if (s == null) throw new ArgumentNullException("s");

            //MD5 Hash aus dem String berechnen. Dazu muss der string in ein Byte[]
            //zerlegt werden. Danach muss das Resultat wieder zurück in ein string.
            var md5 = new MD5CryptoServiceProvider();
            var textToHash = Encoding.Default.GetBytes(s);
            var result = md5.ComputeHash(textToHash);

            //return BitConverter.ToString(result);
            //return string.Concat(result.Select(_ => _.ToString("X2")));
            return result;
        }
    }

    public struct FileDetails
    {
        public FileDetails(Stream stream, string hash)
        {
            Stream = stream;
            Hash = hash;
        }

        public string Hash { get; set; }
        public Stream Stream { get; set; }
    }
}
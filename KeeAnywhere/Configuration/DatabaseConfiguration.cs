using System;
using System.Runtime.Serialization;

namespace KeeAnywhere.Configuration
{
    /// <summary>
    /// Plugin configuration class. Contains functions to serialize/deserialize to/from JSON.
    /// </summary>
    [DataContract]
    public class DatabaseConfiguration 
    {
        /// <summary>
        /// Gets or sets the name of the OneDrive the KeePass database is synchronized with
        /// </summary>
        [DataMember]
        public string AccountId { get; set; }

        /// <summary>
        /// Gets or sets database file path on OneDrive relative to the user 
        /// </summary>
        [DataMember]
        public string RemoteDatabasePath { get; set; }

        /// <summary>
        /// Gets or sets the path of the local Database
        /// </summary>
        [DataMember]
        public string LocalDatabasePath { get; set; }

        /// <summary>
        /// Gets or sets a boolean to indicate if the database should be synced with OneDrive
        /// </summary>
        [DataMember]
        public bool DoNotSync { get; set; }

        /// <summary>
        /// The SHA1 hash of the local KeePass database
        /// </summary>
        [DataMember]
        public string LocalFileHash { get; set; }

        /// <summary>
        /// Date and time at which the database last synced with OneDrive
        /// </summary>
        [DataMember]
        public DateTime? LastSyncedAt { get; set; }

        /// <summary>
        /// Date and time at which the database has last been compared with its equivallent on OneDrive
        /// </summary>
        [DataMember]
        public DateTime? LastCheckedAt { get; set; }
    }
}


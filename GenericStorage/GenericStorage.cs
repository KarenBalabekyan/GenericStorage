using Dynamix.Net.Extensions;
using Dynamix.Net.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;

namespace Dynamix.Net
{
    /// <inheritdoc />
    /// <summary>
    /// A simple and lightweight tool for persisting data in dotnet (core) apps.
    /// </summary>
    public class GenericStorage : IDisposable
    {
        /// <summary>
        /// Gets the number of elements contained in the GenericStorage.
        /// </summary>
        public int Count => Storage.Count;

        /// <summary>
        /// Configurable behaviour for this GenericStorage instance.
        /// </summary>
        private readonly GenericStorageConfiguration _config;

        /// <summary>
        /// User-provided encryption key, used for encrypting/decrypting values.
        /// </summary>
        private readonly string _encryptionKey;

        /// <summary>
        /// Most current actual, in-memory state representation of the GenericStorage.
        /// </summary>
        private Dictionary<string, byte[]> Storage { get; set; } = new Dictionary<string, byte[]>();

        public GenericStorage() : this(new GenericStorageConfiguration())
        {
        }

        public GenericStorage(GenericStorageConfiguration configuration) : this(configuration, string.Empty)
        {
        }

        public GenericStorage(GenericStorageConfiguration configuration, string encryptionKey)
        {
            _config = configuration ?? throw new ArgumentNullException(nameof(configuration));

            if (_config.EnableEncryption)
            {
                if (string.IsNullOrEmpty(encryptionKey))
                    throw new ArgumentNullException(nameof(encryptionKey),
                        "When EnableEncryption is enabled, an encryptionKey is required when initializing the GenericStorage.");
                _encryptionKey = encryptionKey;
            }

            if (_config.AutoLoad)
                Load();
        }

        /// <summary>
        /// Clears the in-memory contents of the GenericStorage, but leaves any persisted state on disk intact.
        /// </summary>
        /// <remarks>
        /// Use the Destroy method to delete the persisted file on disk.
        /// </remarks>
        public void Clear()
        {
            Storage.Clear();
        }

        /// <summary>
        /// Deletes the persisted file on disk, if it exists, but keeps the in-memory data intact.
        /// </summary>
        /// <remarks>
        /// Use the Clear method to clear only the in-memory contents.
        /// </remarks>
        public void Destroy()
        {
            var filepath = FileHelpers.GetGenericStoreFilePath(_config.FileName);
            if (File.Exists(filepath))
                File.Delete(FileHelpers.GetGenericStoreFilePath(_config.FileName));
        }

        /// <summary>
        /// Gets an object from the GenericStorage, without knowing its type.
        /// </summary>
        /// <param name="key">Unique key, as used when the object was stored.</param>
        public object Get(string key)
        {
            return Get<object>(key);
        }

        /// <summary>
        /// Gets a strong typed object from the GenericStorage.
        /// </summary>
        /// <param name="key">Unique key, as used when the object was stored.</param>
        public T Get<T>(string key)
        {
            var succeeded = Storage.TryGetValue(key, out byte[] raw);
            if (!succeeded) throw new ArgumentNullException($"Could not find key '{key}' in the GenericStorage.");

            if (_config.EnableEncryption)
                raw = CryptographyHelpers.Decrypt(_encryptionKey, _config.EncryptionSalt, raw);

            return raw.FromByteArray<T>();
        }

        /// <summary>
        /// Loads the persisted state from disk into memory, overriding the current memory instance.
        /// </summary>
        /// <remarks>
        /// Simply doesn't do anything if the file is not found on disk.
        /// </remarks>
        public void Load()
        {
            if (!File.Exists(FileHelpers.GetGenericStoreFilePath(_config.FileName))) return;

            var serializedContent = File.ReadAllBytes(FileHelpers.GetGenericStoreFilePath(_config.FileName));

            Storage.Clear();
            Storage = serializedContent.FromByteArray<Dictionary<string, byte[]>>();
        }

        /// <summary>
        /// Stores an object into the GenericStorage.
        /// </summary>
        /// <param name="key">Unique key, can be any string, used for retrieving it later.</param>
        /// <param name="instance"></param>
        public void Store<T>(string key, T instance) 
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            var value = instance.ToByteArray();

            if (Storage.Keys.Contains(key))
                Storage.Remove(key);

            if (_config.EnableEncryption)
                value = CryptographyHelpers.Encrypt(_encryptionKey, _config.EncryptionSalt, value);

            Storage.Add(key, value);
        }

        /// <summary>
        /// Syntax sugar that transforms the response to an IEnumerable<T/>, whilst also passing along an optional WHERE-clause. 
        /// </summary>
        public IEnumerable<T> Query<T>(string key, Func<T, bool> predicate = null)
        {
            var collection = Get<IEnumerable<T>>(key);
            return predicate == null ? collection : collection.Where(predicate);
        }

        /// <summary>
        /// Persists the in-memory store to disk.
        /// </summary>
        public void Persist()
        {
            var serializedBytes = Storage.ToByteArray();

            using (var fileStream = new FileStream(FileHelpers.GetGenericStoreFilePath(_config.FileName),
                FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var writer = new StreamWriter(fileStream))
                {
                    writer.Write(serializedBytes);
                }
            }
        }

        public void Dispose()
        {
            if (_config.AutoSave)
                Persist();

            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        }
    }
}
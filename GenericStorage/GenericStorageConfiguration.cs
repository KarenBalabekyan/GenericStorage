namespace Dynamix.Net
{
    /// <summary>
    /// Provides options to configure GenericStorage to behave just like you want it.
    /// </summary>
    public class GenericStorageConfiguration : IGenericStorageConfiguration
    {
        /// <inheritdoc />
        /// <summary>
        /// Indicates if GenericStorage should automatically load previously persisted state from disk, when it is initialized (defaults to true).
        /// </summary>
        /// <remarks>
        /// Requires manually to call Load() when disabled.
        /// </remarks>
        public bool AutoLoad { get; set; } = true;

        /// <inheritdoc />
        /// <summary>
        /// Indicates if GenericStorage should automatically persist the latest state to disk, on dispose (defaults to true).
        /// </summary>
        /// <remarks>
        /// Disabling this requires a manual call to Persist() in order to save changes to disk.
        /// </remarks>
        public bool AutoSave { get; set; } = true;

        /// <inheritdoc />
        /// <summary>
        /// Indicates if GenericStorage should encrypt its contents when persisting to disk.
        /// </summary>
        public bool EnableEncryption { get; set; } = false;

        /// <summary>
        /// [Optional] Add a custom salt to encryption, when EnableEncryption is enabled.
        /// </summary>
        public string EncryptionSalt { get; set; } = ".GenericStorage";

        /// <inheritdoc />
        /// <summary>
        /// Filename for the persisted state on disk (defaults to ".GenericStorage").
        /// </summary>
        public string FileName { get; set; } = ".GenericStorage";
    }
}
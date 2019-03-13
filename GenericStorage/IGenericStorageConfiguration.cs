namespace Dynamix.Net
{
    public interface IGenericStorageConfiguration
    {
        /// <summary>
        /// Indicates if GenericStorage should automatically load previously persisted state from disk, when it is initialized (defaults to true).
        /// </summary>
        /// <remarks>
        /// Requires manually to call Load() when disabled.
        /// </remarks>
        bool AutoLoad { get; set; }

        /// <summary>
        /// Indicates if GenericStorage should automatically persist the latest state to disk, on dispose (defaults to true).
        /// </summary>
        /// <remarks>
        /// Disabling this requires a manual call to Persist() in order to save changes to disk.
        /// </remarks>
        bool AutoSave { get; set; }

        /// <summary>
        /// Indicates if GenericStorage should encrypt its contents when persisting to disk.
        /// </summary>
        bool EnableEncryption { get; set; }

        /// <summary>
        /// Filename for the persisted state on disk (defaults to ".GenericStorage").
        /// </summary>
        string FileName { get; set; }
    }
}
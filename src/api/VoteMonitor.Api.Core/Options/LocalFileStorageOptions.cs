namespace VoteMonitor.Api.Core.Options;

/// <summary>
/// Options for defining the FileService implementation
/// </summary>
public class LocalFileStorageOptions
{
    /// <summary>
    /// Only relevant when `Type`=`LocalFileService`.
    /// This will be a relative path (`\notes`).
    /// Make sure you configure your container persistent storage on this path
    /// </summary>
    public Dictionary<string, string> StoragePaths { get; set; }
}
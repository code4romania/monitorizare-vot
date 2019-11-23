using System.Collections.Generic;

namespace VotingIrregularities.Api.Options
{
    /// <summary>
    /// Options for defining the FileService implementation
    /// </summary>
    public class FileServiceOptions
    {
        /// <summary>
        /// Can be `BlobService` or `LocalFileService`
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Only relevand when `Type`=`LocalFileService`.
        /// This will be a relative path (`\notes`). Make sure you configure your container persistent storage on this path
        /// </summary>
        public Dictionary<string, string> StoragePaths { get; set; }
    }
}
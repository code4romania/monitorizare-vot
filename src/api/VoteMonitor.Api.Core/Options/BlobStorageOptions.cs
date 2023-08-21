namespace VoteMonitor.Api.Core.Options
{
    /// <summary>
    /// Manages the details about the blob storage being used
    /// </summary>
    public class BlobStorageOptions
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// The name of the blob container.
        /// </summary>
        public string ContainerName { get; set; }

        /// <summary>
        /// Gets or sets blob availability in minutes
        /// </summary>
        public int SASBlobAvailabilityInMinutes { get; set; }
    }
}

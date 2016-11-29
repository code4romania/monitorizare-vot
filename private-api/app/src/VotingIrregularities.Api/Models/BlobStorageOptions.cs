namespace VotingIrregularities.Api.Models
{
    public class BlobStorageOptions
    {
        /// <summary>
        /// The account name used to connect to Blob Storage.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// The key used to connect to Blob Storage.
        /// </summary>
        public string AccountKey { get; set; }

        /// <summary>
        /// The name of the blob container.
        /// </summary>
        public string Container { get; set; }

        public BlobStorageOptions Value
        {
            get { return this; }
        }
    }
}

namespace VoteMonitor.Api.Core.Options
{
    public class S3StorageOptions
    {
        public string BucketName { get; set; }
        public int PresignedUrlExpirationInMinutes { get; set; }
    }
}

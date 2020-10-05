namespace VoteMonitor.Api.Core.Options
{
    public class ApplicationCacheOptions
    {
        public int Hours { get; set; }

        public int Minutes { get; set; } = 30;

        public int Seconds { get; set; }

        public string Implementation { get; set; } = ApplicationCacheImplementationType.NoCache.ToString();
    }

    public enum ApplicationCacheImplementationType
    {
        NoCache,
        MemoryDistributedCache,
        RedisCache
    }
}

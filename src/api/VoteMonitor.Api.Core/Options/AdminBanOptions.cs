namespace VoteMonitor.Api.Core.Options
{
    public class AdminBanOptions
    {
        public bool IsEnabled { get; set; }
        public int[] BannedIds { get; set; } = new int[0];
    }
}

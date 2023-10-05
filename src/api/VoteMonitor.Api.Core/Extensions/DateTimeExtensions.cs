namespace VoteMonitor.Api.Core.Extensions;

public static class DateTimeExtensions
{
    /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
    public static long ToUnixEpochDate(this DateTime date)
        => (long)Math.Round((date.ToUniversalTime() -
                             new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .TotalSeconds);

    public static DateTime? AsUtc(this DateTime? date) =>
       date == null ? null : DateTime.SpecifyKind(
            date.Value,
            DateTimeKind.Utc);
}

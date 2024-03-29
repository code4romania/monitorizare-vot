namespace VoteMonitor.Api.Core.Options;

public class HashOptions
{
    public string Salt { get; set; }

    /// <summary>
    /// Can be set to `Hash` or `ClearText`
    /// `Hash` will use the HashService (that needs the Salt setting) to generate hashes for the password
    /// :warning: `ClearText` will allow your development environment to create and store clear text passwords in the database. Please only use this in development to speed up things.
    /// </summary>
    public HashServiceType HashServiceType { get; set; } = HashServiceType.ClearText;
}
public enum HashServiceType
{
    Hash,
    ClearText
}
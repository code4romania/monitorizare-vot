namespace VoteMonitor.Api.Observer.Models;

public class ActiveObserverFilter
{
    /// <summary>
    /// A list of counties to filter the observers
    /// [ "B", "AB", "CT" ]
    /// </summary>
    public string[] CountyCodes { get; set; }

    /// <summary>
    /// Get only the observers that have not left the polling stations selected in the interval
    /// </summary>
    public bool CurrentlyCheckedIn { get; set; }
}

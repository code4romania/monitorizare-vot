namespace VoteMonitor.Api.Observer.Models
{
    public class ActiveObserverFilter
    {
        /// <summary>
        /// A list of counties to filter the observers
        /// [ "B", "AB", "CT" ]
        /// </summary>
        public string[] CountyCodes { get; set; }
        /// <summary>
        /// This helps filtering the active observers starting from the specified PollingStation number
        /// </summary>
        public int FromPollingStationNumber { get; set; }
        /// <summary>
        /// The end interval for the PollingStation number
        /// </summary>
        public int ToPollingStationNumber { get; set; }
        /// <summary>
        /// Get only the observers that have not left the polling stations selected in the interval
        /// </summary>
        public bool CurrentlyCheckedIn { get; set; }
    }
}

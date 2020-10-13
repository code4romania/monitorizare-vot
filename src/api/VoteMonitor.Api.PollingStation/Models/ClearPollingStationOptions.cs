using System.ComponentModel;

namespace VoteMonitor.Api.PollingStation.Models
{
    public class ClearPollingStationOptions
    {
        [DefaultValue(false)]
        public bool IncludeRelatedData { get; set; }
    }
}

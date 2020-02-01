using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.PollingStation.Models
{
    public class PollingStationsFilter : PagingModel
    {
        public int IdCounty { get; set; }
    }
}
using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.PollingStation.Models
{
    public class PollingStationsFilterModel : PagingModel
    {
        public int CountyId { get; set; }
    }
}
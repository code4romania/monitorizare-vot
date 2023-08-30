using CsvHelper.Configuration.Attributes;

namespace VoteMonitor.Api.Location.Models
{
    public class PollingStationCsvModel
    {
        [Index(0)]
        public string CountyCode { get; set; }

        [Index(1)]
        public string Address { get; set; }

        [Index(2)]
        public int Number { get; set; }
    }
}

namespace VoteMonitor.Api.Location.Models
{
    public class CountyPollingStationLimit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Limit { get; set; }
        public bool Diaspora { get; set; }

    }
}
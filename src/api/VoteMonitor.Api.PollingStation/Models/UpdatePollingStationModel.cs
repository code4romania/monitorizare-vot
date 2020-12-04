namespace VoteMonitor.Api.PollingStation.Models
{
    public class UpdatePollingStationModel
    {
        public string Address { get; set; }
        public string Coordinates { get; set; }
        public string AdministrativeTerritoryCode { get; set; }
        public string TerritoryCode { get; set; }
        public int Number { get; set; }
    }
}
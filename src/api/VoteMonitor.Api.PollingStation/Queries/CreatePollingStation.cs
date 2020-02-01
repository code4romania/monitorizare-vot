using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class CreatePollingStation : IRequest<Models.PollingStation>
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Coordinates { get; set; }
        public string AdministrativeTerritoryCode { get; set; }
        public int IdCounty { get; set; }
        public string TerritoryCode { get; set; }
        public int Number { get; set; }


        public CreatePollingStation(Models.PollingStation pollingStation)
        {
            Id = pollingStation.Id;
            Address = pollingStation.Address;
            Coordinates = pollingStation.Coordinates;
            AdministrativeTerritoryCode = pollingStation.AdministrativeTerritoryCode;
            IdCounty = pollingStation.IdCounty;
            TerritoryCode = pollingStation.TerritoryCode;
            Number = pollingStation.Number;
        }
    }
}
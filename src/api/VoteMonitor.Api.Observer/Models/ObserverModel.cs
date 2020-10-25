using System;

namespace VoteMonitor.Api.Observer.Models
{
    public class ObserverModel
    {
        public int Id { get; set; }
        public string Ngo { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public DateTime? DeviceRegisterDate { get; set; }
        public DateTime? LastSeen { get; set; }
        public int NumberOfNotes { get; set; }
        public int NumberOfPollingStations { get; set; }
    }
}

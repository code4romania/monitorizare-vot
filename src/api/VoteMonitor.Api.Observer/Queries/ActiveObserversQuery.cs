using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Queries
{
    public class ActiveObserversQuery : IRequest<List<ObserverModel>>
    {
        public string[] CountyCodes { get; set; }
        public int FromPollingStationNumber { get; set; }
        public int ToPollingStationNumber { get; set; }
        public bool CurrentlyCheckedIn { get; set; }
        public int NgoId { get; set; }
    }
}
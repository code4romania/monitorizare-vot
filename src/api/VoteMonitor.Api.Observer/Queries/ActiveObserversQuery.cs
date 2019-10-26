using AutoMapper;
using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Queries
{
    public class ActiveObserversQuery :IRequest<List<ObserverModel>>
    {
        public string[] CountyCodes { get; set; }
        public int FromPollingStationNumber { get; set; }
        public int ToPollingStationNumber { get; set; }
        public bool CurrentlyCheckedIn { get; set; }
        public int IdNgo { get; set; }
    }

    public class ActiveObserverProfile : Profile
    {
        public ActiveObserverProfile()
        {
            CreateMap<ActiveObserverFilter, ActiveObserversQuery>();
            CreateMap<ActiveObserversQuery, ActiveObserverFilter>();
        }
    }
}
using System.Collections.Generic;
using MediatR;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Queries
{
    public class ObserverListQuery : IRequest<List<ObserverModel>>
    {
        public int IdNgo { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
    }
}

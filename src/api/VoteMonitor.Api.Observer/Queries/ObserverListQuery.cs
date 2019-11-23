using System.Collections.Generic;
using MediatR;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Queries
{
    public class ObserverListQuery : PagingModel
    {
        public string Number { get; set; }
        public string Name { get; set; }
    }
}

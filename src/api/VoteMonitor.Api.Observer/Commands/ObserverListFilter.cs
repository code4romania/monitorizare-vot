using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ObserverListFilter : IRequest<IList<ObserverModel>>
    {
        public int NgoId { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
    }
}

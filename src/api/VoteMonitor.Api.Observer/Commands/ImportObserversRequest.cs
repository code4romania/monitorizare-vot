using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ImportObserversRequest : IRequest<int> {
        public int IdOng { get; set; }
        public string FilePath { get;set;}
    }
}

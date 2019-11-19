using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ObserverGenerateCommand : IRequest<List<GeneratedObserver>>
    {
        public ObserverGenerateCommand(int nrObs, int id)
        {
            NrObservers = nrObs;
            IdNgo = id;
        }

        public int NrObservers { get; set; }
        public int IdNgo { get; set; }
    }
}

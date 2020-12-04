using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ObserverGenerateCommand : IRequest<List<GeneratedObserver>>
    {
        public ObserverGenerateCommand(int numberOfObservers, int ngoId)
        {
            NumberOfObservers = numberOfObservers;
            NgoId = ngoId;
        }

        public int NumberOfObservers { get; set; }
        public int NgoId { get; set; }
    }
}

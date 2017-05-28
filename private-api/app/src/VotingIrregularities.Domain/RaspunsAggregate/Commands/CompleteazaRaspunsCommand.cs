using System.Collections.Generic;
using MediatR;

namespace VotingIrregularities.Domain.RaspunsAggregate.Commands
{
    public class CompleteazaRaspunsCommand : IRequest<int>
    {
        public CompleteazaRaspunsCommand()
        {
            Raspunsuri = new List<ModelRaspuns>();
        }
        public int IdObservator { get; set; }
        public List<ModelRaspuns> Raspunsuri { get; set; }

    }
}

using MediatR;
using System;

namespace VotingIrregularities.Domain.SectieAggregate
{
    public class ActualizeazaSectieCommand : IAsyncRequest<int>
    {
        public int IdObservator { get; set; }
        public int IdSectieDeVotare { get; set; }
        public DateTime OraPlecarii { get; set; }
    }
}

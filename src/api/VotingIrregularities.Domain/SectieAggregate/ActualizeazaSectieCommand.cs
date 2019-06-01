using MediatR;
using System;

namespace VotingIrregularities.Domain.SectieAggregate
{
    [Obsolete("Use UpdatePollingSectionCommand instead")]
    public class ActualizeazaSectieCommand : IRequest<int>
    {
        public int IdObservator { get; set; }
        public int IdSectieDeVotare { get; set; }
        public DateTime OraPlecarii { get; set; }
    }
}

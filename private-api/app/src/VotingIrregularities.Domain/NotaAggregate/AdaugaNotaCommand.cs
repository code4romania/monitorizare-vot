using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace VotingIrregularities.Domain.NotaAggregate
{
    public class AdaugaNotaCommand : IAsyncRequest<int>
    {
        public int IdObservator { get; set; }
    }
}

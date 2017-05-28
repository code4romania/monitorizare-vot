using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Domain.UserAggregate
{
    public class InregistreazaDispozitivCommand : IRequest<int>
    {
        public string IdDispozitivMobil { get; set; }
        public int IdObservator { get; set; }
    }
}

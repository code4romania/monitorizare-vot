using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.UserAggregate
{
    public class InregistreazaDispozitivHandler : IAsyncRequestHandler<InregistreazaDispozitivCommand, int>
    {
        public InregistreazaDispozitivHandler(VotingContext context)
        {

        }
        public Task<int> Handle(InregistreazaDispozitivCommand message)
        {
            throw new NotImplementedException();
        }
    }
}

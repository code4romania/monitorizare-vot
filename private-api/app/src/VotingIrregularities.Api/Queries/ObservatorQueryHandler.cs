using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SimpleInjector;
using VotingIrregularities.Api.Models.AccountViewModels;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Queries
{
    public class ObservatorQueryHandler : IAsyncRequestHandler<ApplicationUser, ModelObservatorInregistrat>
    {
        private readonly VotingContext _context;

        public ObservatorQueryHandler(VotingContext context)
        {
            _context = context;
        }
        public async Task<ModelObservatorInregistrat> Handle(ApplicationUser message)
        {
            //var userInfo = _context.Observator
            return await Task.FromResult(new ModelObservatorInregistrat
            {
                EsteAutentificat = true,
                IdObservator = 1,
                PrimaAutentificare = false
            });
        }
    }
}

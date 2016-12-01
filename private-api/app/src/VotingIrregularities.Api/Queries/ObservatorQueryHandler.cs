using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SimpleInjector;
using VotingIrregularities.Api.Models.AccountViewModels;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Queries
{
    public class ObservatorQueryHandler : IAsyncRequestHandler<ApplicationUser, ModelObservatorInregistrat>
    {
        private readonly VotingContext _context;
        private readonly IHashService _hash;

        public ObservatorQueryHandler(VotingContext context, IHashService hash)
        {
            _context = context;
            _hash = hash;
        }
        public async Task<ModelObservatorInregistrat> Handle(ApplicationUser message)
        {
            var hashValue = _hash.GetHash(message.Pin);

            var userinfo = _context.Observator
                .FirstOrDefault(a => a.Pin == hashValue &&
                                     (string.IsNullOrWhiteSpace(a.IdDispozitivMobil) || a.IdDispozitivMobil == message.UDID) &&
                                     a.NumarTelefon == message.Phone);
            if (userinfo == null)
                return new ModelObservatorInregistrat
                {
                    EsteAutentificat = false
                };

            return new ModelObservatorInregistrat
            {
                IdObservator = userinfo.IdObservator,
                EsteAutentificat = true,
                PrimaAutentificare = string.IsNullOrWhiteSpace(userinfo.IdDispozitivMobil)
            };
        }
    }
}

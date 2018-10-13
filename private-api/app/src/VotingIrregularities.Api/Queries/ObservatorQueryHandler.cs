using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VotingIrregularities.Api.Models.AccountViewModels;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Queries
{
    public class ObservatorQueryHandler : AsyncRequestHandler<ApplicationUser, ModelObservatorInregistrat>
    {
        private readonly VotingContext _context;
        private readonly IHashService _hash;

        public ObservatorQueryHandler(VotingContext context, IHashService hash)
        {
            _context = context;
            _hash = hash;
        }

        protected override async Task<ModelObservatorInregistrat> HandleCore(ApplicationUser message)
        {
            var hashValue = _hash.GetHash(message.Pin);

            var userinfo = await _context.Observers
                .FirstOrDefaultAsync(a => a.Pin == hashValue &&
                                     (string.IsNullOrWhiteSpace(a.MobileDeviceId) || a.MobileDeviceId == message.UDID) &&
                                     a.Phone == message.Phone);
            if (userinfo == null)
                return new ModelObservatorInregistrat
                {
                    EsteAutentificat = false
                };

            return new ModelObservatorInregistrat
            {
                IdObservator = userinfo.Id,
                EsteAutentificat = true,
                PrimaAutentificare = string.IsNullOrWhiteSpace(userinfo.MobileDeviceId)
            };
        }
    }
}

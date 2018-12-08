using System.Linq;
using System.Threading.Tasks;
using MediatR;
using VotingIrregularities.Api.Models.AccountViewModels;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Queries
{
    public class ObserverQueryHandler : IAsyncRequestHandler<ApplicationUser, RegisteredObserverModel>
    {
        private readonly VotingContext _context;
        private readonly IHashService _hash;

        public ObserverQueryHandler(VotingContext context, IHashService hash)
        {
            _context = context;
            _hash = hash;
        }
        public async Task<RegisteredObserverModel> Handle(ApplicationUser message)
        {
            var hashValue = _hash.GetHash(message.Pin);

            var userInfo = _context.Observer
                .FirstOrDefault(a => a.Pin == hashValue &&
                                     (string.IsNullOrWhiteSpace(a.MobileDeviceId) || a.MobileDeviceId == message.UDID) &&
                                     a.TelephoneNumber == message.Phone);
            if (userInfo == null)
                return new RegisteredObserverModel
                {
                    IsAuthenticated = false
                };

            return new RegisteredObserverModel
            {
                ObserverId = userInfo.ObserverId,
                IsAuthenticated = true,
                FirstAuthentication = string.IsNullOrWhiteSpace(userInfo.MobileDeviceId)
            };
        }
    }
}

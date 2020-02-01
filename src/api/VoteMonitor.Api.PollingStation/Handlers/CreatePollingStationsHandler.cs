using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Handlers
{
    public class CreatePollingStationsHandler : IRequestHandler<CreatePollingStation, Models.PollingStation>
    {
        public Task<Models.PollingStation> Handle(CreatePollingStation request, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}

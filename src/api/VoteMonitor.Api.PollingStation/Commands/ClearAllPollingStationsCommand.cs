using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.PollingStation.Models;

namespace VoteMonitor.Api.PollingStation.Commands
{
    public class ClearAllPollingStationsCommand : IRequest<Result>
    {
        internal ClearPollingStationOptions Options;

        public ClearAllPollingStationsCommand(ClearPollingStationOptions options)
        {
            this.Options = options;
        }
    }
}

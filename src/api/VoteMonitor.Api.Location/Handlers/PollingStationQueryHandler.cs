using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Location.Services;

namespace VoteMonitor.Api.Location.Handlers
{
    public class PollingStationQueryHandler : AsyncRequestHandler<PollingStationQuery, int>
    {
        private readonly IPollingStationService _pollingStationService;

        public PollingStationQueryHandler(IPollingStationService pollingStationService)
        {
            _pollingStationService = pollingStationService;
        }

        protected override async Task<int> HandleCore(PollingStationQuery message)
        {
            return await _pollingStationService.GetPollingStationByCountyCode(message.IdPollingStation, message.CountyCode);
        }
    }
}

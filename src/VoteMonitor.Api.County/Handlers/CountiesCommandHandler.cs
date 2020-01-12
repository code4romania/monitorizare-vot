using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.County.Models;
using VoteMonitor.Api.Observer.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class CountiesCommandHandler : IRequestHandler<GetCountiesForExport, List<CountyCsvModel>>
    {
        private readonly VoteMonitorContext _voteMonitorContext;
        private readonly ILogger _logger;
        private readonly IHashService _hash;

        public CountiesCommandHandler(VoteMonitorContext voteMonitorContext, ILogger logger)
        {
            _voteMonitorContext = voteMonitorContext;
            _logger = logger;
        }

        public async Task<List<CountyCsvModel>> Handle(GetCountiesForExport request, CancellationToken cancellationToken)
        {
            return await _voteMonitorContext.Counties
                     .OrderBy(c => c.Order)
                     .Select(c => new CountyCsvModel
                     {
                         Id = c.Id,
                         Code = c.Code,
                         Name = c.Name,
                         NumberOfPollingStations = c.NumberOfPollingStations,
                         Diaspora = c.Diaspora,
                         Order = c.Order
                     })
                     .ToListAsync();
        }
    }
}
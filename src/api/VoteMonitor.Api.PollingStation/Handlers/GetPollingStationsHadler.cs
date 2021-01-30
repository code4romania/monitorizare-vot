using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers
{
    public class GetPollingStationsHandler : IRequestHandler<GetPollingStations, IEnumerable<GetPollingStationModel>>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetPollingStationsHandler(VoteMonitorContext context, IMapper mapper, ILogger<GetPollingStationsHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<GetPollingStationModel>> Handle(GetPollingStations request, CancellationToken cancellationToken)
        {
            var skip = (request.Page - 1) * request.PageSize;
            var take = request.PageSize;

            try
            {
                var iQueryable = CreateQuery(request);

                var pollingStations = await iQueryable
                    .Skip(skip)
                    .Take(take)
                    .Select(m => _mapper.Map<GetPollingStationModel>(m))
                    .ToListAsync(cancellationToken);

                return pollingStations;
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, "Error retrieving Polling Stations", ex);
                throw;
            }
        }

        private IQueryable<Entities.PollingStation> CreateQuery(GetPollingStations request)
        {
            IQueryable<Entities.PollingStation> iQueryable = _context.PollingStations;

            if (request.CountyId > 0)
            {
                iQueryable = iQueryable.Where(p => p.IdCounty == request.CountyId);
            }

            return iQueryable;
        }
    }
}

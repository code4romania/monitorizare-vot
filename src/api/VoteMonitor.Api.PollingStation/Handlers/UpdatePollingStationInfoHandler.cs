﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers
{
    public class UpdatePollingStationInfoHandler : IRequestHandler<UpdatePollingStationInfo, Unit>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UpdatePollingStationInfoHandler(VoteMonitorContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdatePollingStationInfo request, CancellationToken cancellationToken)
        {
            try
            {
                var pollingStationInfo = _context.PollingStationInfos.FirstOrDefault(
                    p => p.IdObserver == request.IdObserver &&
                    p.IdPollingStation == request.IdPollingStation);

                if(pollingStationInfo == null)
                {
                    return Unit.Value;
                }

                pollingStationInfo.ObserverLeaveTime = request.ObserverLeaveTime;
                pollingStationInfo.LastModified = DateTime.Now;

                _context.Update(pollingStationInfo);
                await _context.SaveChangesAsync(cancellationToken);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating polling station info: ", ex);
                throw;
            }
        }
    }
}

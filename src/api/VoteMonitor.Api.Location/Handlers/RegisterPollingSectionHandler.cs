using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.Handlers
{
    public class RegisterPollingSectionHandler : IRequestHandler<RegisterPollingStationCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public RegisterPollingSectionHandler(VoteMonitorContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(RegisterPollingStationCommand message, CancellationToken cancellationToken)
        {
            try
            {
                //TODO[DH] this can be moved to a previous step, before the command is executed
                var pollingStation = await _context.PollingStations
                    .Where(a =>
                        a.Number == message.IdPollingStation &&
                        a.County.Code == message.CountyCode)
                    .FirstOrDefaultAsync();

                if (pollingStation == null)
                    throw new ArgumentException("Sectia nu exista");

                var formular = await _context.PollingStationInfos
                    .FirstOrDefaultAsync(a =>
                        a.IdObserver == message.IdObserver &&
                        a.IdPollingStation == pollingStation.Id);

                if (formular == null)
                {
                    formular = _mapper.Map<PollingStationInfo>(message);

                    formular.IdPollingStation = pollingStation.Id;
                    formular.IdObserver = message.IdObserver;

                    _context.Add(formular);
                }
                else
                {
                    _mapper.Map(message, formular);
                }

                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }
    }
}

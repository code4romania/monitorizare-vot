using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.Handlers;

public class RegisterPollingSectionHandler : IRequestHandler<RegisterPollingStationCommand, int>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public RegisterPollingSectionHandler(VoteMonitorContext context, ILogger<RegisterPollingSectionHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(RegisterPollingStationCommand message, CancellationToken cancellationToken)
    {
        try
        {
            //TODO[DH] this can be moved to a previous step, before the command is executed
            var pollingStation = await _context.PollingStations
                .Where(a =>
                    a.Number == message.PollingStationNumber
                    && a.Municipality.Code == message.MunicipalityCode
                    && a.Municipality.County.Code == message.CountyCode)
                .FirstOrDefaultAsync();

            if (pollingStation == null)
            {
                throw new ArgumentException($"polling station not found for {message.CountyCode} {message.MunicipalityCode}");
            }

            var pollingStationInfo = await _context.PollingStationInfos
                .FirstOrDefaultAsync(a =>
                    a.IdObserver == message.IdObserver &&
                    a.IdPollingStation == pollingStation.Id);

            if (pollingStationInfo == null)
            {
                pollingStationInfo = new PollingStationInfo
                    {
                        IdPollingStation = pollingStation.Id,
                        IdObserver = message.IdObserver,
                        IsPollingStationPresidentFemale = message.IsPollingStationPresidentFemale,
                        LastModified = DateTime.UtcNow,
                        ObserverArrivalTime = message.ObserverArrivalTime,
                        ObserverLeaveTime = message.ObserverLeaveTime
                    };

                _context.Add(pollingStationInfo);
            }
            else
            {
                pollingStationInfo.IsPollingStationPresidentFemale = message.IsPollingStationPresidentFemale;
                pollingStationInfo.LastModified = DateTime.UtcNow;
                pollingStationInfo.ObserverArrivalTime = message.ObserverArrivalTime;
                pollingStationInfo.ObserverLeaveTime = message.ObserverLeaveTime;
            }

            return await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return -1;
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core.Extensions;
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
                return await SaveToPollingStationInfoCorruptedData(message);
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

                    LastModified = DateTime.UtcNow,
                    ObserverArrivalTime = message.ObserverArrivalTime.AsUtc(),
                    ObserverLeaveTime = message.ObserverLeaveTime.AsUtc(),
                    NumberOfVotersOnTheList = message.NumberOfVotersOnTheList,
                    NumberOfCommissionMembers = message.NumberOfCommissionMembers,
                    NumberOfFemaleMembers = message.NumberOfFemaleMembers,
                    MinPresentMembers = message.MinPresentMembers,
                    ChairmanPresence = message.ChairmanPresence,
                    SinglePollingStationOrCommission = message.SinglePollingStationOrCommission,
                    AdequatePollingStationSize = message.AdequatePollingStationSize,
                };

                _context.Add(pollingStationInfo);
            }
            else
            {
                pollingStationInfo.LastModified = DateTime.UtcNow;
                pollingStationInfo.ObserverArrivalTime = message.ObserverArrivalTime.AsUtc();
                pollingStationInfo.ObserverLeaveTime = message.ObserverLeaveTime.AsUtc();
                pollingStationInfo.NumberOfVotersOnTheList = message.NumberOfVotersOnTheList;
                pollingStationInfo.NumberOfCommissionMembers = message.NumberOfCommissionMembers;
                pollingStationInfo.NumberOfFemaleMembers = message.NumberOfFemaleMembers;
                pollingStationInfo.MinPresentMembers = message.MinPresentMembers;
                pollingStationInfo.ChairmanPresence = message.ChairmanPresence;
                pollingStationInfo.SinglePollingStationOrCommission = message.SinglePollingStationOrCommission;
                pollingStationInfo.AdequatePollingStationSize = message.AdequatePollingStationSize;
            }

            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return -1;
    }

    private async Task<int> SaveToPollingStationInfoCorruptedData(RegisterPollingStationCommand message)
    {
        var pollingStationInfo = await _context.PollingStationInfosCorrupted
               .FirstOrDefaultAsync(a =>
                   a.IdObserver == message.IdObserver &&
                   a.MunicipalityCode == message.MunicipalityCode &&
                   a.CountyCode == message.CountyCode);

        if (pollingStationInfo == null)
        {
            pollingStationInfo = new PollingStationInfoCorrupted
            {
                IdObserver = message.IdObserver,
                CountyCode = message.CountyCode,
                MunicipalityCode = message.MunicipalityCode,
                PollingStationNumber = message.PollingStationNumber,

                LastModified = DateTime.UtcNow,
                ObserverArrivalTime = message.ObserverArrivalTime.AsUtc(),
                ObserverLeaveTime = message.ObserverLeaveTime.AsUtc(),
                NumberOfVotersOnTheList = message.NumberOfVotersOnTheList,
                NumberOfCommissionMembers = message.NumberOfCommissionMembers,
                NumberOfFemaleMembers = message.NumberOfFemaleMembers,
                MinPresentMembers = message.MinPresentMembers,
                ChairmanPresence = message.ChairmanPresence,
                SinglePollingStationOrCommission = message.SinglePollingStationOrCommission,
                AdequatePollingStationSize = message.AdequatePollingStationSize,
            };

            _context.Add(pollingStationInfo);
        }
        else
        {
            pollingStationInfo.LastModified = DateTime.UtcNow;
            pollingStationInfo.ObserverArrivalTime = message.ObserverArrivalTime.AsUtc();
            pollingStationInfo.ObserverLeaveTime = message.ObserverLeaveTime.AsUtc();
            pollingStationInfo.NumberOfVotersOnTheList = message.NumberOfVotersOnTheList;
            pollingStationInfo.NumberOfCommissionMembers = message.NumberOfCommissionMembers;
            pollingStationInfo.NumberOfFemaleMembers = message.NumberOfFemaleMembers;
            pollingStationInfo.MinPresentMembers = message.MinPresentMembers;
            pollingStationInfo.ChairmanPresence = message.ChairmanPresence;
            pollingStationInfo.SinglePollingStationOrCommission = message.SinglePollingStationOrCommission;
            pollingStationInfo.AdequatePollingStationSize = message.AdequatePollingStationSize;
        }

        return await _context.SaveChangesAsync();
    }
}

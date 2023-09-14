using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers;

public class CheckObserverExistsHandler : IRequestHandler<GetObserverDetails, ObserverModel>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public CheckObserverExistsHandler(VoteMonitorContext context,ILogger<CheckObserverExistsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ObserverModel> Handle(GetObserverDetails request, CancellationToken cancellationToken)
    {
        try
        {
            var observer = await _context.Observers
                .Where(p => p.Id == request.ObserverId && p.IdNgo == request.NgoId)
                .Select(o=> new ObserverModel
                {
                    Id = o.Id,
                    Name = o.Name,
                    Phone = o.Phone,
                    Ngo = o.Ngo.Name,
                    NumberOfNotes = o.Notes.Count,
                    NumberOfPollingStations = o .PollingStationInfos.Count,
                    DeviceRegisterDate = o.DeviceRegisterDate
                })
                .FirstOrDefaultAsync(cancellationToken);

            return observer;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving observer: ");
            throw;
        }
    }
}

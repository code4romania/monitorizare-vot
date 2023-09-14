using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers;

public class ActiveObserversQueryHandler : IRequestHandler<ActiveObserversQuery, List<ObserverModel>>
{
    private readonly VoteMonitorContext _context;

    public ActiveObserversQueryHandler(VoteMonitorContext context)
    {
        _context = context;
    }
    public Task<List<ObserverModel>> Handle(ActiveObserversQuery request, CancellationToken cancellationToken)
    {
        var results = _context.PollingStationInfos
            .Include(pi => pi.PollingStation)
            .Include(pi => pi.PollingStation.Municipality)
            .ThenInclude(c => c.County)
            .Include(pi => pi.Observer)
            .Where(i => request.CountyCodes.Contains(i.PollingStation.Municipality.County.Code));

        if (request.NgoId > 0)
        {
            results = results.Where(i => i.Observer.IdNgo == request.NgoId);
        }

        var observers = results
            .Select(i => i.Observer)
            .AsEnumerable()
            .Select(o => new ObserverModel
            {
                Id = o.Id,
                Name = o.Name,
                Phone = o.Phone,
                Ngo = o.Ngo.Name,
                NumberOfNotes = o.Notes.Count,
                NumberOfPollingStations = o.PollingStationInfos.Count,
                DeviceRegisterDate = o.DeviceRegisterDate
            })
            .ToList();

        return Task.FromResult(observers);
    }
}

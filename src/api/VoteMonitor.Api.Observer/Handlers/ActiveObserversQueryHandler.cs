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
            .AsNoTracking()
            .Include(pi => pi.PollingStation)
            .Include(pi => pi.PollingStation.Municipality)
            .ThenInclude(c => c.County)
            .Include(pi => pi.Observer)
            .ThenInclude(o=>o.Ngo)
            .Select(x => new 
                {
                    ObserverId = x.Observer.Id,
                    ObserverName = x.Observer.Name,
                    ObserverPhone = x.Observer.Phone,
                    NgoName = x.Observer.Ngo.Name,
                    NgoId = x.Observer.Ngo.Id,
                    NumberOfNotes = x.Observer.Notes.Count,
                    NumberOfPollingStations = x.Observer.PollingStationInfos.Count,
                    DeviceRegisterDate = x.Observer.DeviceRegisterDate,
                    CountyCode  = x.PollingStation.Municipality.County.Code,
                    LastAnswer = x.Observer.Answers.Any() ? x.Observer.Answers.Max(a => a.LastModified) : (DateTime?)null,
                    LastNote = x.Observer.Notes.Any() ? x.Observer.Notes.Max(a => a.LastModified) : (DateTime?)null,
            })
            .Where(x => request.CountyCodes.Contains(x.CountyCode));

        if (request.NgoId > 0)
        {
            results = results.Where(x => x.NgoId == request.NgoId);
        }

        var observers = results
            .Select(x => new ObserverModel
            {
                Id = x.ObserverId,
                Name = x.ObserverName,
                Phone = x.ObserverPhone,
                Ngo = x.NgoName,
                NumberOfNotes = x.NumberOfNotes,
                NumberOfPollingStations = x.NumberOfPollingStations,
                DeviceRegisterDate = x.DeviceRegisterDate,
                LastSeen = (x.LastAnswer ?? DateTime.MinValue) > (x.LastNote ?? DateTime.MinValue) ? x.LastAnswer : x.LastNote,
            })
            .ToList();

        return Task.FromResult(observers);
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers;

public class ObserverListQueryHandler : IRequestHandler<ObserverListCommand, ApiListResponse<ObserverModel>>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public ObserverListQueryHandler(VoteMonitorContext context, ILogger<ObserverListQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<ApiListResponse<ObserverModel>> Handle(ObserverListCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Searching for Observers with the following filters (NgoId, Name, Phone): {request.NgoId}, {request.Name}, {request.Number}");

        IQueryable<Entities.Observer> observers = _context.Observers;

        if (request.NgoId > 0)
        {
            observers = observers.Where(o => o.IdNgo == request.NgoId);
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            observers = observers.Where(o => o.Name.Contains(request.Name.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(request.Number))
        {
            observers = observers.Where(o => o.Phone.Contains(request.Number.Trim()));
        }

        var count = await observers.CountAsync(cancellationToken);

        var requestedPageObservers = GetPagedQuery(observers, request.Page, request.PageSize)
            .Select(o => new
            {
                Id = o.Id,
                Phone = o.Phone,
                Name = o.Name,
                NgoName = o.Ngo.Name,
                LastAnswer = o.Answers.Any() ? o.Answers.Max(a=>a.LastModified) : (DateTime?)null,
                LastNote = o.Notes.Any() ? o.Notes.Max(a => a.LastModified) : (DateTime?)null,
                NumberOfPollingStations = o.PollingStationInfos.Count,
                NumberOfNotes = o.Notes.Count,
                DeviceRegisterDate = o.DeviceRegisterDate
            })
            .ToList();

        return new ApiListResponse<ObserverModel>
        {
            TotalItems = count,
            Data = requestedPageObservers.Select(o => new ObserverModel
            {
                Id = o.Id,
                Phone = o.Phone,
                Name = o.Name,
                Ngo = o.NgoName,
                LastSeen = (o.LastAnswer ?? DateTime.MinValue) > (o.LastNote ?? DateTime.MinValue) ? o.LastAnswer : o.LastNote,
                NumberOfPollingStations = o.NumberOfPollingStations,
                NumberOfNotes = o.NumberOfNotes,
                DeviceRegisterDate = o.DeviceRegisterDate
            }).ToList(),
            Page = request.Page,
            PageSize = request.PageSize
        };
    }

    private static IQueryable<Entities.Observer> GetPagedQuery(IQueryable<Entities.Observer> observers, int page, int pageSize)
    {
        if (pageSize > 0)
        {
            return observers
                .Skip(pageSize * (page - 1))
                .Take(pageSize);
        }

        return observers;
    }
}

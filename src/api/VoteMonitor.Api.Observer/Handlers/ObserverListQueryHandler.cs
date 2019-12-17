using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class ObserverListQueryHandler : IRequestHandler<ObserverListCommand, ApiListResponse<ObserverModel>>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        public ObserverListQueryHandler(VoteMonitorContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<ApiListResponse<ObserverModel>> Handle(ObserverListCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Searching for Observers with the following filters (IdNgo, Name, Phone): {request.IdNgo}, {request.Name}, {request.Number}");

            IQueryable<Entities.Observer> observers = _context.Observers
                .Include(o => o.Ngo)
                .Include(o => o.Notes)
                .Include(o => o.PollingStationInfos);

            if (request.IdNgo > 0)
                observers = observers.Where(o => o.IdNgo == request.IdNgo);
            if (!string.IsNullOrEmpty(request.Name))
                observers = observers.Where(o => o.Name.Contains(request.Name));
            if (!string.IsNullOrEmpty(request.Number))
                observers = observers.Where(o => o.Phone.Contains(request.Number));

            var count = await observers.CountAsync(cancellationToken);

            var requestedPageObservers = GetPagedQuery(observers, request.Page, request.PageSize)
                .ToList()
                .Select(Mapper.Map<ObserverModel>);


            return new ApiListResponse<ObserverModel>
            {
                TotalItems = count,
                Data = requestedPageObservers.ToList(),
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
}

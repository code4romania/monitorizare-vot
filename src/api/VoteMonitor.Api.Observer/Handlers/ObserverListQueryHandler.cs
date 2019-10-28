using System.Collections.Generic;
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
using VoteMonitor.Api.Observer.Queries;
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

            var observers = _context.Observers.AsQueryable();
            
            if (request.IdNgo > 0)
                observers = observers.Where(o => o.IdNgo == request.IdNgo);
            if (!string.IsNullOrEmpty(request.Name))
                observers = observers.Where(o => o.Name.Contains(request.Name));
            if (!string.IsNullOrEmpty(request.Number))
                observers = observers.Where(o => o.Phone.Contains(request.Number));

            var allObservers = observers
                .Include(o => o.Ngo)
                .Include(o => o.Notes)
                .Include(o => o.PollingStationInfos);
            var count = await allObservers.CountAsync();

            var requestedPageObservers = observers
                .Skip(request.PageSize * (request.Page - 1))
                .Take(request.PageSize)
                .Select(Mapper.Map<ObserverModel>)
                .ToList();

            return new ApiListResponse<ObserverModel> {
                TotalItems = count,
                Data = requestedPageObservers,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}

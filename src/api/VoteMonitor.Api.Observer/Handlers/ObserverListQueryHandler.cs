using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class ObserverListQueryHandler : IRequestHandler<ObserverListQuery, List<ObserverModel>>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        public ObserverListQueryHandler(VoteMonitorContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
        public Task<List<ObserverModel>> Handle(ObserverListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Searching for Observers with the following filters (IdNgo, Name, Phone): {request.IdNgo}, {request.Name}, {request.Number}");

            var observers = _context.Observers.AsQueryable();
            
            if (request.IdNgo > 0)
                observers = observers.Where(o => o.IdNgo == request.IdNgo);
            if (!string.IsNullOrEmpty(request.Name))
                observers = observers.Where(o => o.Name.Contains(request.Name));
            if (!string.IsNullOrEmpty(request.Number))
                observers = observers.Where(o => o.Phone.Contains(request.Number));

            return Task.FromResult(observers
                .Include(o => o.Ngo)
                .Include(o => o.Notes)
                .Include(o => o.PollingStationInfos)
                .Select(Mapper.Map<ObserverModel>).ToList());
        }
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class ActiveObserversQueryHandler : IRequestHandler<ActiveObserversQuery, List<ObserverModel>>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public ActiveObserversQueryHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public Task<List<ObserverModel>> Handle(ActiveObserversQuery request, CancellationToken cancellationToken)
        {
            var results = _context.PollingStationInfos
                .Include(pi => pi.PollingStation)
                .Include(pi => pi.PollingStation.County)
                .Include(pi => pi.Observer)
                .Where(i => request.CountyCodes.Contains(i.PollingStation.County.Code))
                .Where(i => i.PollingStation.Number >= request.FromPollingStationNumber)
                .Where(i => i.PollingStation.Number <= request.ToPollingStationNumber);

            if (request.NgoId > 0)
            {
                results = results.Where(i => i.Observer.IdNgo == request.NgoId);
            }

            var observers = results
                    .Select(i => i.Observer)
                    .AsEnumerable()
                    .Select(_mapper.Map<ObserverModel>)
                    .ToList();

            return Task.FromResult(observers);
        }
    }
}

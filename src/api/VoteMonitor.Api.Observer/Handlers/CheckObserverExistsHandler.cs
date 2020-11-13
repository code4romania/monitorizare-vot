using System;
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
    public class CheckObserverExistsHandler : IRequestHandler<GetObserverDetails, ObserverModel>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CheckObserverExistsHandler(VoteMonitorContext context,IMapper mapper, ILogger<CheckObserverExistsHandler> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ObserverModel> Handle(GetObserverDetails request, CancellationToken cancellationToken)
        {
            try
            {
                var observer = await _context.Observers.FirstOrDefaultAsync(p => p.Id == request.ObserverId && p.IdNgo == request.NgoId, cancellationToken);

                return _mapper.Map<ObserverModel>(observer);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving observer: ", ex);
                throw;
            }
        }
    }
}
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Utils;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers
{
    public class GenerateObserversHandler : IRequestHandler<ObserverGenerateCommand, List<GeneratedObserver>>
    {
        private readonly VoteMonitorContext _voteMonitorContext;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IHashService _hashService;

        public GenerateObserversHandler(VoteMonitorContext voteMonitorContext, ILogger<GenerateObserversHandler> logger, IMapper mapper, IHashService hashService)
        {
            _voteMonitorContext = voteMonitorContext;
            _logger = logger;
            _mapper = mapper;
            _hashService = hashService;
        }

        public async Task<List<GeneratedObserver>> Handle(ObserverGenerateCommand request, CancellationToken cancellationToken)
        {
            List<Entities.Observer> dbObservers = new List<Entities.Observer>();
            List<GeneratedObserver> generatedObservers = new List<GeneratedObserver>();


            for (int i = 0; i < request.NumberOfObservers; ++i)
            {
                dbObservers.Add(RandomObserverBuilder.Instance(_hashService).Build(request.NgoId));
            }

            try
            {
                using (var tran = await _voteMonitorContext.Database.BeginTransactionAsync(cancellationToken))
                {
                    int latestId = _voteMonitorContext.Observers.Count() > 0 ?
                                    _voteMonitorContext.Observers
                                    .OrderByDescending(o => o.Id)
                                    .First()
                                    .Id
                                    : 0;

                    dbObservers = dbObservers
                        .Select(o => { o.Id = ++latestId; return o; })
                        .ToList();

                    _voteMonitorContext.Observers.AddRange(dbObservers.ToArray());
                    await _voteMonitorContext.SaveChangesAsync(cancellationToken);
                    tran.Commit();

                    return dbObservers
                        .Select(o => _mapper.Map<GeneratedObserver>(o))
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during generation of random observers", ex, ex.Message);
            }

            return await Task.FromResult(generatedObservers);
        }
    }
}
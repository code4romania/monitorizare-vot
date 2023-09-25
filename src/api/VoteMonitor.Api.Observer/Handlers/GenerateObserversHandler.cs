using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Utils;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Observer.Handlers;

public class GenerateObserversHandler : IRequestHandler<ObserverGenerateCommand, List<GeneratedObserver>>
{
    private readonly VoteMonitorContext _voteMonitorContext;
    private readonly ILogger _logger;
    private readonly IHashService _hashService;

    public GenerateObserversHandler(VoteMonitorContext voteMonitorContext, ILogger<GenerateObserversHandler> logger, IHashService hashService)
    {
        _voteMonitorContext = voteMonitorContext;
        _logger = logger;
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
                    .Select(o => new GeneratedObserver()
                    {
                        Id = o.Id.ToString(),
                        Pin = o.Pin,
                        PhoneNumber = o.Phone
                    })
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during generation of random observers");
        }

        return await Task.FromResult(generatedObservers);
    }
}

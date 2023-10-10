using MediatR;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Core.Services;

namespace VoteMonitor.Api.Core.Handlers;

public class CacheHandler : IRequestHandler<ClearCache>
{
    private readonly ICacheService _cacheService;

    public CacheHandler(ICacheService cacheService)
    {
        _cacheService = cacheService;
    }
    public async Task Handle(ClearCache request, CancellationToken cancellationToken)
    {
        await _cacheService.ClearAllValuesAsync();
    }
}

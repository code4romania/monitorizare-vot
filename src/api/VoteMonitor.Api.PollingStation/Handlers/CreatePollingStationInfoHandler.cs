using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers;

public class CreatePollingStationInfoHandler : IRequestHandler<CreatePollingStationInfo>
{
    private readonly VoteMonitorContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CreatePollingStationInfoHandler(VoteMonitorContext context, IMapper mapper, ILogger<CreatePollingStationInfoHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task Handle(CreatePollingStationInfo request, CancellationToken cancellationToken)
    {
        try
        {
            var pollingStationInfo = _mapper.Map<PollingStationInfo>(request);

            _context.PollingStationInfos.Add(pollingStationInfo);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating polling station info: ");
            throw;
        }
    }
}

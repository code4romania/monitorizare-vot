using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Ngo.Models;
using VoteMonitor.Api.Ngo.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Ngo.Handlers;

public class NgoQueryHandler : IRequestHandler<GetAllNgos, Result<List<NgoModel>>>,
    IRequestHandler<GetNgoDetails, Result<NgoModel>>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger<NgoQueryHandler> _logger;

    public NgoQueryHandler(VoteMonitorContext context, ILogger<NgoQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<NgoModel>>> Handle(GetAllNgos request, CancellationToken cancellationToken)
    {
        try
        {
            var listAsync = await _context.Ngos.Select(x => ToModel(x)).ToListAsync(cancellationToken);
            return Result.Success(listAsync);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when loading all ngos");
            return Result.Failure<List<NgoModel>>("Could not load ngos.");
        }
    }

    public async Task<Result<NgoModel>> Handle(GetNgoDetails request, CancellationToken cancellationToken)
    {
        try
        {
            var ngo = await _context.Ngos.FirstOrDefaultAsync(x => x.Id == request.NgoId, cancellationToken);
            if (ngo == null)
            {
                return Result.Failure<NgoModel>($"Could not find ngo with id {request.NgoId}");
            }

            var mappedNgo = ToModel(ngo);
            return Result.Success(mappedNgo);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when loading all ngos");
            return Result.Failure<NgoModel>($"Error when loading info for ngo with id {request.NgoId}");
        }
    }

    private static NgoModel ToModel(Entities.Ngo x)
    {
        return new NgoModel
        {
            Id = x.Id,
            Name = x.Name,
            IsActive = x.IsActive,
            Organizer = x.Organizer,
            ShortName = x.ShortName
        };
    }
}

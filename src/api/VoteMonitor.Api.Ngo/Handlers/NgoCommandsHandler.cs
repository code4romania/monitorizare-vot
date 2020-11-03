using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Ngo.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Ngo.Handlers
{
    public class NgoCommandsHandler : IRequestHandler<CreateNgo, Result>
        , IRequestHandler<UpdateNgo, Result>
        , IRequestHandler<DeleteNgo, Result>
        , IRequestHandler<SetNgoStatusFlag, Result>
        , IRequestHandler<SetNgoOrganizerFlag, Result>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<NgoCommandsHandler> _logger;

        public NgoCommandsHandler(ILogger<NgoCommandsHandler> logger, IMapper mapper, VoteMonitorContext context)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
        }

        public async Task<Result> Handle(CreateNgo request, CancellationToken cancellationToken)
        {
            try
            {
                var newNgo = _mapper.Map<Entities.Ngo>(request.Ngo);
                var maxNgoId = await _context.Ngos.MaxAsync(x => x.Id, cancellationToken);
                newNgo.Id = maxNgoId + 1;

                await _context.Ngos.AddAsync(newNgo, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not create ngo", request.Ngo);
                return Result.Failure("Could not create ngo");
            }
        }

        public async Task<Result> Handle(UpdateNgo request, CancellationToken cancellationToken)
        {
            try
            {
                var ngo = await _context.Ngos.FirstOrDefaultAsync(x => x.Id == request.Ngo.Id, cancellationToken);
                if (ngo == null)
                {
                    return Result.Failure($"could not find ngo with id {request.Ngo.Id}");
                }

                ngo.Name = request.Ngo.Name;
                ngo.ShortName = request.Ngo.ShortName;
                ngo.IsActive = request.Ngo.IsActive;
                ngo.Organizer = request.Ngo.Organizer;

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update ngo", request.Ngo);
                return Result.Failure($"Could not update ngo with id = {request.Ngo.Id}");
            }
        }

        public async Task<Result> Handle(DeleteNgo request, CancellationToken cancellationToken)
        {
            try
            {
                var ngo = await _context.Ngos.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (ngo == null)
                {
                    return Result.Failure($"Could not find ngo with id = {request.Id}");
                }
                _context.Ngos.Remove(ngo);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete ngo", request.Id);
                return Result.Failure($"Could not delete ngo with id = {request.Id}");
            }
        }

        public async Task<Result> Handle(SetNgoStatusFlag request, CancellationToken cancellationToken)
        {
            try
            {
                var ngo = await _context.Ngos.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (ngo == null)
                {
                    return Result.Failure($"Could not find ngo with id = {request.Id}");
                }
                ngo.IsActive = request.IsActive;
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update status for ngo", request.Id);
                return Result.Failure($"Could not update status of ngo with id = {request.Id}");
            }
        }

        public async Task<Result> Handle(SetNgoOrganizerFlag request, CancellationToken cancellationToken)
        {
            try
            {
                var ngo = await _context.Ngos.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                if (ngo == null)
                {
                    return Result.Failure($"Could not find ngo with id = {request.Id}");
                }

                ngo.Organizer = request.IsOrganizer;
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update organizer flag for ngo", request.Id);
                return Result.Failure($"Could not update organizer flag for ngo with id = {request.Id}");
            }
        }
    }
}
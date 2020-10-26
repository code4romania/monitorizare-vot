using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Ngo.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Ngo.Handlers
{
    public class NgoAdminCommandsHandler : IRequestHandler<CreateNgoAdmin, Result>
        , IRequestHandler<UpdateNgoAdmin, Result>
        , IRequestHandler<DeleteNgoAdmin, Result>

    {
        private readonly VoteMonitorContext _context;
        private readonly IHashService _hashService;
        private readonly ILogger<NgoAdminCommandsHandler> _logger;

        public NgoAdminCommandsHandler(ILogger<NgoAdminCommandsHandler> logger, VoteMonitorContext context, IHashService hashService)
        {
            _logger = logger;
            _context = context;
            _hashService = hashService;
        }

        public async Task<Result> Handle(CreateNgoAdmin request, CancellationToken cancellationToken)
        {
            try
            {
                var maxNgoAdminId = await _context.NgoAdmins.MaxAsync(x => x.Id, cancellationToken);
                var ngoAdmin = new Entities.NgoAdmin()
                {
                    Id = maxNgoAdminId + 1,
                    Account = request.NgoAdmin.Account,
                    IdNgo = request.NgoAdmin.IdNgo,
                    Password = _hashService.GetHash(request.NgoAdmin.Password)
                };

                await _context.NgoAdmins.AddAsync(ngoAdmin, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not create ngo admin", request.NgoAdmin);
                return Result.Failure("Could not create ngo admin");
            }
        }

        public async Task<Result> Handle(UpdateNgoAdmin request, CancellationToken cancellationToken)
        {
            try
            {
                var ngoAdmin = await _context.NgoAdmins.FirstOrDefaultAsync(x => x.Id == request.NgoAdmin.Id && x.IdNgo == request.NgoAdmin.IdNgo, cancellationToken);
                if (ngoAdmin == null)
                {
                    return Result.Failure($"could not find ngo with id {request.NgoAdmin.Id}");
                }

                ngoAdmin.Account = request.NgoAdmin.Account;
                ngoAdmin.Password = _hashService.GetHash(request.NgoAdmin.Password);

                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not update ngo admin", request.NgoAdmin);
                return Result.Failure($"Could not update ngo admin with id = {request.NgoAdmin.Id} and idNgo = {request.NgoAdmin.IdNgo}");
            }
        }

        public async Task<Result> Handle(DeleteNgoAdmin request, CancellationToken cancellationToken)
        {
            try
            {
                var ngoAdmin = await _context.NgoAdmins.FirstOrDefaultAsync(x => x.Id == request.IdNgoAdmin && x.IdNgo == request.IdNgo, cancellationToken);
                if (ngoAdmin == null)
                {
                    return Result.Failure($"Could not find ngo admin with id = {request.IdNgoAdmin} and idNgo = {request.IdNgo}");
                }
                _context.NgoAdmins.Remove(ngoAdmin);
                await _context.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not delete ngo admin", request.IdNgo, request.IdNgoAdmin);
                return Result.Failure($"Could not delete ngo admin with id = {request.IdNgoAdmin} and ngoId = {request.IdNgo}");
            }
        }
    }
}
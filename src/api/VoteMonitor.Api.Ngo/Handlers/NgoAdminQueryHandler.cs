using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Ngo.Models;
using VoteMonitor.Api.Ngo.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Ngo.Handlers
{
    public class NgoAdminQueryHandler : IRequestHandler<GetAllNgoAdmins, Result<List<NgoAdminModel>>>,
        IRequestHandler<GetNgoAdminDetails, Result<NgoAdminModel>>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger<NgoAdminQueryHandler> _logger;

        public NgoAdminQueryHandler(VoteMonitorContext context, ILogger<NgoAdminQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<List<NgoAdminModel>>> Handle(GetAllNgoAdmins request, CancellationToken cancellationToken)
        {
            try
            {
                var listAsync = await _context.NgoAdmins
                    .Where(x => x.IdNgo == request.IdNgo)
                    .Select(x => new NgoAdminModel
                    {
                        Id = x.Id,
                        Account = x.Account,
                        IdNgo = x.IdNgo
                    }).ToListAsync(cancellationToken);

                return Result.Success(listAsync);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when loading all ngo admins");
                return Result.Failure<List<NgoAdminModel>>("Could not load ngo admins.");
            }
        }

        public async Task<Result<NgoAdminModel>> Handle(GetNgoAdminDetails request, CancellationToken cancellationToken)
        {
            try
            {
                var ngoAdmin = await _context.NgoAdmins.FirstOrDefaultAsync(x => x.Id == request.AdminId && x.IdNgo == request.NgoId, cancellationToken);
                if (ngoAdmin == null)
                {
                    return Result.Failure<NgoAdminModel>($"Could not find ngo admin with id = {request.AdminId} and in ngo with id = {request.NgoId}");
                }

                var model = new NgoAdminModel()
                {
                    Id = ngoAdmin.Id,
                    Account = ngoAdmin.Account,
                    IdNgo = ngoAdmin.IdNgo
                };

                return Result.Success(model);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when loading all ngo admins", request.AdminId, request.NgoId);
                return Result.Failure<NgoAdminModel>($"Could not load info for ngo admin with id {request.AdminId} and in ngo with id = {request.NgoId}");
            }
        }
    }
}
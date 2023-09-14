using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Queries;

public record GetNgoAdminDetails(int NgoId, int AdminId) : IRequest<Result<NgoAdminModel>>;

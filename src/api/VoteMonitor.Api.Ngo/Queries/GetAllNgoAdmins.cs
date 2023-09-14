using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Queries;

public record GetAllNgoAdmins(int IdNgo) : IRequest<Result<List<NgoAdminModel>>>;

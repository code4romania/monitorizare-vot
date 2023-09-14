using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Queries;

public record GetNgoDetails(int NgoId) :IRequest<Result<NgoModel>>;

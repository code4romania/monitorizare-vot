using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Queries;

public record GetAllNgos : IRequest<Result<List<NgoModel>>>;

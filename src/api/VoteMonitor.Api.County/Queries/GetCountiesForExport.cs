using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetCountiesForExport : IRequest<Result<List<CountyCsvModel>>>;

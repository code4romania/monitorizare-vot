using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetCountyById(int CountyId) : IRequest<Result<CountyModel>>;

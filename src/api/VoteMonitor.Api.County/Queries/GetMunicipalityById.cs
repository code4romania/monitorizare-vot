using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetMunicipalityById(int MunicipalityId) : IRequest<Result<MunicipalityModel>>;

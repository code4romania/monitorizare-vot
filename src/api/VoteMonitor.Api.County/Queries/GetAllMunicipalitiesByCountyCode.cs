using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetAllMunicipalitiesByCountyCode(string CountyCode) : IRequest<Result<List<MunicipalityModel>>>;

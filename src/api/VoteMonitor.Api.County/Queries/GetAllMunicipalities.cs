using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetAllMunicipalities(string CountyCode) : IRequest<Result<List<MunicipalityModel>>>;

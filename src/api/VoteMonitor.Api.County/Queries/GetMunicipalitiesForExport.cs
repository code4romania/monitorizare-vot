using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetMunicipalitiesForExport : IRequest<Result<List<MunicipalityCsvModel>>>;

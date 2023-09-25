using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetProvincesForExport : IRequest<Result<List<ProvinceCsvModel>>>;

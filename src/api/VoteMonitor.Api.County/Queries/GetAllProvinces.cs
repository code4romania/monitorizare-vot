using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetAllProvinces : IRequest<Result<List<ProvinceModel>>>;

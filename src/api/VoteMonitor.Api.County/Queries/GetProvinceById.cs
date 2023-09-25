using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetProvinceById(int ProvinceId) : IRequest<Result<ProvinceModel>>;

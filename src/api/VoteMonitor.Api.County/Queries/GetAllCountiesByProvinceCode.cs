using CSharpFunctionalExtensions;
using MediatR;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Queries;

public record GetAllCountiesByProvinceCode(string ProvinceCode) : IRequest<Result<List<CountyModel>>>;

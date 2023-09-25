using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.County.Commands;

public record CreateOrUpdateProvinces(IFormFile File) : IRequest<Result>;

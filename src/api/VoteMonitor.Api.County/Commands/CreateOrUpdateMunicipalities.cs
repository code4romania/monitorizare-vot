using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.County.Commands;

public record CreateOrUpdateMunicipalities(IFormFile File) : IRequest<Result>;

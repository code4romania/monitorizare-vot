using MediatR;
using Microsoft.AspNetCore.Http;
using VoteMonitor.Api.Location.Models.ResultValues;

namespace VoteMonitor.Api.Location.Commands;

public record ImportPollingStationsCommand(IFormFile File) : IRequest<PollingStationImportResultValue>;

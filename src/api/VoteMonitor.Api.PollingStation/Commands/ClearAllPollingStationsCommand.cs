using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.PollingStation.Commands;

public record ClearAllPollingStationsCommand(bool IncludeRelatedData) : IRequest<Result>;

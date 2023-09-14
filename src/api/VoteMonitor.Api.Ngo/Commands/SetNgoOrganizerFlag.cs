using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Ngo.Commands;

public record SetNgoOrganizerFlag(int Id, bool IsOrganizer) : IRequest<Result>;

using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Ngo.Commands;

public record SetNgoStatusFlag(int Id, bool IsActive) : IRequest<Result>;

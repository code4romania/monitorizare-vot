using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Ngo.Commands;

public record DeleteNgo(int Id) : IRequest<Result>;

using CSharpFunctionalExtensions;
using MediatR;

namespace VoteMonitor.Api.Ngo.Commands;

public record DeleteNgoAdmin(int IdNgo, int IdNgoAdmin) : IRequest<Result>;

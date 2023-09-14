using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Observer.Commands;

public record ImportObserversRequest(int NgoId, IFormFile File) : IRequest<int>;

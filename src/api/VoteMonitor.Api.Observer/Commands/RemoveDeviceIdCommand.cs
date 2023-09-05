using MediatR;

namespace VoteMonitor.Api.Observer.Commands;

public class RemoveDeviceIdCommand : IRequest
{
    public int Id { get; set; }
}
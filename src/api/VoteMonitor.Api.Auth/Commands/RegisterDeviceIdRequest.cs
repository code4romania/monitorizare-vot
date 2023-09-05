using MediatR;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Commands;

public class RegisterDeviceId : IRequest<int>
{
    public string MobileDeviceId { get; set; }
    public MobileDeviceIdType MobileDeviceIdType { get; set; }
    public int ObserverId { get; set; }
}
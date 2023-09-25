using MediatR;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Commands;

public record RegisterDeviceId(string MobileDeviceId, MobileDeviceIdType MobileDeviceIdType, int ObserverId) : IRequest<int>;

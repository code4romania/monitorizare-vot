using MediatR;

namespace VotingIrregularities.Domain.UserAggregate
{
    public class RegisterDeviceId : IRequest<int>
    {
        public string MobileDeviceId { get; set; }
        public int ObserverId { get; set; }
    }
}

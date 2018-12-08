using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Domain.UserAggregate
{
    public class RegisterDeviceCommand : IRequest<int>
    {
        public string MobileDeviceId { get; set; }
        public int ObserverId { get; set; }
    }
}

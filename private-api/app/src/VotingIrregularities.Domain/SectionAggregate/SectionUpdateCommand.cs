using System;
using MediatR;

namespace VotingIrregularities.Domain.SectionAggregate
{
    public class SectionUpdateCommand : IRequest<int>
    {
        public int ObserverId { get; set; }
        public int VotingSectionId { get; set; }
        public DateTime LeaveTime { get; set; }
    }
}

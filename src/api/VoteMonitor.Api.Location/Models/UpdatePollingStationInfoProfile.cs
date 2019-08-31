using AutoMapper;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Api.Location.Queries;
using VotingIrregularities.Domain.SectieAggregate;

namespace VoteMonitor.Api.Location.Models
{
    public class UpdatePollingStationInfoProfile : Profile
    {
        public UpdatePollingStationInfoProfile()
        {
            CreateMap<UpdatePollingStationInfo, PollingStationQuery>();
            CreateMap<UpdatePollingStationInfo, UpdatePollingSectionCommand>();
        }
    }
}

using AutoMapper;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Note.Models;

namespace VoteMonitor.Api.Note.Profiles
{
    public class PollingStationQueryProfile : Profile
    {
        public PollingStationQueryProfile()
        {
            CreateMap<NoteModel, PollingStationQuery>()
                .ForMember(dest => dest.CountyCode, c => c.MapFrom(y => y.CountyCode))
                .ForMember(dest => dest.IdPollingStation, c => c.MapFrom(y => y.PollingStationNumber));
        }
    }
}

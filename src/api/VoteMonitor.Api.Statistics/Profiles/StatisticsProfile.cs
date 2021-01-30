using AutoMapper;
using VoteMonitor.Api.Statistics.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Statistics.Profiles
{
    public class StatisticsProfile : Profile
    {
        public StatisticsProfile()
        {
            CreateMap<ComposedStatistics, SimpleStatisticsModel>()
                .ForMember(dest => dest.Label, c => c.MapFrom(src => $"Sectia {src.Code} {src.Label}"))
                .ForMember(dest => dest.Value, c => c.MapFrom(src => src.Value.ToString()));

            CreateMap<SimpleStatistics, SimpleStatisticsModel>()
                .ForMember(dest => dest.Value, c => c.MapFrom(src => src.Value.ToString()));
        }
    }
}
using AutoMapper;
using VoteMonitor.Api.Core;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Statistics.Models
{
    public class SimpleStatisticsFilter : PagingModel
    {
        public string FormCode { get; set; }
        public StatisticsGroupingTypes GroupingType { get; set; }
    }

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

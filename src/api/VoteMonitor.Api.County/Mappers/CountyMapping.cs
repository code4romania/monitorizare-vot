using AutoMapper;
using VoteMonitor.Api.County.Models;

namespace VoteMonitor.Api.County.Mappers
{
    public class CountyMapping : Profile
    {
        public CountyMapping()
        {
            _ = CreateMap<Entities.County, CountyModel>()
                .ForMember(dest => dest.Id, x => x.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, x => x.MapFrom(src => src.Code))
                .ForMember(dest => dest.Diaspora, x => x.MapFrom(src => src.Diaspora))
                .ForMember(dest => dest.Name, x => x.MapFrom(src => src.Name))
                .ForMember(dest => dest.NumberOfPollingStations, x => x.MapFrom(src => src.NumberOfPollingStations))
                .ForMember(dest => dest.Order, x => x.MapFrom(src => src.Order));

            _ = CreateMap<Entities.County, CountyCsvModel>()
                .ForMember(dest => dest.Id, x => x.MapFrom(src => src.Id))
                .ForMember(dest => dest.Code, x => x.MapFrom(src => src.Code))
                .ForMember(dest => dest.Diaspora, x => x.MapFrom(src => src.Diaspora))
                .ForMember(dest => dest.Name, x => x.MapFrom(src => src.Name))
                .ForMember(dest => dest.NumberOfPollingStations, x => x.MapFrom(src => src.NumberOfPollingStations))
                .ForMember(dest => dest.Order, x => x.MapFrom(src => src.Order));
        }
    }
}

using AutoMapper;
using VoteMonitor.Api.Observer.Commands;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Queries;

namespace VoteMonitor.Api.Observer.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<ActiveObserverFilter, ActiveObserversQuery>();
            CreateMap<ActiveObserversQuery, ActiveObserverFilter>();

            CreateMap<Entities.Observer, ObserverModel>()
              .ForMember(dest => dest.Ngo, c => c.MapFrom(src => src.Ngo.Name))
              .ForMember(dest => dest.NumberOfNotes, c => c.MapFrom(src => src.Notes.Count))
              .ForMember(dest => dest.NumberOfPollingStations, c => c.MapFrom(src => src.PollingStationInfos.Count));

            CreateMap<Entities.Observer, GeneratedObserver>()
                .ForMember(dest => dest.Id, c => c.MapFrom(src => src.Id))
                .ForMember(dest => dest.Pin, c => c.MapFrom(src => src.Pin))
                .ForMember(dest => dest.PhoneNumber, c => c.MapFrom(src => src.Phone));

            CreateMap<NewObserverModel, NewObserverCommand>()
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Phone, c => c.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Pin, c => c.MapFrom(src => src.Pin));

            CreateMap<RemoveDeviceIdModel, RemoveDeviceIdCommand>();

            CreateMap<ObserverListQuery, ObserverListCommand>()
                .ForMember(dest => dest.Number, c => c.MapFrom(src => src.Number))
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Page, c => c.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, c => c.MapFrom(src => src.PageSize));

            CreateMap<EditObserverModel, EditObserverCommand>();

        }
    }
}

using AutoMapper;

using VoteMonitor.Api.Observer.Commands;

namespace VoteMonitor.Api.Observer.Models
{
    public class DeleteObserverModel
    {
        public int IdObserver { get; set; }
    }

    public class ObserverMapperProfile : Profile
    {
        public ObserverMapperProfile()
        {
           CreateMap<DeleteObserverModel, DeleteObserverCommand>()
                .ForMember(dest => dest.IdObserver, c => c.MapFrom(src => src.IdObserver))
                ;
        }
    }
}

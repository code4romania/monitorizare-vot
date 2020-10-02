using AutoMapper;
using MediatR;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Commands
{
    public class NewObserverCommand : IRequest<int>
    {
        public int IdNgo { get; set; }
        public string Phone { get; set; }
        public string Pin { get; set; }
        public string Name { get; set; }
        public bool SendSMS { get; set; }
    }
    public class ObserverProfile : Profile
    {
        public ObserverProfile()
        {
            CreateMap<NewObserverModel, NewObserverCommand>()
                .ForMember(dest => dest.Name, c => c.MapFrom(src => src.Name))
                .ForMember(dest => dest.Phone, c => c.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Pin, c => c.MapFrom(src => src.Pin))
               ;
        }
    }

}

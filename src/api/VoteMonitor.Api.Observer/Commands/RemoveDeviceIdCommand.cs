using MediatR;
using AutoMapper;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Commands
{
    public class RemoveDeviceIdCommand : IRequest<bool>
    {
        public int IdObserver { get; set; }
    }

    public class RemoveDeviceIdProfile : Profile
    {
        public RemoveDeviceIdProfile()
        {
            CreateMap<RemoveDeviceIdModel, RemoveDeviceIdCommand>()
                .ForMember(dest => dest.IdObserver, c => c.MapFrom(src => src.IdObserver))
                ;
        }
    }
}
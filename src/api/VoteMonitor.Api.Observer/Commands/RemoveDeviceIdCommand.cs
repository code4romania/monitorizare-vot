using MediatR;
using AutoMapper;
using VoteMonitor.Api.Observer.Models;

namespace VoteMonitor.Api.Observer.Commands
{
    public class RemoveDeviceIdCommand : IRequest
    {
        public int Id { get; set; }
    }
    
    public class RemoveDeviceIdProfile : Profile
    {
        public RemoveDeviceIdProfile()
        {
            CreateMap<RemoveDeviceIdModel, RemoveDeviceIdCommand>();
        }
    }
}
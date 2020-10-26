using AutoMapper;
using MediatR;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Notification.Models;
using VoteMonitor.Api.Notification.Queries;

namespace VoteMonitor.Api.Notification.Commands
{
    public class NotificationListCommand : IRequest<ApiListResponse<NotificationModel>>
    {
        public bool Organizer { get; set; }
        public int? IdNgo { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class NotificationListCommandProfile : Profile
    {
        public NotificationListCommandProfile()
        {
            CreateMap<NotificationListQuery, NotificationListCommand>()
                .ForMember(dest => dest.IdNgo, c => c.MapFrom(src => src.IdNgo))
                .ForMember(dest => dest.Organizer, c => c.MapFrom(src => src.Organizer))
                .ForMember(dest => dest.Page, c => c.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, c => c.MapFrom(src => src.PageSize))
               ;
        }
    }
}

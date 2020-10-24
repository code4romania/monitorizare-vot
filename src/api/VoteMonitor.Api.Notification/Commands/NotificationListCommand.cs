using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Notification.Models;
using VoteMonitor.Api.Notification.Queries;

namespace VoteMonitor.Api.Notification.Commands
{
    public class NotificationListCommand : IRequest<ApiListResponse<NotificationModel>>
    {
        public UserType UserType { get; set; }
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
                .ForMember(dest => dest.UserType, c => c.MapFrom(src => src.UserType))
                .ForMember(dest => dest.Page, c => c.MapFrom(src => src.Page))
                .ForMember(dest => dest.PageSize, c => c.MapFrom(src => src.PageSize))
               ;
        }
    }
}

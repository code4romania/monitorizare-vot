using System;
using System.Linq;
using AutoMapper;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Api.Notification.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Notification.Profiles
{
    public class RequestNotificationMapperProfile : Profile
    {
        public RequestNotificationMapperProfile()
        {
            CreateMap<NotificationRegistrationDataModel, NotificationRegistrationDataCommand>();

            CreateMap<SendNotificationCommand, Entities.Notification>()
                .ForMember(dest => dest.Body, c => c.MapFrom(src => src.Message))
                .ForMember(dest => dest.NotificationRecipients,
                    c => c.MapFrom(src =>
                        src.Recipients.Select(r => new NotificationRecipient { ObserverId = r }).ToList()))
                .ForMember(dest => dest.InsertedAt, c => c.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.SenderAdminId, c => c.MapFrom(src => src.SenderAdminId));

            CreateMap<SendNotificationToAllCommand, Entities.Notification>()
                .ForMember(dest => dest.Body, c => c.MapFrom(src => src.Message));


            CreateMap<SendNotificationModel, SendNotificationCommand>();
            CreateMap<SendNotificationToAllModel, SendNotificationToAllCommand>();
        }
    }
}

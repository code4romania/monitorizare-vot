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
            CreateMap<SendNotificationCommand, Entities.Notification>()
                .ForMember(dest => dest.Body, c => c.MapFrom(src => src.Message))
                .ForMember(dest => dest.NotificationRecipients,
                    c => c.MapFrom(src =>
                        src.Recipients.Select(r => new NotificationRecipient { ObserverId = r }).ToList()))
                .ForMember(dest => dest.InsertedAt, c => c.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.SenderAdminId, c => c.MapFrom(src => src.SenderAdminId));


            CreateMap<NotificationRegistrationDataModel, NotificationRegistrationDataCommand>()
                .ForMember(dest => dest.ObserverId, c => c.MapFrom(src => src.ObserverId))
                .ForMember(dest => dest.ChannelName, c => c.MapFrom(src => src.ChannelName))
                .ForMember(dest => dest.Token, c => c.MapFrom(src => src.Token));

            CreateMap<SendNotificationModel, SendNotificationCommand>();
        }
    }
}

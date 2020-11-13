using AutoMapper;
using System;
using System.Linq;
using VoteMonitor.Api.Core.Commands;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Notification.Models
{
    public class RequestNotificationMapperProfile : Profile
    {
        public RequestNotificationMapperProfile()
        {
            CreateMap<NewNotificationCommand, Entities.Notification>()
                .ForMember(dest => dest.Body, c => c.MapFrom(src => src.Message))
                .ForMember(dest => dest.NotificationRecipients,
                    c => c.MapFrom(src =>
                        src.Recipients.Select(r => new NotificationRecipient { ObserverId = int.Parse(r) }).ToList()))
                .ForMember(dest => dest.InsertedAt, c => c.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.SenderAdminId, c => c.MapFrom(src => src.SenderAdminId));


            CreateMap<NotificationRegistrationDataModel, NotificationRegistrationDataCommand>()
                .ForMember(dest => dest.ObserverId, c => c.MapFrom(src => src.ObserverId))
                .ForMember(dest => dest.ChannelName, c => c.MapFrom(src => src.ChannelName))
                .ForMember(dest => dest.Token, c => c.MapFrom(src => src.Token));

            CreateMap<NotificationNewModel, NewNotificationCommand>()
                .ForMember(dest => dest.Channel, c => c.MapFrom(src => src.Channel))
                .ForMember(dest => dest.From, c => c.MapFrom(src => src.From))
                .ForMember(dest => dest.Title, c => c.MapFrom(src => src.Title))
                .ForMember(dest => dest.Message, c => c.MapFrom(src => src.Message))
                .ForMember(dest => dest.Recipients, c => c.MapFrom(src => src.Recipients));
        }
    }
}
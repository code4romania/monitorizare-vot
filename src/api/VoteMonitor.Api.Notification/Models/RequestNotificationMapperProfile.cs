using AutoMapper;
using System;
using System.Linq;
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
                ;
        }
    }
}
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoteMonitor.Api.Notification.Commands;

namespace VoteMonitor.Api.Notification.Models
{
    public class RequestNotificationMapperProfile : Profile
    {
        public RequestNotificationMapperProfile()
        {
            CreateMap<NewNotificationCommand, Entities.Notification>()
                .ConstructUsing((NewNotificationCommand request) =>
                {
                    return new Entities.Notification
                    {
                        Body = request.Message,
                        Title = request.Title,
                        Channel = request.Channel,
                        InsertedAt = DateTime.Now,
                        NotificationRecipients = 
                            request.Recipients.Select(r => new Entities.NotificationRecipient
                            {
                                ObserverId = int.Parse(r)
                            }).ToList()
                    };
                })
                ;
        }
        }
}

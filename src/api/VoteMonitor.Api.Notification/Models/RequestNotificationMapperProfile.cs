using System;
using System.Linq;
using AutoMapper;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Notification.Models
{
    public class RequestNotificationMapperProfile : Profile
    {
        public RequestNotificationMapperProfile()
        {
            CreateMap<NewNotificationCommand, Entities.Notification>()
                .ConstructUsing(request =>
                {
                    return new Entities.Notification
                    {
                        Body = request.Message,
                        Title = request.Title,
                        Channel = request.Channel,
                        InsertedAt = DateTime.Now,
                        NotificationRecipients = 
                            request.Recipients.Select(r => new NotificationRecipient
                            {
                                ObserverId = int.Parse(r)
                            }).ToList()
                    };
                })
                ;
        }
        }
}

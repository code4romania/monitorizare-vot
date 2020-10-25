using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VoteMonitor.Api.Notification.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Channel { get; set; }
        public string Body { get; set; }
        public DateTime InsertedAt { get; set; }
        //
        public int SenderId { get; set; }
        public int SenderIdNgo { get; set; }
        public string SenderNgoName { get; set; }
        public string SenderAccount { get; set; }
        //
        public ICollection<int> SentObserverIds { get; set; }
    }

    public class NotificationModelProfile : Profile
    {
        public NotificationModelProfile()
        {
            CreateMap<Entities.Notification, NotificationModel>()
                .ForMember(dest => dest.Id, c => c.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, c => c.MapFrom(src => src.Title))
                .ForMember(dest => dest.Channel, c => c.MapFrom(src => src.Channel))
                .ForMember(dest => dest.Body, c => c.MapFrom(src => src.Body))
                .ForMember(dest => dest.InsertedAt, c => c.MapFrom(src => src.InsertedAt))
                .ForMember(dest => dest.SenderId, c => c.MapFrom(src => src.SenderAdmin.Id))
                .ForMember(dest => dest.SenderIdNgo, c => c.MapFrom(src => src.SenderAdmin.IdNgo))
                .ForMember(dest => dest.SenderNgoName, c => c.MapFrom(src => src.SenderAdmin.Ngo.Name))
                .ForMember(dest => dest.SenderAccount, c => c.MapFrom(src => src.SenderAdmin.Account))
                .ForMember(dest => dest.SentObserverIds, c => c.MapFrom(src => src.NotificationRecipients.Select(o => o.ObserverId)));
        }
    }
}

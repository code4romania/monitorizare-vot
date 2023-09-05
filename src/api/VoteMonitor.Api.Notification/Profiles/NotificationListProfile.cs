using AutoMapper;
using VoteMonitor.Api.Notification.Commands;
using VoteMonitor.Api.Notification.Queries;

namespace VoteMonitor.Api.Notification.Profiles;

public class NotificationListProfile : Profile
{
    public NotificationListProfile()
    {
        CreateMap<NotificationListQuery, NotificationListCommand>();
    }
}
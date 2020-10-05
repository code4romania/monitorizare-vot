using AutoMapper;
using VoteMonitor.Api.Auth.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Auth.Profiles
{
    public class UserInfoProfile : Profile
    {
        public UserInfoProfile()
        {
            CreateMap<NgoAdmin, UserInfo>()
                .ForMember(u => u.Organizer, opt => opt.MapFrom(a => a.Ngo.Organizer));
        }
    }
}
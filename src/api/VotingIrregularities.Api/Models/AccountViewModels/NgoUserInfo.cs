using AutoMapper;
using VoteMonitor.Entities;

namespace VotingIrregularities.Api.Models.AccountViewModels
{
    public class NgoUserInfo
    {
        public int IdNgo { get; set; }
        public bool Organizer { get; set; }

        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }
	}

    public class UserInfoProfile : Profile
    {
        public UserInfoProfile()
        {
	        CreateMap<NgoAdmin, NgoUserInfo>()
		        .ForMember(u => u.IdNgo, opt => opt.MapFrom(a => a.IdNgo))
		        .ForMember(u => u.Organizer, opt => opt.MapFrom(a => a.Ngo.Organizer))
		        .ForMember(u => u.IsAuthenticated, opt => opt.MapFrom(a => false))
		        .ForMember(u => u.UserName, opt => opt.MapFrom(a => a.Account));
        }
    }
}

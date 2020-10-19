using AutoMapper;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Profiles
{
    public class OptionProfile : Profile
    {
        public OptionProfile()
        {
            CreateMap<OptionDTO, OptionModel>(MemberList.Source);
            CreateMap<OptionModel, OptionDTO>(MemberList.Source);

            CreateMap<Option, OptionDTO>(MemberList.Source);

            CreateMap<OptionDTO, Option>().ForMember(o => o.OptionsToQuestions, c => c.Ignore());

            CreateMap<CreateOptionModel, OptionDTO>()
                .ForMember(x => x.Id, c => c.Ignore());
        }
    }
}

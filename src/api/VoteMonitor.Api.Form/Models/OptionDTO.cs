using AutoMapper;
using VoteMonitor.Api.Form.Controllers;
using VoteMonitor.Api.Form.Models.Options;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Models
{
	public class OptionDto
	{
		public int Id { get; set; }
		public bool IsFreeText { get; set; }
		public string Text { get; set; }
		public string Hint { get; set; }
	}

	public class OptionProfile : Profile
	{
		public OptionProfile()
		{
			CreateMap<OptionDto, OptionModel>(MemberList.Source);
			CreateMap<OptionModel, OptionDto>(MemberList.Source);

			CreateMap<Option, OptionDto>(MemberList.Source);
			CreateMap<OptionDto, Option>(MemberList.Source);

			CreateMap<CreateOptionModel, OptionDto>()
				.ForMember(x => x.Id, c => c.Ignore())
				.ForMember(dest => dest.IsFreeText, c => c.MapFrom(src => src.IsFreeText))
				.ForMember(dest => dest.Text, c => c.MapFrom(src => src.Text))
				.ForMember(dest => dest.Hint, c => c.MapFrom(src => src.Hint));
		}
	}
}
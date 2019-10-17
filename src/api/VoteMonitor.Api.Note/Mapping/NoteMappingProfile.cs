using System;
using AutoMapper;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Note.Commands;
using VoteMonitor.Api.Note.Models;

namespace VoteMonitor.Api.Note.Mapping
{
	public class NoteMappingProfile : Profile
	{
		public NoteMappingProfile()
		{
			CreateMap<UploadNoteModel, PollingStationQuery>(MemberList.Destination)
				.ForMember(dest => dest.IdPollingStation, c => c.MapFrom(src => src.PollingStationNumber))
				.ForMember(dest => dest.CountyCode, c => c.MapFrom(src => src.CountyCode));

			CreateMap<UploadNoteModel, AddNoteCommand>(MemberList.Destination)
				.ForMember(dest => dest.IdQuestion, c => c.MapFrom(src => src.QuestionId == 0 || !src.QuestionId.HasValue ? null : src.QuestionId))
				.ForMember(dest => dest.Text, c => c.MapFrom(src => src.Text));


			CreateMap<AddNoteCommand, Entities.Note>(MemberList.Destination)
				.ForMember(dest => dest.AttachementPath, c => c.MapFrom(src => src.AttachementPath))
				.ForMember(dest => dest.IdQuestion, c => c.MapFrom(src => src.IdQuestion))
				.ForMember(dest => dest.IdObserver, c => c.MapFrom(src => src.IdObserver))
				.ForMember(dest => dest.IdPollingStation, c => c.MapFrom(src => src.IdPollingStation))
				.ForMember(dest => dest.Text, c => c.MapFrom(src => src.Text))
				.ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow));

		}
	}
}
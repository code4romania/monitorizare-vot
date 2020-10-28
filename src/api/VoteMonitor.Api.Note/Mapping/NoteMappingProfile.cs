using AutoMapper;
using System;
using System.Linq;
using VoteMonitor.Api.Location.Queries;
using VoteMonitor.Api.Note.Commands;
using VoteMonitor.Api.Note.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Note.Mapping
{
    public class NoteMappingProfile : Profile
    {
        public NoteMappingProfile()
        {
            CreateMap<UploadNoteModel, PollingStationQuery>(MemberList.Destination)
                .ForMember(dest => dest.IdPollingStation, c => c.MapFrom(src => src.PollingStationNumber));

            CreateMap<UploadNoteModel, AddNoteCommand>(MemberList.Destination)
                .ForMember(dest => dest.IdQuestion,
                    c => c.MapFrom(src => src.QuestionId == 0 || !src.QuestionId.HasValue ? null : src.QuestionId));

            CreateMap<AddNoteCommand, Entities.Note>(MemberList.Destination)
                .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Attachments, c => c.MapFrom(src => src.AttachementPaths.Select(x => new NotesAttachments()
                {
                    Path = x
                }).ToList()));
        }
    }
}
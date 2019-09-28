using AutoMapper;
using System;
using System.Linq;
using VoteMonitor.Api.Note.Commands;
using VoteMonitor.Api.Note.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Note.Profiles
{
    public class AddNoteCommandProfile : Profile
    {
        public AddNoteCommandProfile()
        {
            CreateMap<NoteModel, AddNoteCommand>()
                .ForMember(p => p.IdQuestion, src => src.MapFrom(y => y.QuestionId))
                .ForMember(p => p.IdPollingStation, src => src.MapFrom(y => y.PollingStationNumber))
                .ForMember(p => p.Text, src => src.MapFrom(y => y.Text));

            CreateMap<AddNoteCommand, Entities.Note>()
                .ForMember(dest => dest.IdQuestion, c => c.MapFrom(src =>
                    !src.IdQuestion.HasValue || src.IdQuestion.Value <= 0 ? null : src.IdQuestion)
                    )   
                .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.NoteAttachments , c => c.MapFrom(src => src.AttachementPaths.Select(blobUrl => new NoteAttachment(blobUrl))))
                .ForMember(dest => dest.IdObserver, c => c.MapFrom(src => src.IdObserver))
                .ForMember(dest => dest.IdPollingStation, c => c.MapFrom(src => src.IdPollingStation))
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Text)); ;
        }
    }
}

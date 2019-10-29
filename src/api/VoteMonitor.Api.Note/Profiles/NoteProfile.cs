using AutoMapper;
using System.Linq;
using VoteMonitor.Api.Note.Models;

namespace VoteMonitor.Api.Note.Profiles
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<Entities.Note, NoteModel>()
                .ForMember(x => x.CountyCode, opt => opt.MapFrom(y => y.PollingStation.County))
                .ForMember(x => x.FormCode, opt => opt.MapFrom(y => y.Question.FormSection.Form.Code))
                .ForMember(x => x.NoteAttachments, opt => opt.MapFrom(y => y.NoteAttachments.ToList().Select(x => x.NotePath)))
                .ForMember(x => x.PollingStationNumber, opt => opt.MapFrom(y => y.IdPollingStation))
                .ForMember(x => x.QuestionId, opt => opt.MapFrom(y => y.IdQuestion))
                .ForMember(x => x.Text, opt => opt.MapFrom(y => y.Text));
        }
    }
}

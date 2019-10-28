using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Note.Models;

namespace VoteMonitor.Api.Note.Commands
{
    public class AddNoteCommand : IRequest<NoteModel>
    {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public int? IdQuestion { get; set; }
        public string Text { get; set; }
        public List<string> AttachementPaths { get; set; }
    }
}

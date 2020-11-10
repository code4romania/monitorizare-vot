using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Note.Models;

namespace VoteMonitor.Api.Note.Queries
{
    public class NoteQueryV2 : IRequest<List<NoteModelV2>>
    {
        public int? IdPollingStation { get; set; }
        public int? IdObserver { get; set; }
        public int? IdQuestion { get; set; }
    }
}
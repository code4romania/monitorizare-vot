﻿using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Note.Models;

namespace VoteMonitor.Api.Note.Queries
{
    public class NoteQuery : IRequest<List<NoteModel>>
    {
        public int? IdPollingStation { get; set; }
        public int? IdObserver { get; set; }
        public int? IdQuestion { get; set; }
    }
}
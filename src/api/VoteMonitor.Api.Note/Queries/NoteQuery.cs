﻿using MediatR;
using System;
using System.Collections.Generic;
using VoteMonitor.Api.Note.Models;

namespace VoteMonitor.Api.Note.Queries
{
    [Obsolete("Will be removed when ui will use multiple files upload")]
    public class NoteQuery : IRequest<List<NoteModel>>
    {
        public int? IdPollingStation { get; set; }
        public int? IdObserver { get; set; }
        public int? IdQuestion { get; set; }
    }
}
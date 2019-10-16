﻿using MediatR;
using Microsoft.AspNetCore.Http;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ImportObserversRequest : IRequest<int> {
        public int IdOng { get; set; }
        public IFormFile File { get;set;}
    }
}

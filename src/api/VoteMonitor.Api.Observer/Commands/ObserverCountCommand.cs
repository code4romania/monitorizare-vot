using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Observer.Models;
using VoteMonitor.Api.Observer.Queries;

namespace VoteMonitor.Api.Observer.Commands {
    public class ObserverCountCommand : IRequest<int> {
        public int IdNgo { get; set; }
    }
}

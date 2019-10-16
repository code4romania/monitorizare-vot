using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoteMonitor.Api.Observer.Commands
{
    public class NewNotificationCommand : IRequest<int>
    {
        public string Message { get; set; }
        public List<string> recipients { get; set; }

        public string from;
    }
}

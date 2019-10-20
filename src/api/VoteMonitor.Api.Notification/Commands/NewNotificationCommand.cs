using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoteMonitor.Api.Observer.Commands
{
    public class NewNotificationCommand : IRequest<int>
    {
        public string Channel { get; set; }
        public string From { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public List<string> Recipients { get; set; }
    }
}

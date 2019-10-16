using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoteMonitor.Api.Observer.Commands
{

    public class NotificationRegDataCommand : IRequest<int>
    {
        public int ObserverId { get; set; }
        public string ChannelName { get; set; }
        public string Token { get; set; }
    }
}

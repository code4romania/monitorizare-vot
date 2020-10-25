﻿using MediatR;

namespace VoteMonitor.Api.Core.Commands
{
    public class NotificationRegistrationDataCommand : IRequest<int>
    {
        public int ObserverId { get; set; }
        public string ChannelName { get; set; }
        public string Token { get; set; }
    }
}
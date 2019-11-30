﻿using MediatR;

namespace VoteMonitor.Api.Observer.Commands
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        public int IdNgo { get; set; }
        public string PhoneNumber { get; set; }
        public string Pin { get; set; }
        public bool Organizer { get; set; }
    }
}
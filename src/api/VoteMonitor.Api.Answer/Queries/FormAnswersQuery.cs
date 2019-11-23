﻿using MediatR;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Queries
{
    public class FormAnswersQuery : IRequest<PollingStationInfosDTO>
    {
        public int PollingStationId { get; set; }
        public int ObserverId { get; set; }
    }
}
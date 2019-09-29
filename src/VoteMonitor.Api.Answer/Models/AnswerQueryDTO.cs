using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Answer.Models {
    public class AnswerQueryDTO {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public string ObserverName { get; set; }
        public string PollingStationName { get; set; }
    }

    public class AnswersQuery : PagingModel, IRequest<ApiListResponse<AnswerQueryDTO>> {
        public int IdONG { get; set; }
        public bool Urgent { get; set; }
        public bool Organizer { get; set; }
        public string County { get; set; }
        public int PollingStationNumber { get; set; }
        public int ObserverId { get; set; }
    }
}

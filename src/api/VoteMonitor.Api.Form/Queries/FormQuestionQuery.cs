using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries
{

    public class FormQuestionQuery : IRequest<IEnumerable<FormSectionDTO>>
    {
        public int FormId { get; set; }
        public int CacheHours { get; set; }
        public int CacheMinutes { get; set; }
        public int CacheSeconds { get; set; }
    }
}

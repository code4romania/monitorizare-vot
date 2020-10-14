using MediatR;
using System.Collections.Generic;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries
{
    public class FormVersionQuery : IRequest<List<FormDetailsModel>>
    {
        public bool? Diaspora { get; }
        public bool? Draft { get; }

        public FormVersionQuery(bool? diaspora, bool? draft)
        {
            Diaspora = diaspora;
            Draft = draft;
        }
    }

    public class FormQuestionsQuery : IRequest<IEnumerable<ModelSectiune>>
    {
        public string CodFormular { get; set; }
        public int CacheHours { get; set; }
        public int CacheMinutes { get; set; }
        public int CacheSeconds { get; set; }
    }
}

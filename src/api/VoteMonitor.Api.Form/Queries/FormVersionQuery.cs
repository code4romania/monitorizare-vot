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

    public class FormQuestionsQuery
    {
    }
}

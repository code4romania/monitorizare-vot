using System.Collections.Generic;
using MediatR;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Queries
{
    public class FormVersionQuery : IRequest<List<FormDetailsModel>>
    {
	    public bool? Diaspora { get; }

	    public FormVersionQuery(bool? diaspora)
	    {
		    Diaspora = diaspora;
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

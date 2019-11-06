using MediatR;
using System.Collections.Generic;

namespace VoteMonitor.Api.Form.Models
{
    public class FormVersionQuery : IRequest<List<VoteMonitor.Entities.Form>>
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

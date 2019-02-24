using MediatR;
using System.Collections.Generic;

namespace VotingIrregularities.Api.Models
{
    public class FormVersionQuery : IRequest<Dictionary<string, int>>
    {
    }

    public class FormQuestionsQuery : IRequest<IEnumerable<ModelSectiune>>
    {
        public string CodFormular { get; set; }
        public int CacheHours { get; set; }
        public int CacheMinutes { get; set; }
        public int CacheSeconds { get; set; }
    }
}

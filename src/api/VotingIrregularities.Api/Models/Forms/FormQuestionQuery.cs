using MediatR;
using System.Collections.Generic;
using VotingIrregularities.Api.Models.Forms;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Models
{

    public class FormQuestionQuery : IRequest<IEnumerable<FormSectionDTO>>
    {
        public string FormCode { get; set; }
        public int CacheHours { get; set; }
        public int CacheMinutes { get; set; }
        public int CacheSeconds { get; set; }
    }
}

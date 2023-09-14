using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoteMonitor.Api.Form.Commands
{
    public class DeleteQuestionOptionCommand : IRequest<bool>
    {
        public int SectionId { get; set; }

        public int QuestionId { get; set; }

        public int OptionId { get; set; }
    }
}

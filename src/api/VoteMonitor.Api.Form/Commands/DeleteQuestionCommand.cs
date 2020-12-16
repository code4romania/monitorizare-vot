using MediatR;

namespace VoteMonitor.Api.Form.Commands
{
    public class DeleteQuestionCommand : IRequest<bool>
    {
        public int SectionId { get; set; }

        public int QuestionId { get; set; }
    }
}

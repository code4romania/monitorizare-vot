using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Handlers
{
    public class DeleteQuestionHandler : IRequestHandler<DeleteQuestionCommand, bool>
    {
        private readonly VoteMonitorContext _context;
        public DeleteQuestionHandler(VoteMonitorContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                var questionToBeRemoved = await _context.Questions
                    .Where(q => q.Id == request.QuestionId && q.IdSection == request.SectionId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (questionToBeRemoved == null)
                {
                    return false;
                }

                if (await QuestionHasAnswers(questionToBeRemoved.Id) || await QuestionHasNotes(questionToBeRemoved.Id))
                {
                    return false;
                }

                DeleteOptionsToQuestion(questionToBeRemoved.Id);
                DeleteOptionsWithNoRelatedQuestions(questionToBeRemoved.Id);
                DeleteQuestion(questionToBeRemoved);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);
                return true;
            }
        }

        private async Task<bool> QuestionHasAnswers(int questionId)
        {
            var answersForQuestion = _context.Answers.Where(a => a.OptionAnswered.Question.Id == questionId);
            return await answersForQuestion.AnyAsync();
        }

        private async Task<bool> QuestionHasNotes(int questionId)
        {
            var notes = _context.Notes.Where(n => n.IdQuestion == questionId);
            return await notes.AnyAsync();
        }

        private void DeleteOptionsWithNoRelatedQuestions(int questionID)
        {
            var optionsToBeRemoved = _context.Options
                .Where(o => !o.OptionsToQuestions.Any(
                    otq => otq.IdQuestion != questionID && o.Id == otq.IdOption));
            _context.RemoveRange(optionsToBeRemoved);
        }

        private void DeleteOptionsToQuestion(int id)
        {
            var optionsToQuestionToBeRemoved = _context.OptionsToQuestions.Where(otq => otq.IdQuestion == id);
            _context.RemoveRange(optionsToQuestionToBeRemoved);
        }

        private void DeleteQuestion(Question questionToBeRemoved)
        {
            _context.Remove(questionToBeRemoved);
        }
    }
}

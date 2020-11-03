using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Handlers
{
    public class DeleteSectionHandler : IRequestHandler<DeleteSectionCommand, bool>
    {
        private readonly VoteMonitorContext _context;

        public DeleteSectionHandler(VoteMonitorContext context)
        {
            _context = context;
        }
        public async Task<bool> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {
                var sectionToBeRemoved = await _context.FormSections.FirstOrDefaultAsync(x => x.Id == request.SectionId);
                if (sectionToBeRemoved == null)
                {
                    return false;
                }

                if (await QuestionsFromThisSectionHaveAnswers(sectionToBeRemoved.Id))
                {
                    return false;
                }

                DeleteOptionsToQuestions(sectionToBeRemoved.Id);
                DeleteQuestions(sectionToBeRemoved.Id);
                DeleteSection(sectionToBeRemoved);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);

                return true;
            }
        }

        private async Task<bool> QuestionsFromThisSectionHaveAnswers(int sectionId)
        {
            var answersForSection = _context.Answers.Where(x => x.OptionAnswered.Question.IdSection == sectionId);
            return await answersForSection.AnyAsync();
        }

        private void DeleteQuestions(int sectionId)
        {
            var questionsToBeRemoved = _context.Questions.Where(x => x.IdSection == sectionId);
            _context.Questions.RemoveRange(questionsToBeRemoved);
        }

        private void DeleteOptionsToQuestions(int sectionId)
        {
            var optionsToQuestionsToBeRemoved = _context.OptionsToQuestions.Where(x => x.Question.IdSection == sectionId);
            _context.OptionsToQuestions.RemoveRange(optionsToQuestionsToBeRemoved);
        }

        private void DeleteSection(FormSection sectionToBeRemoved)
        {
            _context.FormSections.Remove(sectionToBeRemoved);
        }
    }
}

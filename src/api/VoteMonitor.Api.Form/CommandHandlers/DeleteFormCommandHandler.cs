using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers
{
    public class DeleteFormCommandHandler : IRequestHandler<DeleteFormCommand, bool>
    {
        private readonly VoteMonitorContext _context;

        public DeleteFormCommandHandler(VoteMonitorContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteFormCommand request, CancellationToken cancellationToken)
        {
            var form = await _context.Forms.FirstOrDefaultAsync(f => f.Id == request.FormId);
            if (form == null)
            {
                return false;
            }

            var sections = _context.FormSections.Where(s => s.IdForm == form.Id);
            var sectionsIds = sections.Select(s => s.Id);
            var questions = _context.Questions.Where(q => sectionsIds.Contains(q.IdSection));
            var questionsIds = questions.Select(q => q.Id);
            var optionsToQuestions = _context.OptionsToQuestions.Where(o => questionsIds.Contains(o.IdQuestion));
            var optionsIds = optionsToQuestions.Select(o => o.IdOption);

            // check if there are already saved answers
            var haveAnswers = await _context.Answers.AnyAsync(a => optionsIds.Contains(a.IdOptionToQuestion), cancellationToken);
            if (haveAnswers)
            {
                return false;
            }

            var options = _context.Options.Where(o => optionsIds.Contains(o.Id));

            _context.OptionsToQuestions.RemoveRange(optionsToQuestions);
            _context.Options.RemoveRange(options);
            _context.Questions.RemoveRange(questions);
            _context.FormSections.RemoveRange(sections);
            _context.Forms.Remove(form);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

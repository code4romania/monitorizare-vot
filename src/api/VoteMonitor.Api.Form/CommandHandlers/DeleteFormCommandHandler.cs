using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.CommandHandlers
{
    public class DeleteFormCommandHandler : IRequestHandler<DeleteFormCommand, Result<bool, DeleteFormErrorType>>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger<DeleteFormCommandHandler> _logger;

        public DeleteFormCommandHandler(VoteMonitorContext context, ILogger<DeleteFormCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<bool, DeleteFormErrorType>> Handle(DeleteFormCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var form = await _context.Forms.FirstOrDefaultAsync(f => f.Id == request.FormId);
                if (form == null)
                {
                    return DeleteFormErrorType.FormNotFound;
                }

                if (form.Draft == false)
                {
                    return DeleteFormErrorType.FormNotDraft;
                }

                var sections = _context.FormSections.Where(s => s.IdForm == form.Id);
                var sectionsIds = sections.Select(s => s.Id);
                var questions = _context.Questions.Where(q => sectionsIds.Contains(q.IdSection));
                var questionsIds = questions.Select(q => q.Id);
                var optionsToQuestions = _context.OptionsToQuestions.Where(o => questionsIds.Contains(o.IdQuestion)).ToList();
                var optionsIds = optionsToQuestions.Select(o => o.IdOption);
                var optionsToQuestionsIds = optionsToQuestions.Select(o => o.Id);

                // check if there are already saved answers
                var haveAnswers = await _context.Answers.AnyAsync(a => optionsToQuestionsIds.Contains(a.IdOptionToQuestion), cancellationToken);
                if (haveAnswers)
                {
                    return DeleteFormErrorType.FormHasAnswers;
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
            catch (Exception e)
            {
                _logger.LogError(e, $"Error occured when deleting form with id= {request.FormId}");
                return DeleteFormErrorType.ErrorOccured;
            }
        }
    }
}

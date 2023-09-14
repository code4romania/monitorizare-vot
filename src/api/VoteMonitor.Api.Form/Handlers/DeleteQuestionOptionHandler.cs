using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Handlers
{
    public class DeleteQuestionOptionHandler : IRequestHandler<DeleteQuestionOptionCommand, bool>
    {
        private readonly VoteMonitorContext _context;
        public DeleteQuestionOptionHandler(VoteMonitorContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteQuestionOptionCommand request, CancellationToken cancellationToken)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
            {

                var optionToBeRemoved = _context.Options.FirstOrDefault(o => o.Id == request.OptionId);

                if (optionToBeRemoved == null)
                {
                    return false;
                }


                if (await OptionToBeDeleted(request.OptionId))
                {
                    if (await OptionHasAnswers(request.OptionId))
                    {
                        return false;
                    }

                    await DeleteQuestionsToOption(request.OptionId);
                    DeleteOption(optionToBeRemoved);

                }
                else
                {
                    RemoveOptionFromQuestion(request.SectionId, request.QuestionId, request.OptionId);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);

                return true;
            }
        }

        private void RemoveOptionFromQuestion(int sectionId, int questionId, int optionId)
        {
            var questionOptionLinkToBeRemoved = _context.Questions.Where(q => q.IdSection == sectionId && q.Id == questionId)
            .SelectMany(q => q.OptionsToQuestions)
            .Where(otq => otq.IdOption == optionId).FirstOrDefault();

            _context.Remove(questionOptionLinkToBeRemoved);
        }

        private async Task<bool> OptionToBeDeleted(int optiondId)
        {
            var atMostOnOneQuestion = await _context.OptionsToQuestions
            .Where(o => o.IdOption == optiondId).CountAsync() <= 1;

            return atMostOnOneQuestion;

        }


        private async Task<bool> OptionHasAnswers(int optionId)
        {
            return await _context
            .Answers.Where(a => a.OptionAnswered.IdOption == optionId).AnyAsync();
        }

        private async Task DeleteQuestionsToOption(int optionId)
        {
            var questionsToOptions = await _context.OptionsToQuestions.Where(otq => otq.IdOption == optionId).ToListAsync();

            _context.RemoveRange(questionsToOptions);
        }

        private void DeleteOption(Option option)
        {
            _context.Remove(option);
        }
    }
}

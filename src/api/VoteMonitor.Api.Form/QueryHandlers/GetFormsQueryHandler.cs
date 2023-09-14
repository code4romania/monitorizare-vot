using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers;

public class GetFormsQueryHandler : IRequestHandler<GetFormsQuery, IReadOnlyList<FormResponseModel>>
{
    private readonly VoteMonitorContext _context;
    public GetFormsQueryHandler(VoteMonitorContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FormResponseModel>> Handle(GetFormsQuery request, CancellationToken cancellationToken)
    {
        var forms = await _context.Forms
            .Include(f => f.FormSections)
            .ThenInclude(fs => fs.Questions)
            .ThenInclude(q => q.OptionsToQuestions)
            .ThenInclude(otq => otq.Option)
            .Where(x => x.Draft == false)
            .OrderBy(f => f.Order)
            .Select(ToFormResponseModel)
            .ToListAsync(cancellationToken);

        return forms;
    }

    private static readonly Expression<Func<Entities.Form, FormResponseModel>> ToFormResponseModel =
        form => new FormResponseModel
        {
            Id = form.Id,
            Code = form.Code,
            CurrentVersion = form.CurrentVersion,
            Description = form.Description,
            Diaspora = form.Diaspora,
            FormSections = form.FormSections
                .AsQueryable()
                .OrderBy(x => x.OrderNumber)
                .Select(ToFormSectionResponseModel)
                .ToList()
                .AsReadOnly()
        };

    private static readonly Expression<Func<FormSection, FormSectionResponseModel>> ToFormSectionResponseModel =
        section => new FormSectionResponseModel
        {
            Id = section.Id,
            Code = section.Code,
            Description = section.Description,
            Questions = section.Questions
                .AsQueryable()
                .OrderBy(x => x.OrderNumber)
                .Select(ToQuestionResponseModel)
                .ToList()
                .AsReadOnly()
        };

    private static readonly Expression<Func<Question, QuestionResponseModel>> ToQuestionResponseModel =
        question => new QuestionResponseModel
        {
            Id = question.Id,
            Code = question.Code,
            Hint = question.Hint,
            QuestionType = question.QuestionType,
            Text = question.Text,
            Options = question.OptionsToQuestions
                .AsQueryable()
                .OrderBy(x => x.Option.OrderNumber)
                .Select(MapOptionsResponseModel)
                .ToList()
                .AsReadOnly()
        };

    private static readonly Expression<Func<OptionToQuestion, OptionsResponseModel>> MapOptionsResponseModel =
        optionToQuestion => new OptionsResponseModel
        {
            Id = optionToQuestion.Id,
            OptionId = optionToQuestion.Option.Id,
            Hint = optionToQuestion.Option.Hint,
            Text = optionToQuestion.Option.Text,
            IsFreeText = optionToQuestion.Option.IsFreeText,
            IsFlagged = optionToQuestion.Flagged
        };
}

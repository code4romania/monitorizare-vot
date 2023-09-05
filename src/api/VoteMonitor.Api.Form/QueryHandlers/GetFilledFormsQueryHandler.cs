using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers;

public class GetFilledFormsQueryHandler : IRequestHandler<GetFilledFormsQuery, IReadOnlyList<FilledFormResponseModel>>
{
    private readonly VoteMonitorContext _context;

    public GetFilledFormsQueryHandler(VoteMonitorContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FilledFormResponseModel>> Handle(GetFilledFormsQuery request, CancellationToken cancellationToken)
    {
        var answers = await _context.Answers.AsNoTracking()
            .Include(a => a.Observer)
            .Include(a => a.PollingStation)
            .Include(a => a.OptionAnswered)
            .ThenInclude(otq => otq.Question)
            .ThenInclude(q => q.FormSection)
            .ThenInclude(fs => fs.Form)
            .Where(a => a.OptionAnswered.Question.FormSection.Form.Draft == false) // exclude draft forms
            .Where(a => a.Observer.IdNgo != 1) // exclude code4ro org
            .Where(a => a.Observer.Ngo.IsActive)
            .Where(a => !a.Observer.IsTestObserver) // Exclude test observers ,test NGO and inactive NGOs
            .ToListAsync(cancellationToken);

        // we have to aggregate on client side since this cannot be translated to sql group by
        var groupings = answers.GroupBy(x => new
        {
            FormId = x.OptionAnswered.Question.FormSection.Form.Id,
            ObserverId = x.Observer.Id,
            PollingStationId = x.IdPollingStation
        });

        var results = new List<FilledFormResponseModel>();
        foreach (var grouping in groupings)
        {
            var form = new FilledFormResponseModel
            {
                FormId = grouping.Key.FormId,
                ObserverId = grouping.Key.ObserverId,
                LastModified = grouping.Max(x => x.LastModified),
                PollingStationId = grouping.Key.PollingStationId,
                FilledInQuestions = grouping
                    .GroupBy(x => new
                    {
                        Sectionid = x.OptionAnswered.Question.FormSection.Id,
                        QuestionId = x.OptionAnswered.IdQuestion
                    })
                    .Select(x => new AnswerResponseModel()
                    {
                        QuestionId = x.Key.QuestionId,
                        FormSectionId = x.Key.Sectionid,
                        SelectedOptions = x.Select(y => new SelectedOptionResponseModel
                            {
                                Id = y.IdOptionToQuestion,
                                OptionId = y.OptionAnswered.IdOption,
                                FreeTextValue = y.Value,
                                LasModified = y.LastModified
                            })
                            .ToList()
                            .AsReadOnly()
                    }).ToList().AsReadOnly()
            };

            results.Add(form);
        }

        return results.AsReadOnly();
    }
}
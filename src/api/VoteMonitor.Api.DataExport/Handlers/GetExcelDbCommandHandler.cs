using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.DataExport.FileGenerator;
using VoteMonitor.Api.DataExport.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.DataExport.Handlers;

public class GetExcelDbCommandHandler : IRequestHandler<GetExcelDbCommand, byte[]>
{
    private readonly VoteMonitorContext _context;

    public GetExcelDbCommandHandler(VoteMonitorContext context)
    {
        _context = context;
    }

    public async Task<byte[]> Handle(GetExcelDbCommand request, CancellationToken cancellationToken)
    {
        var ngos = await _context.Ngos
            .Select(ngo => new { ngo.Id, ngo.Name, ngo.Organizer, })
            .OrderBy(x=>x.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        var observers = await _context.Observers
            .Select(observer => new
            {
                observer.Id,
                observer.Phone,
                observer.Name,
                NgoId=  observer.IdNgo,
                observer.FromTeam,
                observer.IsTestObserver
            })
            .OrderBy(x => x.NgoId)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        var counties = await _context.Counties
            .OrderBy(x => x.Order)
            .Select(county => new { county.Id, county.Code, county.Name, county.Diaspora, county.Order })
            .ToListAsync(cancellationToken: cancellationToken);
      
        var municipalities = await _context.Municipalities
            .OrderBy(x => x.Order)
            .Select(municipality => new { municipality.Id, CountyId = municipality.County.Id, municipality.Code, municipality.Name })
            .ToListAsync(cancellationToken: cancellationToken);

        var pollingStations = await _context
            .PollingStations
            .Select(pollingStation => new
            {
                pollingStation.Id,
                CountyId = pollingStation.Municipality.County.Id,
                MunicipalityId = pollingStation.Municipality.Id,
                pollingStation.Number,
                pollingStation.Address
            })
            .OrderBy(x => x.CountyId)
            .ThenBy(x => x.MunicipalityId)
            .ThenBy(x => x.Number)
            .ToListAsync(cancellationToken: cancellationToken);

        var forms = await _context.Forms
            .Include(f => f.FormSections)
            .ThenInclude(fs => fs.Questions)
            .ThenInclude(q => q.OptionsToQuestions)
            .ThenInclude(otq => otq.Option)
            .Where(x => x.Draft == false)
            .OrderBy(f => f.Order)
            .ToListAsync(cancellationToken);

        var aggregatedForms = forms.SelectMany(form => form.FormSections
                .OrderBy(x => x.OrderNumber)
                .SelectMany(formSection => formSection.Questions.OrderBy(x => x.OrderNumber)
                    .Select(question => new
                    {
                        FormId = form.Id,
                        FormCode = form.Code,
                        FormCurrentVersion = form.CurrentVersion,
                        FormDescription = form.Description,
                        FormDiaspora = form.Diaspora,
                        FormSectionId = formSection.Id,
                        FormSectionCode = formSection.Code,
                        FormSectionDescription = formSection.Description,
                        QuestionId = question.Id,
                        QuestionCode = question.Code,
                        QuestionHint = question.Hint,
                        QuestionQuestionType = question.QuestionType,
                        QuestionText = question.Text,
                        Options = question.OptionsToQuestions
                            .OrderBy(x => x.Option.OrderNumber)
                            .Select(optionToQuestion => new
                            {
                                //optionToQuestion.Id,
                                OptionId = optionToQuestion.Option.Id,
                                optionToQuestion.Option.Hint,
                                optionToQuestion.Option.Text,
                                optionToQuestion.Option.IsFreeText,
                                IsFlagged = optionToQuestion.Flagged
                            })
                            .ToList()
                    })))
            .ToList();

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
            .Select(x => new
            {
                FormId = x.OptionAnswered.Question.FormSection.Form.Id,
                FormSectionId = x.OptionAnswered.Question.FormSection.Id,
                ObserverId = x.Observer.Id,
                PollingStationId = x.IdPollingStation,
                QuestionId = x.OptionAnswered.IdQuestion,
                x.LastModified,
                OptionId = x.OptionAnswered.IdOption
            })
            .ToListAsync(cancellationToken);

        // we have to aggregate on client side since this cannot be translated to sql group by
        var filledInForms = answers.GroupBy(x => new
            {
                x.FormId,
                x.FormSectionId,
                x.ObserverId,
                x.PollingStationId,
                x.QuestionId
            }).Select(g => new
            {
                g.Key.PollingStationId,
                g.Key.ObserverId,
                g.Key.FormId,
                g.Key.FormSectionId,
                g.Key.QuestionId,
                LastModified = g.Max(x => x.LastModified),
                Answers = g.Select(x => x.OptionId).ToList()
            })
            .OrderBy(x => x.ObserverId)
            .ToList();


        var notes = await _context.Notes
            .Include(x => x.Attachments)
            .Select(note => new
            {
                Id = note.Id,
                PollingStationId = note.IdPollingStation,
                ObserverId = note.IdObserver,
                note.IdQuestion,
                note.LastModified,
                Attachments = note.Attachments.Select(attachment => new { attachment.Id, attachment.Path }).ToList()
            }).ToListAsync(cancellationToken);

        return ExcelFile
            .New()
            .WithSheet("ngos", ngos)
            .WithSheet("observers", observers)
            .WithSheet("counties", counties)
            .WithSheet("municipalities", municipalities)
            .WithSheet("polling-stations", pollingStations)
            .WithSheet("forms", aggregatedForms)
            .WithSheet("filled-forms", filledInForms)
            .WithSheet("notes", notes)
            .Write();
    }
}

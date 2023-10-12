using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
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
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        var observers = await _context.Observers
            .Select(observer => new
            {
                observer.Id,
                observer.Phone,
                observer.Name,
                NgoId = observer.IdNgo,
                observer.FromTeam,
                observer.IsTestObserver
            })
            .OrderBy(x => x.NgoId)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        var provinces = await _context.Provinces
            .OrderBy(x => x.Order)
            .Select(province => new { province.Id, province.Code, province.Name })
            .ToListAsync(cancellationToken: cancellationToken);

        var counties = await _context.Counties
            .Include(x => x.Province)
            .OrderBy(x => x.Order)
            .Select(county => new { county.Id, county.Code, ProvinceCode = county.Province.Code, county.Name, county.Diaspora })
            .ToListAsync(cancellationToken: cancellationToken);

        var municipalities = await _context.Municipalities
            .Include(x => x.County)
            .OrderBy(x => x.Order)
            .Select(municipality => new { municipality.Id, municipality.Code, CountyCode = municipality.County.Code, municipality.Name })
            .ToListAsync(cancellationToken: cancellationToken);

        var pollingStations = await _context
            .PollingStations
            .AsNoTracking()
            .OrderBy(x => x.Municipality.Order)
            .ThenBy(x => x.Number)
            .Select(pollingStation => new
            {
                pollingStation.Id,
                MunicipalityCode = pollingStation.Municipality.Code,
                pollingStation.Number,
                pollingStation.Address
            })

            .ToListAsync(cancellationToken: cancellationToken);

        var forms = await _context.Forms
            .AsNoTracking()
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
                        FormCode = form.Code,
                        FormSectionCode = formSection.Code,
                        FormSectionDescription = formSection.Description,
                        QuestionCode = question.Code,
                        QuestionQuestionType = question.QuestionType,
                        QuestionText = question.Text,
                        Options = question.OptionsToQuestions
                            .OrderBy(x => x.Option.OrderNumber)
                            .Select(optionToQuestion => new
                            {
                                optionToQuestion.Option.Text,
                                optionToQuestion.Option.IsFreeText,
                                IsFlagged = optionToQuestion.Flagged
                            })
                            .ToList()
                    })))
            .ToList();

        var filledInForms = await GetFilledInForms(cancellationToken);

        var notes = await GetNotesForExport(cancellationToken);

        var excelBuilder = ExcelFile
            .New()
            .WithSheet("ngos", ngos)
            .WithSheet("observers", observers)
            .WithSheet("provinces", provinces)
            .WithSheet("counties", counties)
            .WithSheet("municipalities", municipalities)
            .WithSheet("polling-stations", pollingStations)
            .WithSheet("forms", aggregatedForms);

        filledInForms.ForEach(f =>
        {
            excelBuilder = excelBuilder.WithSheet(f.Code, f.Data);
        });

        excelBuilder = excelBuilder.WithSheet("notes", notes);

        return excelBuilder.Write();
    }

    private async Task<DataTable> GetNotesForExport(CancellationToken cancellationToken)
    {
        var notes = await _context.Notes
            .AsNoTracking()
            .Include(x => x.Attachments)
            .Include(x => x.Question)
            .ThenInclude(x => x.FormSection)
            .ThenInclude(x => x.Form)
            .Include(x => x.PollingStation)
            .ThenInclude(x => x.Municipality)
            .Select(note => new
            {
                Id = note.Id,
                PollingStation = note.PollingStation.Municipality.Code + ":" + note.PollingStation.Number,
                Question = note.Question,
                ObserverName = note.Observer.Name,
                ObserverPhone = note.Observer.Phone,
                ObserverId = note.Observer.Id,
                LastModified = note.LastModified,
                note.Text,
                Attachments = note.Attachments.Select(attachment => new { attachment.Path, attachment.FileName }).ToList()
            })
            .OrderBy(x => x.ObserverId)
            .ThenBy(x => x.LastModified)
            .ToListAsync(cancellationToken);

        DataTable dataTable = new DataTable();

        dataTable.Columns.Add("Observer Id", typeof(int));
        dataTable.Columns.Add("Observer Phone", typeof(string));
        dataTable.Columns.Add("Observer Name", typeof(string));
        dataTable.Columns.Add("Polling Station (municipalityCode:number:formCode:questionCode)", typeof(string));
        dataTable.Columns.Add("Last Modified (UTC)", typeof(string));
        dataTable.Columns.Add("Comment", typeof(string));
        var maxAttachments = notes.Select(x => x.Attachments.Count).DefaultIfEmpty(0).Max();

        for (int i = 1; i <= maxAttachments; i++)
        {
            dataTable.Columns.Add($"Attachment-{i}", typeof(string));
        }

        foreach (var note in notes)
        {
            object?[] rowValues = new List<object?>
                {
                    note.ObserverId,
                    note.ObserverPhone,
                    note.ObserverName,
                    GetNoteLocation(note.PollingStation, note.Question?.FormSection?.Form?.Code, note.Question?.Code),
                    note.LastModified.ToString("s"),
                    note.Text
                }
                .Union(note.Attachments.Select(x => x.Path))
                .ToArray();

            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }

    private static string GetNoteLocation(string pollingStation, string formCode, string questionCode)
    {
        if (!string.IsNullOrWhiteSpace(formCode) && !string.IsNullOrWhiteSpace(questionCode))
        {
            return pollingStation + ":" + formCode + ":" + questionCode;
        }

        return pollingStation;
    }

    private async Task<List<(string Code, DataTable Data)>> GetFilledInForms(CancellationToken cancellationToken)
    {
        var answers = await _context.Answers
            .Include(a => a.Observer)
            .Include(a => a.PollingStation)
            .ThenInclude(x => x.Municipality)
            .Include(a => a.OptionAnswered)
            .ThenInclude(x => x.Option)
            .Include(x => x.OptionAnswered.Question)
            .ThenInclude(q => q.FormSection)
            .ThenInclude(fs => fs.Form)
            .Where(a => a.OptionAnswered.Question.FormSection.Form.Draft == false) // exclude draft forms
            .Where(a => a.Observer.IdNgo != 1) // exclude code4ro org
            .Where(a => a.Observer.Ngo.IsActive)
            .Where(a => !a.Observer.IsTestObserver) // Exclude test observers ,test NGO and inactive NGOs
            .ToListAsync(cancellationToken);

        // we have to aggregate on client side since this cannot be translated to sql group by
        var filledInForms = answers.GroupBy(x => new
        {
            ObserverId = x.Observer.Id,
            ObserverPhone = x.Observer.Phone,
            ObserverName = x.Observer.Name,
            FormCode = x.OptionAnswered.Question.FormSection.Form.Code,
            PollingStation = x.PollingStation.Municipality.Code + ":" + x.PollingStation.Number
        })
            .Select(g => new
            {
                g.Key.ObserverId,
                g.Key.ObserverPhone,
                g.Key.ObserverName,
                g.Key.FormCode,
                g.Key.PollingStation,
                LastModified = g.Max(x => x.LastModified).ToString("s"),
                Answers = g
                    .GroupBy(x => new { x.OptionAnswered.Question.Code, x.OptionAnswered.Question.OrderNumber }, y => GetSelectedOptionText(y.OptionAnswered.Option.Text, y.Value))
                    .Select(x => new
                    {
                        QuestionCode = x.Key.Code,
                        QuestionOrderNumber = x.Key.OrderNumber,
                        SelectedValues = string.Join(",", x)
                    })
                    .ToList()
            })
            .GroupBy(x => x.FormCode, y => y,
                (key, group) => new
                {
                    FormCode = key,
                    Answers = group.OrderBy(x => x.ObserverId).ThenBy(x => x.LastModified).ToList()
                })
            .ToDictionary(x => x.FormCode, y => y.Answers);


        var result = new List<(string code, DataTable answers)>();

        foreach (var filledInForm in filledInForms)
        {
            var dataTable = new DataTable();
            var questionCodes = filledInForm
                 .Value
                 .SelectMany(x => x.Answers)
                 .DistinctBy(x => x.QuestionCode)
                 .OrderBy(x => x.QuestionOrderNumber)
                 .Select(x => x.QuestionCode)
                 .Select(x => new DataColumn(x, typeof(string)))
                 .ToArray();

            dataTable.Columns.Add("Observer Id", typeof(int));
            dataTable.Columns.Add("Observer Phone", typeof(string));
            dataTable.Columns.Add("Observer Name", typeof(string));
            dataTable.Columns.Add("FormCode", typeof(string));
            dataTable.Columns.Add("Polling Station (municipalityCode:number)", typeof(string));
            dataTable.Columns.Add("Last Modified (UTC)", typeof(string));
            dataTable.Columns.AddRange(questionCodes);

            foreach (var row in filledInForm.Value)
            {
                object?[] rowValues = new List<object?>
                {
                    row.ObserverId,
                    row.ObserverPhone,
                    row.ObserverName,
                    row.FormCode,
                    row.PollingStation,
                    row.LastModified
                }
                    .Union(row.Answers.Select(x => x.SelectedValues))
                    .ToArray();

                dataTable.Rows.Add(rowValues);
            }

            result.Add((filledInForm.Key, dataTable));
        }

        return result;
    }

    private string GetSelectedOptionText(string optionText, string enteredFreeText)
    {
        if (!string.IsNullOrWhiteSpace(enteredFreeText))
        {
            return $"{optionText}<<{enteredFreeText.Trim()}>>";
        }

        return optionText;
    }
}

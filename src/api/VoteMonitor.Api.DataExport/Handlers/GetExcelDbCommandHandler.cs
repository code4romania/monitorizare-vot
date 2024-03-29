using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Data;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.DataExport.FileGenerator;
using VoteMonitor.Api.DataExport.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.DataExport.Handlers;

public class GetExcelDbCommandHandler : IRequestHandler<GetExcelDbCommand, byte[]>
{
    private readonly IFileService _fileService;
    private readonly VoteMonitorContext _context;

    public GetExcelDbCommandHandler(IFileService fileService, VoteMonitorContext context)
    {
        _fileService = fileService;
        _context = context;
    }

    public async Task<byte[]> Handle(GetExcelDbCommand request, CancellationToken cancellationToken)
    {
        var ngos = await _context.Ngos
            .AsNoTracking()
            .Select(ngo => new { ngo.Id, ngo.Name, ngo.Organizer, })
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken: cancellationToken);

        var observers = await _context.Observers
            .AsNoTracking()
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
            .AsNoTracking()
            .OrderBy(x => x.Order)
            .Select(province => new { province.Id, province.Code, province.Name })
            .ToListAsync(cancellationToken: cancellationToken);

        var counties = await _context.Counties
            .AsNoTracking()
            .Include(x => x.Province)
            .OrderBy(x => x.Order)
            .Select(county => new { county.Id, county.Code, ProvinceCode = county.Province.Code, county.Name, county.Diaspora })
            .ToListAsync(cancellationToken: cancellationToken);

        var municipalities = await _context.Municipalities
            .AsNoTracking()
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

        var pollingStationObservations = await GetPollingStationsObservations(cancellationToken);

        var aggregatedForms = await GetForms(cancellationToken);
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
            .WithSheet("forms", aggregatedForms)
            .WithSheet("polling-stations-observations", pollingStationObservations);

        filledInForms.ForEach(f =>
        {
            excelBuilder = excelBuilder.WithSheet(f.Code, f.Data);
        });

        excelBuilder = excelBuilder.WithSheet("notes", notes);

        return excelBuilder.Write();
    }

    private async Task<DataTable> GetPollingStationsObservations(CancellationToken cancellationToken)
    {
        var data = await _context.PollingStationInfos
            .AsNoTracking()
            .Include(x => x.Observer)
            .Include(x => x.PollingStation)
            .ThenInclude(x => x.Municipality)
            .Select(x => new
            {
                ObserverId = x.Observer.Id,
                ObserverName = x.Observer.Name,
                ObserverPhone = x.Observer.Phone,
                PollingStation = x.PollingStation.Municipality.Code + ":" + x.PollingStation.Number,
                LastModified = x.LastModified,
                ArrivalTime = x.ObserverArrivalTime,
                NumberOfVotersOnTheList = x.NumberOfVotersOnTheList,
                NumberOfCommissionMembers = x.NumberOfCommissionMembers,
                NumberOfFemaleMembers = x.NumberOfFemaleMembers,
                MinPresentMembers = x.MinPresentMembers,
                ChairmanPresence = x.ChairmanPresence,
                SinglePollingStationOrCommission = x.SinglePollingStationOrCommission,
                AdequatePollingStationSize = x.AdequatePollingStationSize
            })
            .ToListAsync(cancellationToken);

        var dataTable = new DataTable();

        dataTable.Columns.Add("Observer Id", typeof(int));
        dataTable.Columns.Add("Observer Phone", typeof(string));
        dataTable.Columns.Add("Observer Name", typeof(string));
        dataTable.Columns.Add("Polling Station (municipalityCode:number)", typeof(string));
        dataTable.Columns.Add("Last Modified (UTC)", typeof(string));
        dataTable.Columns.Add("Observer Arrival Time (UTC)", typeof(string));
        dataTable.Columns.Add("NumberOfVotersOnTheList", typeof(string));
        dataTable.Columns.Add("NumberOfCommissionMembers", typeof(string));
        dataTable.Columns.Add("NumberOfFemaleMembers", typeof(string));
        dataTable.Columns.Add("MinPresentMembers", typeof(string));
        dataTable.Columns.Add("ChairmanPresence", typeof(bool));
        dataTable.Columns.Add("SinglePollingStationOrCommission", typeof(bool));
        dataTable.Columns.Add("AdequatePollingStationSize", typeof(bool));

        foreach (var row in data)
        {
            object?[] rowValues =
            {
                row.ObserverId,
                row.ObserverPhone,
                row.ObserverName,
                row.PollingStation,
                row.LastModified.ToString("s"),
                row.ArrivalTime?.ToString("s") ?? "",
                row.NumberOfVotersOnTheList,
                row.NumberOfCommissionMembers,
                row.NumberOfFemaleMembers,
                row.MinPresentMembers,
                row.ChairmanPresence,
                row.SinglePollingStationOrCommission,
                row.AdequatePollingStationSize,
            };
            dataTable.Rows.Add(rowValues);
        }

        return dataTable;
    }

    private async Task<DataTable> GetForms(CancellationToken cancellationToken)
    {
        var forms = await _context.Forms
            .AsNoTracking()
            .Include(f => f.FormSections)
            .ThenInclude(fs => fs.Questions)
            .ThenInclude(q => q.OptionsToQuestions)
            .ThenInclude(otq => otq.Option)
            .Where(x => x.Draft == false)
            .OrderBy(f => f.Order)
            .ToListAsync(cancellationToken);

        var questions = forms.SelectMany(form => form.FormSections
                .OrderBy(x => x.OrderNumber)
                .SelectMany(formSection => formSection.Questions.OrderBy(x => x.OrderNumber)
                    .Select(question => new
                    {
                        FormCode = form.Code,
                        FormOrderNumber = form.Order,
                        FormSectionCode = formSection.Code,
                        QuestionCode = question.Code,
                        QuestionType = question.QuestionType,
                        QuestionText = question.Text,
                        QuestionOrderNumber = question.OrderNumber,
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
            .OrderBy(q => q.FormOrderNumber)
            .ThenBy(q => q.QuestionOrderNumber)
            .ToList();

        DataTable dataTable = new DataTable();

        dataTable.Columns.Add("FormCode", typeof(string));
        dataTable.Columns.Add("FormSectionCode", typeof(string));
        dataTable.Columns.Add("QuestionCode", typeof(string));
        dataTable.Columns.Add("Question", typeof(string));
        dataTable.Columns.Add("Type", typeof(string));
        var maxNumberOfOptions = questions.Select(x => x.Options.Count).DefaultIfEmpty(0).Max();

        for (int i = 1; i <= maxNumberOfOptions; i++)
        {
            dataTable.Columns.Add($"Options-{i}", typeof(string));
        }

        foreach (var question in questions)
        {
            var rowValues = new List<object?>
                {
                    question.FormCode,
                    question.FormSectionCode,
                    question.QuestionCode,
                    question.QuestionText,
                    GetFormattedQuestionType(question.QuestionType),
                };

            rowValues.AddRange(question.Options.Select(x => FormatOption(x.Text, x.IsFreeText, x.IsFlagged)));
            dataTable.Rows.Add(rowValues.ToArray());
        }

        return dataTable;
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
                Attachments = note.Attachments.Select(attachment => new { attachment.FileName }).ToList()
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
            var rowValues = new List<object?>
            {
                note.ObserverId,
                note.ObserverPhone,
                note.ObserverName,
                GetNoteLocation(note.PollingStation, note.Question?.FormSection?.Form?.Code, note.Question?.Code),
                note.LastModified.ToString("s"),
                note.Text
            };

            rowValues.AddRange(note.Attachments.Select(x => _fileService.GetPreSignedUrl(x.FileName)));
                
            dataTable.Rows.Add(rowValues.ToArray());
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
            .AsNoTracking()
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
                    Answers = group
                        .OrderBy(x => x.ObserverId)
                        .ThenBy(x => x.LastModified)
                        .ToList()
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
               
                 .ToArray();

            dataTable.Columns.Add("Observer Id", typeof(int));
            dataTable.Columns.Add("Observer Phone", typeof(string));
            dataTable.Columns.Add("Observer Name", typeof(string));
            dataTable.Columns.Add("FormCode", typeof(string));
            dataTable.Columns.Add("Polling Station (municipalityCode:number)", typeof(string));
            dataTable.Columns.Add("Last Modified (UTC)", typeof(string));

            dataTable.Columns.AddRange(questionCodes.Select(x => new DataColumn(x, typeof(string))).ToArray());

            foreach (var row in filledInForm.Value)
            {
                var rowValues = new List<object?>
                {
                    row.ObserverId,
                    row.ObserverPhone,
                    row.ObserverName,
                    row.FormCode,
                    row.PollingStation,
                    row.LastModified
                };

                var userAnswers = row.Answers
                    .ToDictionary(x => x.QuestionCode, y => y.SelectedValues);

                foreach (var questionCode in questionCodes)
                {
                    var hasFilledInAnswer = userAnswers.TryGetValue(questionCode, out var selectedValues);
                    rowValues.Add(hasFilledInAnswer ? selectedValues : string.Empty);
                }

                dataTable.Rows.Add(rowValues.ToArray());
            }

            result.Add((filledInForm.Key, dataTable));
        }

        return result;
    }

    private static string GetSelectedOptionText(string optionText, string enteredFreeText)
    {
        if (!string.IsNullOrWhiteSpace(enteredFreeText))
        {
            return $"{optionText}<<{enteredFreeText.Trim()}>>";
        }

        return optionText;
    }

    private static string GetFormattedQuestionType(QuestionType questionType)
    {
        switch (questionType)
        {
            case QuestionType.MultipleOption:
                return "multiple choice";
            case QuestionType.SingleOption:
                return "single choice";
            case QuestionType.MultipleOptionWithText:
                return "multiple choice with text";
            case QuestionType.SingleOptionWithText:
                return "single choice with text";
            default:
                return "unknown";
        }
    }

    private static string FormatOption(string text, bool isFreeText, bool isFlagged)
    {
        string isFreeTextFlag = isFreeText ? "$text" : string.Empty;
        string isFlaggedFlag = isFlagged ? "$flagged" : string.Empty;

        return $"{text}{isFreeTextFlag}{isFlaggedFlag}";
    }
}

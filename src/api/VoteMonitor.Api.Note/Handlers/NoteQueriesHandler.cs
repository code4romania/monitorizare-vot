using MediatR;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Note.Commands;
using VoteMonitor.Api.Note.Models;
using VoteMonitor.Api.Note.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Note.Handlers;

public class NoteQueriesHandler :
    IRequestHandler<NoteQuery, List<NoteModel>>,
    IRequestHandler<AddNoteCommandV2, int>,
    IRequestHandler<AddNoteCommand, int>
{

    private readonly VoteMonitorContext _context;

    public NoteQueriesHandler(VoteMonitorContext context)
    {
        _context = context;
    }
    public async Task<List<NoteModel>> Handle(NoteQuery message, CancellationToken token)
    {
        var query = _context.Notes.Include(c => c.Question).AsQueryable();

        if (message.ObserverId.HasValue)
            query = query.Where(x => x.IdObserver == message.ObserverId);

        if (message.IdQuestion.HasValue)
            query = query.Where(x => x.Question.Id == message.IdQuestion);

        if (message.PollingStationId.HasValue)
            query = query.Where(x => x.IdPollingStation == message.PollingStationId);

        var notes =  await query
            .OrderBy(n => n.LastModified)
            .Include(n => n.Attachments)
            .Select(n=> new {
                AttachmentsPaths = n.Attachments.Select(x => x.Path).ToArray(),
                Text = n.Text,
                FormCode = (string?)n.Question.FormSection.Form.Code,
                FormId = (int?)n.Question.FormSection.Form.Id,
                QuestionId = (int?)n.Question.Id,
                CountyCode = n.PollingStation.Municipality.County.Code,
                MunicipalityCode = n.PollingStation.Municipality.Code,
                PollingStationNumber = n.PollingStation.Number
            })
            .ToListAsync(cancellationToken: token);

        return notes.Select(n => new NoteModel
        {
            AttachmentsPaths = n.AttachmentsPaths,
            Text = n.Text,
            FormCode = n.FormCode,
            FormId = n.FormId ?? -1,
            QuestionId = n.QuestionId,
            CountyCode = n.CountyCode,
            MunicipalityCode = n.MunicipalityCode,
            PollingStationNumber = n.PollingStationNumber
        }).ToList();
    }

    public async Task<int> Handle(AddNoteCommandV2 request, CancellationToken cancellationToken)
    {
        var noteEntity = new Entities.Note
        {
            Text = request.Text,
            IdPollingStation = request.PollingStationId,
            // A note can be added to a polling station as well.
            // In that case IdQuestion is either null or 0
            IdQuestion = request.QuestionId == 0 ? null : request.QuestionId,
            IdObserver = request.ObserverId,
            LastModified = DateTime.UtcNow,
            Attachments = request.Attachments.Select(a => new NotesAttachments { Path = a.Path, FileName= a.FileName }).ToList()
        };

        _context.Notes.Add(noteEntity);

        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> Handle(AddNoteCommand request, CancellationToken cancellationToken)
    {
        var noteEntity = new Entities.Note
        {
            Text = request.Text,
            IdPollingStation = request.IdPollingStation,
            // A note can be added to a polling station as well.
            // In that case IdQuestion is either null or 0
            IdQuestion = request.IdQuestion == 0 ? null : request.IdQuestion, 
            IdObserver = request.IdObserver,
            LastModified = DateTime.UtcNow,
            Attachments = new List<NotesAttachments>()
        };

        if (request.Attachement!=null)
        {
            noteEntity.Attachments.Add(new NotesAttachments
            {
                Path = request.Attachement.Path,
                FileName = request.Attachement.FileName
            });
        }

        _context.Notes.Add(noteEntity);

        return await _context.SaveChangesAsync(cancellationToken);
    }
}

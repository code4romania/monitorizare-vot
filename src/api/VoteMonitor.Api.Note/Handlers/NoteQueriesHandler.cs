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

        if (message.IdObserver.HasValue)
            query = query.Where(x => x.IdObserver == message.IdObserver);

        if (message.IdQuestion.HasValue)
            query = query.Where(x => x.Question.Id == message.IdQuestion);

        if (message.IdPollingStation.HasValue)
            query = query.Where(x => x.IdPollingStation == message.IdPollingStation);

        return await query
            .OrderBy(n => n.LastModified)
            .Include(n => n.Attachments)
            .Select(n => new NoteModel
            {
                AttachmentsPaths = n.Attachments.Select(x => x.Path).ToArray(),
                Text = n.Text,
                FormCode = n.Question.FormSection.Form.Code,
                FormId = n.Question.FormSection.Form.Id,
                QuestionId = n.Question.Id,
                CountyCode = n.PollingStation.Municipality.County.Code,
                MunicipalityCode = n.PollingStation.Municipality.Code,
                PollingStationNumber = n.PollingStation.Number
            })
            .ToListAsync(cancellationToken: token);
    }

    public async Task<int> Handle(AddNoteCommandV2 request, CancellationToken cancellationToken)
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
            Attachments = request.AttachmentPaths.Select(path => new NotesAttachments { Path = path }).ToList()
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

        if (!string.IsNullOrEmpty(request.AttachementPath))
        {
            noteEntity.Attachments.Add(new NotesAttachments
            {
                Path = request.AttachementPath
            });
        }

        _context.Notes.Add(noteEntity);

        return await _context.SaveChangesAsync(cancellationToken);
    }
}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Note.Commands;
using VoteMonitor.Api.Note.Models;
using VoteMonitor.Api.Note.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Note.Handlers
{
    public class NoteQueriesHandler :
        IRequestHandler<NoteQuery, List<NoteModel>>,
        IRequestHandler<AddNoteCommandV2, int>,
        IRequestHandler<AddNoteCommand, int>
    {

        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public NoteQueriesHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
                    CountyCode = n.PollingStation.County.Code,
                    PollingStationNumber = n.PollingStation.Number
                })
                .ToListAsync(cancellationToken: token);
        }

        public async Task<int> Handle(AddNoteCommandV2 request, CancellationToken cancellationToken)
        {
            try
            {
                var noteEntity = _mapper.Map<Entities.Note>(request);

                _context.Notes.Add(noteEntity);

                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<int> Handle(AddNoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var noteEntity = _mapper.Map<Entities.Note>(request);

                _context.Notes.Add(noteEntity);

                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}

using System;
using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Note.Commands;
using VoteMonitor.Entities;
using VoteMonitor.Api.Note.Models;
using VoteMonitor.Api.Note.Queries;

namespace VoteMonitor.Api.Note.Handlers
{
	public class NoteQueriesHandler :
		IRequestHandler<NoteQuery, List<NoteModel>>,
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
			return await _context.Notes
				.Where(n => n.IdObserver == message.IdObserver && n.IdPollingStation == message.IdPollingStation)
				.OrderBy(n => n.LastModified)
				.Select(n => new NoteModel
				{
					AttachmentPath = n.AttachementPath,
					Text = n.Text,
					FormCode = n.Question.FormSection.Form.Code,
					QuestionId = n.Question.Id
				})
				.ToListAsync(cancellationToken: token);
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

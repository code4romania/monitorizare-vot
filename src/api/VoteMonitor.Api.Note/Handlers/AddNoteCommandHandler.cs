using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Note.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Note.Handlers
{
    public class AddNoteCommandHandler : IRequestHandler<AddNoteCommand, Entities.Note>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AddNoteCommandHandler(VoteMonitorContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Entities.Note> Handle(AddNoteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.IdQuestion.HasValue && request.IdQuestion.Value > 0)
                {
                    var existaIntrebare = await _context.Questions.AnyAsync(i => i.Id == request.IdQuestion.Value, cancellationToken);

                    if (!existaIntrebare)
                        throw new EntityNotFoundException(typeof(Question), request.IdQuestion.Value);
                }

                var nota = _mapper.Map<Entities.Note>(request);
                await _context.AddAsync(nota);

                await _context.SaveChangesAsync(cancellationToken);
                return nota;
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
                throw;
            }
        }
    }
}

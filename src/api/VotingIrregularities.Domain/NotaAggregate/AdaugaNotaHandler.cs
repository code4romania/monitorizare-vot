using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Entities;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System;
using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace VotingIrregularities.Domain.NotaAggregate
{
    public class AdaugaNotaHandler : IRequestHandler<AdaugaNotaCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AdaugaNotaHandler(VoteMonitorContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(AdaugaNotaCommand message, CancellationToken token)
        {
            try
            {
                if (message.IdIntrebare.HasValue && message.IdIntrebare.Value > 0)
                {
                    var existaIntrebare = await _context.Questions.AnyAsync(i => i.Id == message.IdIntrebare.Value, token);

                    if (!existaIntrebare)
                        throw new ArgumentException($"Intrebarea {message.IdIntrebare.Value} nu exista");
                }

                var nota = _mapper.Map<Note>(message);
                _context.Add(nota);

                return await _context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }
    }
}

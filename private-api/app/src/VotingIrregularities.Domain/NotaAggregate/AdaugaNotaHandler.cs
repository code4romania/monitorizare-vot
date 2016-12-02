using System.Threading.Tasks;
using MediatR;
using VotingIrregularities.Domain.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System;
using Microsoft.EntityFrameworkCore;

namespace VotingIrregularities.Domain.NotaAggregate
{
    public class AdaugaNotaHandler : IAsyncRequestHandler<AdaugaNotaCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AdaugaNotaHandler(VotingContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(AdaugaNotaCommand message)
        {
            try
            {
                if (message.IdIntrebare.HasValue && message.IdIntrebare.Value > 0)
                {
                    var existaIntrebare = await _context.Intrebare.AnyAsync(i => i.IdIntrebare == message.IdIntrebare.Value);

                    if(!existaIntrebare)
                        throw new ArgumentException("Intrebarea nu exista");
                }

                var nota = _mapper.Map<Nota>(message);
                _context.Add(nota);

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }
    }
}

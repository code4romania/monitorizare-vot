using System;
using System.Threading.Tasks;
using MediatR;
using VotingIrregularities.Domain.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VotingIrregularities.Domain.SectieAggregate
{
    public class ActualizeazaSectieHandler : IAsyncRequestHandler<ActualizeazaSectieCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ActualizeazaSectieHandler(VotingContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(ActualizeazaSectieCommand message)
        {
            try
            {
                //TODO this can be moved to a previous step, before the command is executed
                int idSectie = await _context.SectieDeVotare
                    .Where(a =>
                        a.NumarSectie == message.NumarSectie &&
                        a.IdJudetNavigation.CodJudet == message.CodJudet).Select(a => a.IdSectieDeVotarre)
                        .FirstOrDefaultAsync();

                if (idSectie == 0)
                    throw new ArgumentException("Sectia nu exista");

                var formular = await _context.RaspunsFormular
                    .FirstOrDefaultAsync(a =>
                        a.IdObservator == message.IdObservator &&
                        a.IdSectieDeVotare == idSectie);

                if (formular == null)
                    throw new ArgumentException("RaspunsFormular nu exista");
                else
                {
                    _mapper.Map(message, formular);
                    _context.Update(formular);
                }

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

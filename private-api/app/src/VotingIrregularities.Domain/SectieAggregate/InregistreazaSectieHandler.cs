using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;


namespace VotingIrregularities.Domain.SectieAggregate
{
    public class InregistreazaSectieHandler : IAsyncRequestHandler<InregistreazaSectieCommand,int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public InregistreazaSectieHandler(VotingContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(InregistreazaSectieCommand message)
        {
            try
            {
                //TODO[DH] this can be moved to a previous step, before the command is executed
                int? idSectie = await _context.SectieDeVotare
                    .Where(a =>
                        a.NumarSectie == message.NumarSectie &&
                        a.IdJudetNavigation.CodJudet == message.CodJudet).Select(a => a.IdSectieDeVotarre)
                        .FirstOrDefaultAsync();

                if(idSectie == null)
                    throw new ArgumentException("Sectia nu exista");

                var formular = await _context.RaspunsFormular
                    .Where(a =>
                        a.IdObservator == message.IdObservator &&
                        a.IdSectieDeVotare == idSectie)
                    .FirstOrDefaultAsync();

                if (formular == null)
                {
                    formular = _mapper.Map<RaspunsFormular>(message);

                    formular.IdSectieDeVotare = idSectie.Value;

                    _context.Add(formular);
                }
                else
                {
                    _mapper.Map(message, formular);
                }

                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(),ex.Message);
            }

            return -1;
        }
    }
}
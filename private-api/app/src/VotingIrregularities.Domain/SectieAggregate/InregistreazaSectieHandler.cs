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
    public class InregistreazaSectieHandler : IAsyncRequestHandler<InregistreazaSectieCommand, int>
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
                int idSectie = await _context.PollingStations
                    .Where(a =>
                        a.Number == message.NumarSectie &&
                        a.County.Code == message.CodJudet).Select(a => a.Id)
                        .FirstOrDefaultAsync();

                if (idSectie == 0)
                    throw new ArgumentException("Sectia nu exista");

                var formular = await _context.PollingStationInfos
                    .FirstOrDefaultAsync(a =>
                        a.IdObserver == message.IdObservator &&
                        a.IdPollingStation == idSectie);

                if (formular == null)
                {
                    formular = _mapper.Map<PollingStationInfo>(message);

                    formular.IdPollingStation = idSectie;

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
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }
    }
}
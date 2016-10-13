using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;
using Microsoft.EntityFrameworkCore;
using VotingIrregularities.Domain.ValueObjects;

namespace VotingIrregularities.Api.Queries
{
    /// <summary>
    /// Hidrateaza sectiile de votare din comanda data de observator.
    /// </summary>
    public class RaspunsQueryHandler : 
        IAsyncRequestHandler<RaspunsuriBulk, CompleteazaRaspunsCommand>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public RaspunsQueryHandler(VotingContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<CompleteazaRaspunsCommand> Handle(RaspunsuriBulk message)
        {
            // se identifica sectiile in care observatorul a raspuns
            var sectii = message.ModelRaspunsuriBulk
                .Select(a => new {a.NumarSectie, a.CodJudet})
                .Distinct()
                .ToList();

            var command = new CompleteazaRaspunsCommand { IdObservator = message.IdObservator };

            //TODO[DH] - se pot obtine dintr-un cache in loc de BD. daca se gaseste mai mult de o sectie la o pereche de NUmarsectie si codjudet se arunca exceptie
            foreach (var sectie in sectii)
            {
                JudetEnum judet;
                var j = Enum.TryParse(sectie.CodJudet, true, out judet);

                if (!j)
                    throw new ArgumentException($"Judet inexistent: {sectie.CodJudet}");

                var idSectie = await
                    _context.SectieDeVotare.AsNoTracking()
                        .Where(
                            a =>
                                a.IdJudet == (int)judet &&
                                a.NumarSectie == sectie.NumarSectie)
                        .Select(a => a.IdSectieDeVotarre)
                        .ToListAsync();

                if (idSectie.Count == 0)
                    throw new ArgumentException($"Sectie inexistenta: {sectie}");

                command.Raspunsuri.AddRange(message.ModelRaspunsuriBulk
                    .Where(a => a.NumarSectie == sectie.NumarSectie && a.CodJudet == sectie.CodJudet)
                    .Select(a => new ModelRaspuns
                {
                    CodFormular = a.CodFormular,
                    IdIntrebare = a.IdIntrebare,
                    IdSectie = idSectie.Single(),
                    Optiuni = a.Optiuni
                }));
            }

            return await Task.FromResult(command);
        }
    }
}

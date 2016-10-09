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

            // cross check cu sectiile existente in BD
            var idsectii = (from localSectii in sectii
                           join dbSectii in _context.SectieDeVotare.Include(s => s.IdJudetNavigation)
                                    on new { localSectii.NumarSectie, localSectii.CodJudet } equals
                                    new { dbSectii.NumarSectie, dbSectii.IdJudetNavigation.CodJudet}
                                    into sectiiExistente
                           from potrivite in sectiiExistente.DefaultIfEmpty() 
                           select new
                           {
                               localSectii.CodJudet,
                               localSectii.NumarSectie,
                               potrivite?.IdSectieDeVotarre
                           }).ToList();

            // daca cel putin o sectie nu exista, se anuleaza comanda
            if (idsectii.Any(a => !a.IdSectieDeVotarre.HasValue))
                throw new ArgumentException("Sectii de votare inexistente");

           
            var command = new CompleteazaRaspunsCommand {IdObservator = message.IdObservator};

            command.Raspunsuri.AddRange(message.ModelRaspunsuriBulk.Select(a => new ModelRaspuns
            {
                CodFormular = a.CodFormular,
                IdIntrebare = a.IdIntrebare,
                IdSectie = idsectii.Single(i => i.CodJudet == a.CodJudet && i.NumarSectie == a.NumarSectie).IdSectieDeVotarre.Value,
                Optiuni = a.Optiuni
            }));

            return await Task.FromResult(command);

        }
    }
}

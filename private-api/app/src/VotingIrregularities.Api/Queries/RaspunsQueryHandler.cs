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
using VotingIrregularities.Api.Services;
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
        private readonly ISectieDeVotareService _svService;

        public RaspunsQueryHandler(VotingContext context, ISectieDeVotareService svService)
        {
            _context = context;
            _svService = svService;
        }

        public async Task<CompleteazaRaspunsCommand> Handle(RaspunsuriBulk message)
        {
            // se identifica sectiile in care observatorul a raspuns
            var sectii = message.ModelRaspunsuriBulk
                .Select(a => new {a.NumarSectie, a.CodJudet})
                .Distinct()
                .ToList();

            var command = new CompleteazaRaspunsCommand { IdObservator = message.IdObservator };

            
            foreach (var sectie in sectii)
            {
                //TODO[DH] - se pot obtine dintr-un cache in loc de BD. daca se gaseste mai mult de o sectie la o pereche de Numarsectie si codjudet se arunca exceptie
                var idSectie = await _svService.GetSingleSectieDeVotare(sectie.CodJudet, sectie.NumarSectie);

                command.Raspunsuri.AddRange(message.ModelRaspunsuriBulk
                    .Where(a => a.NumarSectie == sectie.NumarSectie && a.CodJudet == sectie.CodJudet)
                    .Select(a => new ModelRaspuns
                {
                    CodFormular = a.CodFormular,
                    IdIntrebare = a.IdIntrebare,
                    IdSectie = idSectie,
                    Optiuni = a.Optiuni
                }));
            }

            return await Task.FromResult(command);
        }
    }
}

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
        private readonly ISectieDeVotareService _svService;

        public RaspunsQueryHandler(ISectieDeVotareService svService)
        {
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
                var idSectie = await _svService.GetSingleSectieDeVotare(sectie.CodJudet, sectie.NumarSectie);

                command.Raspunsuri.AddRange(message.ModelRaspunsuriBulk
                    .Where(a => a.NumarSectie == sectie.NumarSectie && a.CodJudet == sectie.CodJudet)
                    .Select(a => new ModelRaspuns
                {
                    CodFormular = a.CodFormular,
                    IdIntrebare = a.IdIntrebare,
                    IdSectie = idSectie,
                    Optiuni = a.Optiuni,
                    NumarSectie = a.NumarSectie,
                    CodJudet = a.CodJudet
                }));
            }

            return command;
        }
    }
}

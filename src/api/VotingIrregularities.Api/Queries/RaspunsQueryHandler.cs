using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Services;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;

namespace VotingIrregularities.Api.Queries
{
    /// <summary>
    /// Hidrateaza sectiile de votare din comanda data de observator.
    /// </summary>
    public class RaspunsQueryHandler :
        IRequestHandler<RaspunsuriBulk, CompleteazaRaspunsCommand>
    {
        private readonly IPollingStationService _pollingStationService;

        public RaspunsQueryHandler(IPollingStationService svService)
        {
            _pollingStationService = svService;
        }

        public async Task<CompleteazaRaspunsCommand> Handle(RaspunsuriBulk message, CancellationToken cancellationToken)
        {
            // se identifica sectiile in care observatorul a raspuns
            var sectii = message.ModelRaspunsuriBulk
                .Select(a => new { a.NumarSectie, a.CodJudet })
                .Distinct()
                .ToList();

            var command = new CompleteazaRaspunsCommand { IdObservator = message.IdObservator };


            foreach (var sectie in sectii)
            {
                var idSectie = await _pollingStationService.GetPollingStationByCountyCode(sectie.NumarSectie, sectie.CodJudet);

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

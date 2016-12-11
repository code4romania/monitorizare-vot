using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.ValueObjects;

namespace VotingIrregularities.Api.Services
{
    public class SectieDevotareDBService : ISectieDeVotareService
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;

        public SectieDevotareDBService(VotingContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> GetSingleSectieDeVotare(string codJudet, int numarSectie)
        {
            try
            {
                JudetEnum judet;
                var j = Enum.TryParse(codJudet, true, out judet);

                if (!j)
                    throw new ArgumentException($"Judet inexistent: {codJudet}");

                return await GetSingleSectieDeVotare((int)judet, numarSectie);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }

        public async Task<int> GetSingleSectieDeVotare(int idJudet, int numarSectie)
        {
            try
            {
                var idSectie = await
                _context.SectieDeVotare
                    .Where(
                        a =>
                            a.IdJudet == idJudet &&
                            a.NumarSectie == numarSectie)
                    .Select(a => a.IdSectieDeVotarre)
                    .ToListAsync();

                if (idSectie.Count == 0)
                    throw new ArgumentException($"Sectie inexistenta pentru: {new { idJudet, numarSectie }}");


                if (idSectie.Count > 1)
                    throw new ArgumentException($"S-au gasit mai multe sectii pentru: {new { idJudet, idSectie }}");

                return idSectie.Single();
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.ValueObjects;

namespace VotingIrregularities.Api.Services
{
    public class SectieDevotareDBService : ISectieDeVotareService
    {
        private readonly VotingContext _context;

        public SectieDevotareDBService(VotingContext context)
        {
            _context = context;
        }

        public async Task<int> GetSingleSectieDeVotare(string codJudet, int numarSectie)
        {
            JudetEnum judet;
            var j = Enum.TryParse(codJudet, true, out judet);

            if (!j)
                throw new ArgumentException($"Judet inexistent: {codJudet}");

            return await GetSingleSectieDeVotare((int) judet, numarSectie);

        }

        public async Task<int> GetSingleSectieDeVotare(int idJudet, int numarSectie)
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
                throw new ArgumentException($"Sectie inexistenta pentru: {new  { idJudet, idSectie}}");


            if (idSectie.Count > 1)
                throw new ArgumentException($"S-au gasit mai multe sectii pentru: {new { idJudet, idSectie }}");

            return idSectie.Single();
        }
    }
}

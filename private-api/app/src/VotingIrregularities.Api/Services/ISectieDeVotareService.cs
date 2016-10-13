using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Services
{
    public interface ISectieDeVotareService
    {
        Task<int> GetSingleSectieDeVotare(string codJudet, int numarSectie);
        Task<int> GetSingleSectieDeVotare(int idJudet, int numarSectie);
    }
}

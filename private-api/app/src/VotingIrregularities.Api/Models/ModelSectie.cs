using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Models
{
    public class ModelSectie
    {
        public int IdSectieDeVotare { get; set; }
        public string Judet { get; set; }
        public string Oras { get; set; }
        public string AdresaSectie { get; set; }
    }
}

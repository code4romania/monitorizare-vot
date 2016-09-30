using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Models
{
    public class ModelNota
    {
        public string CodJudet { get; set; }
        public int NumarSectie { get; set; }
        public string CodFormular { get; set; }
        public int IdIntrebare { get; set; }
        public string TextNota { get; set; }

    }
}

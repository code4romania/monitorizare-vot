using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Models
{
    public class ModelSectiune
    {
        public string CodSectiune { get; set; }
        public string Descriere { get; set; }

        public List<ModelIntrebare> Intrebari { get; set; }
    }
}

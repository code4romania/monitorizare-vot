using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Models
{
    public class ModelDateSectie
    {
        public string CodJudet { get; set; }
        public int NumarSectie { get; set; }
        public DateTime? OraSosirii { get; set; }
        public DateTime? OraPlecarii { get; set; }
        public bool? EsteZonaUrbana { get; set; }
        public bool? PresedinteBesvesteFemeie { get; set; }
    }
}

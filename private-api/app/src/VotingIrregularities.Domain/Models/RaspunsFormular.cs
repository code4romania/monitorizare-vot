using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class RaspunsFormular
    {
        public int IdObservator { get; set; }
        public int IdSectieDeVotare { get; set; }
        public DateTime DataUltimeiModificari { get; set; }
        public bool? EsteZonaUrbana { get; set; }
        public DateTime? OraPlecarii { get; set; }
        public DateTime? OraSosirii { get; set; }
        public bool? PresedinteBesvesteFemeie { get; set; }

        public virtual Observator IdObservatorNavigation { get; set; }
        public virtual SectieDeVotare IdSectieDeVotareNavigation { get; set; }
    }
}

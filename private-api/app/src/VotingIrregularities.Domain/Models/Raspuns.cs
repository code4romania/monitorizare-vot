using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Raspuns
    {
        public int IdObservator { get; set; }
        public int IdRaspunsDisponibil { get; set; }
        public int IdSectieDeVotare { get; set; }
        public DateTime DataUltimeiModificari { get; set; }
        public string Value { get; set; }
        public string CodJudet { get; set; }
        public int NumarSectie { get; set; }

        public virtual Observator IdObservatorNavigation { get; set; }
        public virtual RaspunsDisponibil IdRaspunsDisponibilNavigation { get; set; }
        public virtual SectieDeVotare IdSectieDeVotareNavigation { get; set; }
    }
}

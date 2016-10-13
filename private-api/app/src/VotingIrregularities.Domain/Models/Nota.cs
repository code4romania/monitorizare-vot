using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Nota
    {
        public int IdNota { get; set; }
        public string CaleFisierAtasat { get; set; }
        public DateTime DataUltimeiModificari { get; set; }
        public int? IdIntrebare { get; set; }
        public int IdObservator { get; set; }
        public int IdSectieDeVotare { get; set; }
        public string TextNota { get; set; }

        public virtual Intrebare IdIntrebareNavigation { get; set; }
        public virtual Observator IdObservatorNavigation { get; set; }
        public virtual SectieDeVotare IdSectieDeVotareNavigation { get; set; }
    }
}

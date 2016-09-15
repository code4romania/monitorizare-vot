using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Raspuns
    {
        public int IdObservator { get; set; }
        public int IdSectieDeVotare { get; set; }
        public int IdIntrebare { get; set; }
        public int IdOptiune { get; set; }
        public string Value { get; set; }
        public DateTime DataUltimeiModificari { get; set; }

        public virtual Intrebare IdIntrebareNavigation { get; set; }
        public virtual Observator IdObservatorNavigation { get; set; }
        public virtual Optiune IdOptiuneNavigation { get; set; }
        public virtual SectieDeVotare IdSectieDeVotareNavigation { get; set; }
    }
}

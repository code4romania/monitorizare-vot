using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class RaspunsDisponibil
    {
        public int IdRaspunsDisponibil { get; set; }
        public int IdIntrebare { get; set; }
        public int IdOptiune { get; set; }
        public bool RaspunsCuFlag { get; set; }

        public virtual Intrebare IdIntrebareNavigation { get; set; }
        public virtual Optiune IdOptiuneNavigation { get; set; }
    }
}

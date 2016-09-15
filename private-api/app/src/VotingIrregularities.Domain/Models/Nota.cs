using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Nota
    {
        public int IdNota { get; set; }
        public int IdIntrebare { get; set; }
        public string TextNota { get; set; }
        public string CaleFisierAtasat { get; set; }
        public DateTime DataUltimeiModificari { get; set; }

        public virtual Intrebare IdIntrebareNavigation { get; set; }
    }
}

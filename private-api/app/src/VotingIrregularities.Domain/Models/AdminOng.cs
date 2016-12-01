using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class AdminOng
    {
        public int IdAdminOng { get; set; }
        public int IdOng { get; set; }
        public string Cont { get; set; }
        public string Parola { get; set; }

        public virtual Ong IdOngNavigation { get; set; }
    }
}

using System;
using System.Collections.Generic;

namespace VotingIrregularities.Api.Models
{
    public class ModelIntrebare
    {

        public int IdIntrebare { get; set; }
        public string TextIntrebare { get; set; }
        public int IdTipIntrebare { get; set; }
        public string CodIntrebare { get; set; }

        public List<ModelRaspunsDisponibil> RaspunsuriDisponibile { get; set; }
    }
}

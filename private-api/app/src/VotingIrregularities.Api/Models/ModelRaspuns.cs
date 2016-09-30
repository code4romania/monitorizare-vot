using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VotingIrregularities.Api.Models
{
    public class ModelRaspuns
    {
        public int IdIntrebare { get; set; }
        public string CodJudet { get; set; }
        public int NumarSectie { get; set; }
        public string CodFormular { get; set; }
        public List<ModelOptiuniSelectate> Optiuni { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using VotingIrregularities.Domain.NotaAggregate;

namespace VotingIrregularities.Api.Models
{
    public class ModelNota
    {
        [Required(AllowEmptyStrings = false)]
        public string CodJudet { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int NumarSectie { get; set; }
        public int IdIntrebare { get; set; }
        public string TextNota { get; set; }

    }

    public class ModelNoteBulk : IAsyncRequest<AdaugaNotaCommand>
    {
        public IEnumerable<ModelNota> Note { get; set; }

        public ModelNoteBulk(ModelNota[] note)
        {
            Note = note;
        }
    }
}

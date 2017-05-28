using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MediatR;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;

namespace VotingIrregularities.Api.Models
{
    public class ModelRaspunsWrapper
    {
        public ModelRaspunsBulk[] Raspuns { get; set; }
    }
    public class ModelRaspunsBulk
    {
        [Required]
        public int IdIntrebare { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string CodJudet { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int NumarSectie { get; set; }

        //[Required(AllowEmptyStrings = false)]
        public string CodFormular { get; set; }
        public List<ModelOptiuniSelectate> Optiuni { get; set; }
    }

    public class RaspunsuriBulk : IRequest<CompleteazaRaspunsCommand>
    {
        public RaspunsuriBulk(IEnumerable<ModelRaspunsBulk> raspunsuri)
        {
            ModelRaspunsuriBulk = raspunsuri.ToList();
        }

        public int IdObservator { get; set; }

        public List<ModelRaspunsBulk> ModelRaspunsuriBulk { get; set; }
    }
}

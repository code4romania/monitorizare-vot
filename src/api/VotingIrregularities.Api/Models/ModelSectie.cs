using MediatR;

namespace VotingIrregularities.Api.Models
{
    public class ModelSectieQuery : IRequest<int>
    {
        public string CodJudet { get; set; }
        public int NumarSectie { get; set; }
    }
}

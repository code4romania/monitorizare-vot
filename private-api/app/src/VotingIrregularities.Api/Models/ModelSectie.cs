using MediatR;

namespace VotingIrregularities.Api.Models
{
    public class ModelSectieQuery : IAsyncRequest<int>
    {
        public string CodJudet { get; set; }
        public int NumarSectie { get; set; }
    }
}

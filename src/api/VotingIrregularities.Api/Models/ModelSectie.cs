using MediatR;
using System;

namespace VotingIrregularities.Api.Models
{
    [Obsolete("Use PollingStationQuery instead.")]
    public class ModelSectieQuery : IRequest<int>
    {
        public string CodJudet { get; set; }
        public int NumarSectie { get; set; }
    }
}

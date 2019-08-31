using System.Collections.Generic;
using VoteMonitor.Entities;

namespace VotingIrregularities.Api.Models
{
    public class ModelVersiune
    {
        /// <summary>
        /// Collection of <see cref="FormVersion"/>
        /// </summary>
        public List<FormVersion> Formulare { get; set; }
    }
}

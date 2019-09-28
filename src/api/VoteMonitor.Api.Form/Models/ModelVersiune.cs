using System.Collections.Generic;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Models
{
    public class ModelVersiune
    {
        /// <summary>
        /// Collection of <see cref="FormVersion"/>
        /// </summary>
        public List<Entities.Form> Formulare { get; set; }
    }
}

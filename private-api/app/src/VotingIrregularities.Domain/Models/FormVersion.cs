using System;

namespace VotingIrregularities.Domain.Models
{
    public partial class FormVersion
    {
        public string FormCode { get; set; }
        public int CurrentVersion { get; set; }
        public string Name { get; set; }
        public DateTime AvailabilityDate { get; set; }
        public int Order { get; set; }
    }
}
